namespace Planilla_WebApi.Modelos
{
    // Un producto del listado de Stock Carne/Pollo (Productos con PedirStock = 1)
    // con los kilos que cargó la sucursal ese día (null si todavía no lo cargó)
    public class Stock_Diario_Producto
    {
        public int id { get; set; }
        public string nombre { get; set; } = "";
        public decimal? kilos { get; set; }
    }

    // Lo que manda la web al guardar el listado completo del día
    public class Stock_Diario_Carga
    {
        public DateTime fecha { get; set; }
        public int sucursal { get; set; }
        public List<Stock_Diario_Producto> productos { get; set; } = new();
    }
}
