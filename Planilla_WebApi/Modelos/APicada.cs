using System.ComponentModel.DataAnnotations;

namespace Planilla_WebApi.Modelos
{
    public class APicada
    {
        public int ID { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int Id_Sucursales { get; set; }
        public int Id_Productos_A { get; set; }
        public string? Desc_A { get; set; }
        public float Kilos_A { get; set; }
        public float Costo_A { get; set; }

        public int Id_Productos_S { get; set; }
        public string? Desc_S { get; set; }
        public float Costo_S { get; set; }
        public float Kilos_S { get; set; }

    }
}
