using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Planilla_WebApi.Conexiones;

namespace Planilla_WebApi.Services
{
    /// <summary>
    /// Marca un controlador o acción que puede recibir fechas anteriores aunque el usuario
    /// sea de una sucursal (por ejemplo cargas semanales como Stock u Ofertas).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermitirFechasAnterioresAttribute : Attribute
    {
    }

    /// <summary>
    /// Reglas de "solo el día de hoy" para los usuarios de sucursal (nivel 1).
    /// Una sucursal solo puede crear, modificar o borrar datos con la fecha de hoy;
    /// los días anteriores quedan de solo lectura. Los usuarios de oficina no se ven afectados.
    /// </summary>
    public static class RestriccionFecha
    {
        public const int NivelSucursal = 1;

        // Cabecera que manda la carga sin conexión con el momento en que se cargó el dato,
        // para que un envío diferido (cargado hoy, enviado mañana) no se rechace.
        public const string CabeceraFechaCarga = "X-Fecha-Carga";

        // Cuántos días hacia atrás se acepta un envío diferido de la carga sin conexión.
        public const int DiasMaximosEnvioDiferido = 3;

        public const string Mensaje = "Solo se pueden cargar o modificar datos del día de hoy.";

        private static readonly ConcurrentDictionary<string, (int Nivel, DateTime Vence)> cacheNiveles =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>Nivel del usuario logueado, o null si no se pudo determinar.</summary>
        public static int? NivelUsuario(HttpContext http)
        {
            var usuario = http.User?.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usuario))
            {
                return null;
            }

            if (cacheNiveles.TryGetValue(usuario, out var guardado) && guardado.Vence > DateTime.Now)
            {
                return guardado.Nivel;
            }

            IList<User>? usuarios;
            try
            {
                usuarios = new dbUsuarios().Usuarios();
            }
            catch (Exception ex)
            {
                // Si no se puede consultar el nivel no se bloquea la carga
                Console.WriteLine(ex.Message);
                return null;
            }

            if (usuarios == null)
            {
                return null;
            }

            var u = usuarios.FirstOrDefault(x => string.Equals(x.Username, usuario, StringComparison.OrdinalIgnoreCase));
            if (u == null)
            {
                return null;
            }

            cacheNiveles[usuario] = (u.Nivel, DateTime.Now.AddMinutes(10));
            return u.Nivel;
        }

        public static bool EsSucursal(HttpContext http)
        {
            return NivelUsuario(http) == NivelSucursal;
        }

        /// <summary>
        /// Día que se considera "hoy" para este pedido: el de hoy, o el que manda la carga
        /// sin conexión en X-Fecha-Carga si es de los últimos días.
        /// </summary>
        public static DateTime DiaDeCarga(HttpContext http)
        {
            var hoy = DateTime.Today;
            var cabecera = http.Request.Headers[CabeceraFechaCarga].ToString();

            if (!string.IsNullOrEmpty(cabecera) &&
                DateTimeOffset.TryParse(cabecera, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var cargado))
            {
                var dia = cargado.ToLocalTime().Date;
                if (dia <= hoy && dia >= hoy.AddDays(-DiasMaximosEnvioDiferido))
                {
                    return dia;
                }
            }

            return hoy;
        }

        /// <summary>
        /// True si una sucursal puede tocar un dato con esta fecha. Se rechazan solo los días
        /// anteriores: una fecha posterior no se bloquea porque varias pantallas viejas mandan la
        /// fecha en UTC y después de las 21 hs llega como el día siguiente.
        /// </summary>
        public static bool FechaPermitida(HttpContext http, DateTime fecha)
        {
            // Las fechas que llegan con "Z" (UTC) se pasan a hora local antes de comparar
            if (fecha.Kind == DateTimeKind.Utc)
            {
                fecha = fecha.ToLocalTime();
            }

            return fecha.Date >= DiaDeCarga(http);
        }
    }

    /// <summary>
    /// Filtro global: en POST/PUT/DELETE de un usuario de sucursal, toda "fecha" que venga
    /// en la ruta, la query o el cuerpo tiene que ser la de hoy. Los borrados o modificaciones
    /// por Id se controlan además en cada controlador contra la fecha guardada.
    /// </summary>
    public class SoloFechaDeHoyFilter : IActionFilter
    {
        private static readonly HashSet<string> metodosQueModifican =
            new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH", "DELETE" };

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;

            if (!metodosQueModifican.Contains(http.Request.Method))
            {
                return;
            }

            if (http.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            if (context.ActionDescriptor.EndpointMetadata.OfType<PermitirFechasAnterioresAttribute>().Any())
            {
                return;
            }

            // Solo se buscan fechas si hay alguna, para no consultar el nivel en cada pedido
            var fechas = new List<DateTime>();
            foreach (var arg in context.ActionArguments)
            {
                JuntarFechas(arg.Key, arg.Value, fechas);
            }

            if (fechas.Count == 0)
            {
                return;
            }

            if (!RestriccionFecha.EsSucursal(http))
            {
                return;
            }

            if (fechas.Any(f => !RestriccionFecha.FechaPermitida(http, f)))
            {
                context.Result = new BadRequestObjectResult(RestriccionFecha.Mensaje);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private static bool EsNombreFecha(string nombre)
        {
            return string.Equals(nombre, "fecha", StringComparison.OrdinalIgnoreCase);
        }

        private static void JuntarFechas(string nombre, object? valor, List<DateTime> fechas)
        {
            if (valor == null)
            {
                return;
            }

            if (valor is DateTime fechaSimple)
            {
                if (EsNombreFecha(nombre))
                {
                    fechas.Add(fechaSimple);
                }
                return;
            }

            var tipo = valor.GetType();
            if (tipo.IsPrimitive || valor is string || valor is decimal)
            {
                return;
            }

            // Listas de registros
            if (valor is IEnumerable lista)
            {
                foreach (var item in lista)
                {
                    JuntarFechas(nombre, item, fechas);
                }
                return;
            }

            // Objetos del cuerpo: solo la propiedad Fecha de primer nivel
            foreach (var prop in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!EsNombreFecha(prop.Name) || prop.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                var valorProp = prop.GetValue(valor);
                if (valorProp is DateTime f)
                {
                    fechas.Add(f);
                }
            }
        }
    }
}
