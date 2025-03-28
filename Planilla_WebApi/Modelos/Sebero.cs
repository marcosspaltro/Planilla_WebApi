namespace Planilla_WebApi.Modelos
{
    public class Sebero
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int Id_Seberos { get; set; }
        public int Id_Sucursales { get; set; }
        public int Id_Productos { get; set; }
        public float Costo { get; set; } = 0;
        public float Kilos { get; set; }
        public string? Descripcion { get; internal set; }
    }
}
