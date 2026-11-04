namespace Planilla_WebApi.Modelos
{
    public class Efectivo
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int  Sucursal { get; set; }
        public float Importe { get; set; }
        public bool Cargado { get; set; } = false;
    }
}
