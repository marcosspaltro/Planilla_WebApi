using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Stock
    {               

        public int ID { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int Sucursal { get; set; }
        public int Producto { get; set; }
        public string? Descripcion { get; set; }
        public float Kilos { get; set; }
        public int Tipo { get; set; }

        public Stock()
        {
        }
    }
}
