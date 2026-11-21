using System.Collections.Concurrent;

namespace Planilla_WebApi.Services
{
    /// <summary>
    /// Límite de intentos de login: después de varios intentos fallidos seguidos para un mismo
    /// usuario, o desde una misma dirección, se rechaza el login por unos minutos.
    /// Se guarda en memoria (se reinicia al reiniciar la API).
    /// </summary>
    public static class IntentosLogin
    {
        public const int MaximoPorUsuario = 5;
        public const int MaximoPorDireccion = 20;
        public static readonly TimeSpan Ventana = TimeSpan.FromMinutes(15);

        private static readonly ConcurrentDictionary<string, (int Cantidad, DateTime Desde)> fallidos =
            new(StringComparer.OrdinalIgnoreCase);

        private static string ClaveUsuario(string usuario) => "u:" + usuario;
        private static string ClaveDireccion(string ip) => "ip:" + ip;

        public static bool Bloqueado(string usuario, string ip)
        {
            return Supera(ClaveUsuario(usuario), MaximoPorUsuario) || Supera(ClaveDireccion(ip), MaximoPorDireccion);
        }

        public static void Fallido(string usuario, string ip)
        {
            Sumar(ClaveUsuario(usuario));
            Sumar(ClaveDireccion(ip));
        }

        public static void Correcto(string usuario, string ip)
        {
            fallidos.TryRemove(ClaveUsuario(usuario), out _);
        }

        private static bool Supera(string clave, int maximo)
        {
            if (!fallidos.TryGetValue(clave, out var dato))
            {
                return false;
            }

            if (DateTime.Now - dato.Desde > Ventana)
            {
                fallidos.TryRemove(clave, out _);
                return false;
            }

            return dato.Cantidad >= maximo;
        }

        private static void Sumar(string clave)
        {
            fallidos.AddOrUpdate(clave,
                _ => (1, DateTime.Now),
                (_, dato) => DateTime.Now - dato.Desde > Ventana ? (1, DateTime.Now) : (dato.Cantidad + 1, dato.Desde));
        }
    }
}
