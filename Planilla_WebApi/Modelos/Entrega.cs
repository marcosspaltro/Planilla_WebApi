using Microsoft.Win32;

namespace Planilla_WebApi.Modelos
{
    public class Entrega
    {
        public int ID { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public int Sucursal { get; set; }
        public Int16 Tipo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public float Importe { get; set; }
        public bool Aprobado { get; set; }


    }

    public class  Tipo_Entrega
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;

    }
}
