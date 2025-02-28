using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Pedidos
    {               

        public int ID { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int Sucursal { get; set; }
        public int Producto { get; set; }
        public string? Descripcion { get; set; }
        public float Kilos { get; set; }
        public float Kilos1 { get; set; }
        public float Kilos2 { get; set; }
        public float Kilos3 { get; set; }
        public int Tipo { get; set; } = 0;

        public Pedidos()
        {
        }
    }
}
