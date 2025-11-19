namespace Planilla_WebApi.Modelos
{
    public class Traslados
    {
        public DateTime Fecha { get; set; }
        public int Suc_Salida { get; set; }
        public int Suc_Entrada { get; set; }
        public int Id_Productos { get; set; }
        public string ?Descripcion { get; set; }
        public float Kilos { get; set; }
        public float Costo_Salida { get; set; }
        public float Costo_Entrada { get; set; }
        public float Costo_SalidaFR { get; set; }
        public float Costo_EntradaFR { get; set; }
        public string Nombre_Entrada { get; set; } = string.Empty;
        public float  Total_Salida { get; set; }
        public int Id_Tipo { get; set; }

    }
}