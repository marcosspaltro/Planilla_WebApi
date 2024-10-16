using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Stock_sub
    {

        
        public int id_prod { get; set; }

        public string fecha { get; set; }
        public float kilos { get; set; }
        public int suc { get; set; }

        public Stock_sub()
        {
        }
    }
}