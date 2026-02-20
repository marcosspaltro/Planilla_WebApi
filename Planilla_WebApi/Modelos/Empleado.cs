namespace Planilla_WebApi.Modelos
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Sucursal { get; set; }
        public DateTime Alta { get; set; }
        public DateTime Baja { get; set; }
    }
}
