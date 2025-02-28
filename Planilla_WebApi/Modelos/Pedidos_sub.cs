using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Pedidos_sub
    {

        
        public int id_prod { get; set; }

        public float kilos { get; set; }
        public int suc { get; set; }

        public Pedidos_sub()
        {
        }
    }
}