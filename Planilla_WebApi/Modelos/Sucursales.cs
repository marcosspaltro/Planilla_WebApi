using Microsoft.EntityFrameworkCore;

namespace Planilla_WebApi.Modelos
{
    public class Sucursales
    {
        public int Id { get; set; }
        public string ?Nombre { get; set; }
        public bool Ver { get; set; }
        public int Tipo { get; set; }
        public bool Propio { get; set; }
    }
}
