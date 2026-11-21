namespace Planilla_WebApi.Conexiones
{
    /// <summary>
    /// Cadena de conexión a SQL Server para las clases de Conexiones.
    /// Se carga al iniciar (Program.cs) desde ConnectionStrings:DefaultConnection de la
    /// configuración del servidor, así la contraseña no queda escrita en el código.
    /// </summary>
    public static class Conexion
    {
        public static string Cadena { get; set; } = string.Empty;
    }
}
