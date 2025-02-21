using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Precios
    {        
        public int Producto { get; set; }
        public string? Descripcion { get; set; }
        public float Precio { get; set; }
        public int Tipo { get; set; }

        public Precios()
        {
        }
    }
}
