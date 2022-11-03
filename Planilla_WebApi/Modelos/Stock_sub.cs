using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Stock_sub
    {

        public string _id { get; set; }
        public int id_prod { get; set; }

        public DateTime fecha { get; set; }
        public float kilos { get; set; }
        public int suc { get; set; }

        public Stock_sub()
        {
        }
    }
}