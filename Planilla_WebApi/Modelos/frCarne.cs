using Planilla_WebApi.Conexiones;

namespace Planilla_WebApi.Modelos
{
    public class frCarne
    {
        public DateTime Fecha { get; set; }
        public string  Producto { get; set; } = string.Empty;
        public float Kilos { get; set; }
        public float Costo { get; set; }
        public float Total { get; set; }
    }
}
