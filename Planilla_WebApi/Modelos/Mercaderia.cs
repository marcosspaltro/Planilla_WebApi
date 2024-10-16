using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Mercaderia
    {               

        public int ID { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int Camion { get; set; }        
        public int Proveedor { get; set; }
        public string nProveedor { get; set; }
        public int Sucursal { get; set; }
        public string nSucursal { get; set; }
        public int Producto { get; set; }
        public string? Descripcion { get; set; }
        public float Kilos { get; set; }
        public float Cantidad { get; set; }

        public Mercaderia()
        {
        }
    }
}
