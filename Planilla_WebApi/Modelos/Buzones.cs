using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Buzones
    {

        public int ID { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public float Importe { get; set; }
        public int Sucursal { get; set; }

        public Buzones()
        {
        }
    }
}
