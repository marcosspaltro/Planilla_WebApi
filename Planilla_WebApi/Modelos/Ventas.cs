using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class Ventas
    {               

        public int Id { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int ID_Camion { get; set; }        
        public int Id_Proveedores { get; set; }        
        public string Nombre_Proveedor { get; set; }
        public int Id_Sucursales { get; set; }        
        public int Id_Productos { get; set; }
        public string? Descripcion { get; set; }
        public double Kilos { get; set; }
        public int Cantidad { get; set; }

       
    }
}
