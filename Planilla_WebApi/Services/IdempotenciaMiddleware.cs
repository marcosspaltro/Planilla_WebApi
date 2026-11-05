using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Services
{
    /// <summary>
    /// Evita registros repetidos cuando la web reenvía un dato (por ejemplo, la carga
    /// sin conexión que se envía al volver la conexión, o un reintento después de un corte).
    ///
    /// La web manda en cada POST/PUT/PATCH/DELETE el encabezado "Idempotency-Key" con un
    /// identificador único por dato cargado; si se reenvía, va con la misma clave.
    /// - Primera vez: se procesa normalmente y, si sale bien (2xx), se guarda la respuesta.
    /// - Ya procesada: NO se vuelve a grabar; se devuelve la misma respuesta guardada.
    /// - Todavía en proceso (llegó dos veces a la vez): 409, la web la reintenta después.
    /// - Si falló (4xx/5xx o excepción): se libera la clave para que se pueda reintentar.
    /// Los pedidos sin el encabezado funcionan igual que antes.
    /// La tabla IdempotenciaClaves se crea sola al iniciar.
    /// </summary>
    public class IdempotenciaMiddleware
    {
        public const string Encabezado = "Idempotency-Key";
        private static readonly string[] Metodos = { "POST", "PUT", "PATCH", "DELETE" };
        private static readonly TimeSpan VencimientoEnProceso = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan Retencion = TimeSpan.FromDays(30);
        private static DateTime _ultimaLimpieza = DateTime.MinValue;

        /// <summary>Queda en false si no se pudo crear la tabla: la API sigue funcionando como antes.</summary>
        public static bool Habilitado { get; set; }

        private readonly RequestDelegate _next;
        private readonly string _conexion;
        private readonly ILogger<IdempotenciaMiddleware> _log;

        public IdempotenciaMiddleware(RequestDelegate next, IConfiguration config, ILogger<IdempotenciaMiddleware> log)
        {
            _next = next;
            _conexion = config.GetConnectionString("DefaultConnection") ?? "";
            _log = log;
        }

        public static void CrearTabla(string conexion)
        {
            const string ddl = @"
IF OBJECT_ID('dbo.IdempotenciaClaves', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.IdempotenciaClaves (
        Clave       NVARCHAR(100) NOT NULL PRIMARY KEY,
        Metodo      NVARCHAR(10)  NOT NULL,
        Ruta        NVARCHAR(400) NOT NULL,
        Usuario     NVARCHAR(100) NULL,
        Estado      INT           NOT NULL,  -- 0 = en proceso, si no el código HTTP
        TipoContenido NVARCHAR(200) NULL,
        Cuerpo      NVARCHAR(MAX) NULL,
        Creado      DATETIME2     NOT NULL DEFAULT SYSDATETIME()
    );
END";
            using var cn = new SqlConnection(conexion);
            cn.Open();
            using var cmd = new SqlCommand(ddl, cn);
            cmd.ExecuteNonQuery();
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            string? clave = ctx.Request.Headers[Encabezado].FirstOrDefault();
            if (!Habilitado
                || string.IsNullOrWhiteSpace(clave)
                || !Metodos.Contains(ctx.Request.Method.ToUpperInvariant())
                || clave.Length > 100)
            {
                await _next(ctx);
                return;
            }

            string ruta = (ctx.Request.Path + ctx.Request.QueryString).ToString();
            if (ruta.Length > 400) ruta = ruta.Substring(0, 400);

            await using var cn = new SqlConnection(_conexion);
            await cn.OpenAsync();
            await LimpiarViejasAsync(cn);

            // 1) Intentar "tomar" la clave. Si ya existe, es un reenvío.
            if (!await TomarClaveAsync(cn, clave, ctx.Request.Method, ruta, ctx.User?.Identity?.Name))
            {
                var previa = await LeerClaveAsync(cn, clave);
                if (previa != null && previa.Value.Estado > 0)
                {
                    // Ya se grabó: devolver la misma respuesta sin volver a grabar
                    ctx.Response.StatusCode = previa.Value.Estado;
                    ctx.Response.Headers["Idempotent-Replayed"] = "true";
                    if (!string.IsNullOrEmpty(previa.Value.TipoContenido))
                        ctx.Response.ContentType = previa.Value.TipoContenido;
                    if (!string.IsNullOrEmpty(previa.Value.Cuerpo))
                        await ctx.Response.WriteAsync(previa.Value.Cuerpo);
                    return;
                }
                if (previa != null && DateTime.Now - previa.Value.Creado < VencimientoEnProceso)
                {
                    ctx.Response.StatusCode = StatusCodes.Status409Conflict;
                    await ctx.Response.WriteAsync("El mismo envío ya se está procesando.");
                    return;
                }
                // Quedó colgada (se cortó el servidor a mitad): se libera y se toma de nuevo
                await BorrarClaveAsync(cn, clave);
                if (!await TomarClaveAsync(cn, clave, ctx.Request.Method, ruta, ctx.User?.Identity?.Name))
                {
                    ctx.Response.StatusCode = StatusCodes.Status409Conflict;
                    await ctx.Response.WriteAsync("El mismo envío ya se está procesando.");
                    return;
                }
            }

            // 2) Procesar el pedido capturando la respuesta
            var original = ctx.Response.Body;
            using var buffer = new MemoryStream();
            ctx.Response.Body = buffer;
            bool ok = false;
            try
            {
                await _next(ctx);
                ok = ctx.Response.StatusCode >= 200 && ctx.Response.StatusCode < 300;
            }
            finally
            {
                ctx.Response.Body = original;
                try
                {
                    if (ok)
                    {
                        buffer.Position = 0;
                        string cuerpo = await new StreamReader(buffer).ReadToEndAsync();
                        await GuardarResultadoAsync(cn, clave, ctx.Response.StatusCode, ctx.Response.ContentType, cuerpo);
                    }
                    else
                    {
                        await BorrarClaveAsync(cn, clave);
                    }
                }
                catch (Exception e)
                {
                    _log.LogError(e, "No se pudo actualizar la clave de idempotencia {Clave}", clave);
                }
                buffer.Position = 0;
                await buffer.CopyToAsync(original);
            }
        }

        private static async Task<bool> TomarClaveAsync(SqlConnection cn, string clave, string metodo, string ruta, string? usuario)
        {
            const string sql = @"
INSERT INTO dbo.IdempotenciaClaves (Clave, Metodo, Ruta, Usuario, Estado)
SELECT @clave, @metodo, @ruta, @usuario, 0
WHERE NOT EXISTS (SELECT 1 FROM dbo.IdempotenciaClaves WITH (UPDLOCK, HOLDLOCK) WHERE Clave = @clave);";
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.Add("@clave", SqlDbType.NVarChar, 100).Value = clave;
            cmd.Parameters.Add("@metodo", SqlDbType.NVarChar, 10).Value = metodo;
            cmd.Parameters.Add("@ruta", SqlDbType.NVarChar, 400).Value = ruta;
            cmd.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value = (object?)usuario ?? DBNull.Value;
            try
            {
                return await cmd.ExecuteNonQueryAsync() == 1;
            }
            catch (SqlException e) when (e.Number == 2627 || e.Number == 2601)
            {
                return false; // clave duplicada: otro pedido la tomó al mismo tiempo
            }
        }

        private static async Task<(int Estado, string? TipoContenido, string? Cuerpo, DateTime Creado)?> LeerClaveAsync(SqlConnection cn, string clave)
        {
            using var cmd = new SqlCommand(
                "SELECT Estado, TipoContenido, Cuerpo, Creado FROM dbo.IdempotenciaClaves WHERE Clave = @clave", cn);
            cmd.Parameters.Add("@clave", SqlDbType.NVarChar, 100).Value = clave;
            using var dr = await cmd.ExecuteReaderAsync();
            if (!await dr.ReadAsync()) return null;
            return (dr.GetInt32(0),
                    dr.IsDBNull(1) ? null : dr.GetString(1),
                    dr.IsDBNull(2) ? null : dr.GetString(2),
                    dr.GetDateTime(3));
        }

        private static async Task GuardarResultadoAsync(SqlConnection cn, string clave, int estado, string? tipo, string cuerpo)
        {
            using var cmd = new SqlCommand(
                "UPDATE dbo.IdempotenciaClaves SET Estado = @estado, TipoContenido = @tipo, Cuerpo = @cuerpo WHERE Clave = @clave", cn);
            cmd.Parameters.Add("@clave", SqlDbType.NVarChar, 100).Value = clave;
            cmd.Parameters.Add("@estado", SqlDbType.Int).Value = estado;
            cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 200).Value = (object?)tipo ?? DBNull.Value;
            cmd.Parameters.Add("@cuerpo", SqlDbType.NVarChar, -1).Value = cuerpo;
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task BorrarClaveAsync(SqlConnection cn, string clave)
        {
            using var cmd = new SqlCommand("DELETE FROM dbo.IdempotenciaClaves WHERE Clave = @clave", cn);
            cmd.Parameters.Add("@clave", SqlDbType.NVarChar, 100).Value = clave;
            await cmd.ExecuteNonQueryAsync();
        }

        // Una vez por día se borran las claves de más de 30 días
        private async Task LimpiarViejasAsync(SqlConnection cn)
        {
            if (DateTime.Now - _ultimaLimpieza < TimeSpan.FromDays(1)) return;
            _ultimaLimpieza = DateTime.Now;
            try
            {
                using var cmd = new SqlCommand("DELETE FROM dbo.IdempotenciaClaves WHERE Creado < @limite", cn);
                cmd.Parameters.Add("@limite", SqlDbType.DateTime2).Value = DateTime.Now - Retencion;
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _log.LogWarning(e, "No se pudieron borrar claves de idempotencia viejas");
            }
        }
    }
}
