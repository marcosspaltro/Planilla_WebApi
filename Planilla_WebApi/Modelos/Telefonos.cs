using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Telefonos
    {               

        public int ID { get; set; } = 0;

        [DataType(DataType.Date)]
        public string? Cuit { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Enlace { get; set; }
        public string? N_cliente { get; set; }

        public Telefonos()
        {
        }
    }
}
