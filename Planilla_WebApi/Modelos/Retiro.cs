namespace Planilla_WebApi.Modelos
{
    public class Retiro
    {
        public int ID { get; set; }
        
        /// <summary>
        /// Semana
        /// </summary>
        public DateTime Fecha { get; set; }
        
        public int Empleado { get; set; }
        
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Suc
        /// </summary>
        public int Sucursal { get; set; }
        
        public int Tipo { get; set; }

        public string Descripcion_Tipo { get; set; } = string.Empty;

        public int Tipo_Descuento { get; set; }
        
        public int Dias { get; set; }
        
        public DateTime Fecha_Imputacion { get; set; }
        /// <summary>
        /// Dias_vCobrados
        /// </summary>
        public int Dias_Cobrados { get; set; }
        
        public string Nota { get; set; } = string.Empty;
        
        public Single Importe { get; set; }

    }
}
