using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    [Keyless]
    public class VentasTipo
    {
        
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }        
        public int Tipo { get; set; }
        public int Id_Sucursales { get; set; }
        public string? Descripcion { get; set; }
        public int Items { get; set; }
        public  float Kilos { get; set; }
        public float Total { get; set; }
    }
}
