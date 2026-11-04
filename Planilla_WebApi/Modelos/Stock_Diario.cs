namespace Planilla_WebApi.Modelos
{
    // Stock diario de carne y pollo (en kilos) de una sucursal
    public class Stock_Diario
    {
        public DateTime fecha { get; set; }
        public int sucursal { get; set; }
        public decimal carne { get; set; }
        public decimal pollo { get; set; }
    }
}
