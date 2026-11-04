using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Planilla_WebApi.Modelos
{
    public class Cant_Clientes
    {
        public DateTime fecha { get; set; }
        public int sucursal { get; set; }
        public int cantidad { get; set; }
    }
}
