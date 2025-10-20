namespace Planilla_WebApi.Modelos
{
    public class frGastosOficina
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public float Importe { get; set; }
    }
}
