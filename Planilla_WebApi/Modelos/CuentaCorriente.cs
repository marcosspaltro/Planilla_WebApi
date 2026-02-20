namespace Planilla_WebApi.Modelos
{
    public class CuentaCorriente
    {
        public int ID { get; set; }
        public DateTime Fecha { get; set; }
        public int Sucursal { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public float Importe { get; set; }
        public bool Entrada { get; set; }
        public int Tipo { get; set; }
    }
}
