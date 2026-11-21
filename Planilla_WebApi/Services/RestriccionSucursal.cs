using System.Collections;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Planilla_WebApi.Conexiones;

namespace Planilla_WebApi.Services
{
    /// <summary>
    /// Marca una acción que un usuario de sucursal o de franquicia puede usar sin indicar su
    /// propia sucursal (por ejemplo la búsqueda de empleados por nombre para cargar un retiro).
    /// Usar solo en consultas que no devuelven datos de plata ni de movimientos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SinRestriccionSucursalAttribute : Attribute
    {
    }

    /// <summary>
    /// Qué sucursales puede ver o cargar cada usuario.
    /// Los usuarios de sucursal (nivel 1) y de franquicia (nivel 100) solo trabajan con su propia
    /// sucursal; oficina (los demás niveles) ve todas.
    /// </summary>
    public static class RestriccionSucursal
    {
        /// <summary>Niveles que solo pueden ver y cargar su propia sucursal.</summary>
        public static readonly HashSet<int> NivelesRestringidos = new() { 1, 100 };

        public const string Mensaje = "No tenés permiso para ver o cargar datos de otra sucursal.";

        // Nombres de parámetro o de propiedad que indican una sucursal.
        private static readonly HashSet<string> nombresSucursal = new(StringComparer.OrdinalIgnoreCase)
        {
            "sucursal", "suc", "f_suc", "id_sucursal", "id_sucursales"
        };

        // Traslados: alcanza con que la sucursal del usuario sea la de salida o la de entrada.
        private static readonly HashSet<string> nombresSalida = new(StringComparer.OrdinalIgnoreCase) { "sucS", "suc_salida" };
        private static readonly HashSet<string> nombresEntrada = new(StringComparer.OrdinalIgnoreCase) { "sucE", "suc_entrada" };

        /// <summary>
        /// Sucursal propia del usuario si está restringido a ella; null si puede ver todas.
        /// Si no se pudo saber quién es, se lo trata como restringido a ninguna (0).
        /// </summary>
        public static int? SucursalPermitida(HttpContext http)
        {
            var datos = RestriccionFecha.DatosUsuario(http);
            if (datos == null)
            {
                return 0;
            }

            return NivelesRestringidos.Contains(datos.Value.Nivel) ? datos.Value.Sucursal : null;
        }

        // Algunas pantallas mandan la sucursal + 1000 (ver Pedidos, Precios, Teléfonos).
        private static int Normalizar(int sucursal)
        {
            return sucursal > 1000 ? sucursal - 1000 : sucursal;
        }

        public static bool MismaSucursal(int pedida, int propia)
        {
            return pedida == propia || Normalizar(pedida) == Normalizar(propia);
        }

        /// <summary>True si el usuario puede tocar un registro guardado de esta sucursal.</summary>
        public static bool PuedeUsar(HttpContext http, int? sucursalGuardada)
        {
            var propia = SucursalPermitida(http);
            if (propia == null || sucursalGuardada == null)
            {
                return true;
            }

            return MismaSucursal(sucursalGuardada.Value, propia.Value);
        }

        /// <summary>
        /// Sucursal de un registro ya guardado, para borrar o modificar por Id.
        /// consulta: un SELECT de una sola columna con el parámetro @id.
        /// </summary>
        public static int? SucursalDeRegistro(string consulta, int id)
        {
            try
            {
                using var sql = new SqlConnection(Conexion.Cadena);
                using var cmd = new SqlCommand(consulta, sql);
                cmd.Parameters.AddWithValue("@id", id);
                sql.Open();
                var valor = cmd.ExecuteScalar();
                return valor == null || valor == DBNull.Value ? null : Convert.ToInt32(valor);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Consultas para los borrados y modificaciones por Id
        public const string SqlGasto = "SELECT ID_Sucursales FROM Gastos_Sucursales WHERE Id = @id";
        public const string SqlPedido = "SELECT Suc FROM Pedidos WHERE Id = @id";
        public const string SqlEfectivo = "SELECT Id_Sucursal FROM dbGastos.dbo.Efectivos WHERE Id = @id";
        public const string SqlEntregaSucursal = "SELECT Suc FROM [dbGastos].[dbo].[Entregas_Sucursales] WHERE ID = @id";
        public const string SqlEntregaFranquicia = "SELECT Suc FROM [dbGastos].[dbo].[Entregas_Franquicia] WHERE ID = @id";

        /// <summary>
        /// Junta las sucursales que vienen en la ruta, la query o el cuerpo.
        /// directas: tienen que ser todas la del usuario.
        /// pares: (salida, entrada) de traslados; alcanza con que una sea la del usuario.
        /// </summary>
        internal static void Juntar(string nombre, object? valor, List<int> directas, List<(int?, int?)> pares)
        {
            if (valor == null)
            {
                return;
            }

            if (valor is int numero)
            {
                if (nombresSucursal.Contains(nombre))
                {
                    directas.Add(numero);
                }
                return;
            }

            var tipo = valor.GetType();
            if (tipo.IsPrimitive || tipo.IsEnum || valor is string || valor is decimal || valor is DateTime)
            {
                return;
            }

            if (valor is IEnumerable lista)
            {
                foreach (var item in lista)
                {
                    Juntar(nombre, item, directas, pares);
                }
                return;
            }

            // Objetos del cuerpo: propiedades de primer nivel
            int? salida = null, entrada = null;
            foreach (var prop in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetIndexParameters().Length > 0 || prop.PropertyType != typeof(int))
                {
                    continue;
                }

                var v = (int)prop.GetValue(valor)!;
                if (nombresSucursal.Contains(prop.Name))
                {
                    directas.Add(v);
                }
                else if (nombresSalida.Contains(prop.Name))
                {
                    salida = v;
                }
                else if (nombresEntrada.Contains(prop.Name))
                {
                    entrada = v;
                }
            }

            if (salida != null || entrada != null)
            {
                pares.Add((salida, entrada));
            }
        }

        internal static bool EsSalida(string nombre) => nombresSalida.Contains(nombre);
        internal static bool EsEntrada(string nombre) => nombresEntrada.Contains(nombre);
    }

    /// <summary>
    /// Filtro global: un usuario de sucursal o de franquicia solo puede consultar o cargar datos
    /// de su propia sucursal. Revisa todo parámetro o propiedad del cuerpo que indique una
    /// sucursal (sucursal, suc, Id_Sucursales, etc.). Los borrados y modificaciones por Id
    /// se controlan además en cada controlador contra la sucursal guardada.
    /// </summary>
    public class RestriccionSucursalFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;

            // Sin usuario no se llega acá: la API exige token (ver Program.cs).
            if (http.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            if (context.ActionDescriptor.EndpointMetadata.OfType<SinRestriccionSucursalAttribute>().Any())
            {
                return;
            }

            var directas = new List<int>();
            var pares = new List<(int?, int?)>();
            int? salida = null, entrada = null;

            // Parámetros de la acción (ruta, query y cuerpo)
            foreach (var p in context.ActionDescriptor.Parameters)
            {
                context.ActionArguments.TryGetValue(p.Name, out var valor);

                if (p.ParameterType == typeof(int))
                {
                    // Un parámetro de sucursal que no vino cuenta como 0 ("todas" en varias consultas)
                    var numero = valor as int? ?? 0;
                    if (RestriccionSucursal.EsSalida(p.Name)) { salida = numero; continue; }
                    if (RestriccionSucursal.EsEntrada(p.Name)) { entrada = numero; continue; }
                }

                RestriccionSucursal.Juntar(p.Name, valor ?? (p.ParameterType == typeof(int) ? 0 : null), directas, pares);
            }

            if (salida != null || entrada != null)
            {
                pares.Add((salida, entrada));
            }

            if (directas.Count == 0 && pares.Count == 0)
            {
                return;
            }

            var propia = RestriccionSucursal.SucursalPermitida(http);
            if (propia == null)
            {
                return;
            }

            bool permitido =
                directas.All(s => RestriccionSucursal.MismaSucursal(s, propia.Value)) &&
                pares.All(par =>
                    (par.Item1 != null && RestriccionSucursal.MismaSucursal(par.Item1.Value, propia.Value)) ||
                    (par.Item2 != null && RestriccionSucursal.MismaSucursal(par.Item2.Value, propia.Value)));

            if (!permitido)
            {
                context.Result = new ObjectResult(RestriccionSucursal.Mensaje) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
