namespace Planilla_WebApi.Modelos
{
    public class frMercaderiaSemana
    {
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int  Items { get; set; }
        public float Kilos { get; set; }        
        public float Costo { get; set; }
        public float Total { get; set; }
    }
}
