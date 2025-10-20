namespace Planilla_WebApi.Modelos
{
    public class gastosSucursales
    {
        public int Id { get; set; }
        public int Id_Sucursales { get; set; }
        public DateTime Fecha { get; set; }

        public int Tipo { get; set; }

        public string Descripcion { get; set; }

        public float Monto { get; set; }

        public gastosSucursales()
        {
            Id = 0;
            Id_Sucursales = 0;
            Fecha = DateTime.Now;
            Descripcion = string.Empty;
            Monto = 0.0f;

        }
    }

    public class tipoGastosSucursales
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Grupo { get; set; }
        public int Rubro { get; set; }

        public tipoGastosSucursales()
        {
            Id = 0;
            Descripcion = string.Empty;
            Grupo = 0;
            Rubro = 0;
        }
    }

    public class gastosSucursalesGrupo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public gastosSucursalesGrupo()
        {
            Id = 0;
            Descripcion = string.Empty;
        }
    }
    public class gastosSucursalesRubro
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        
        public gastosSucursalesRubro()
        {
            Id = 0;
            Descripcion = string.Empty;
            
        }
    }
}