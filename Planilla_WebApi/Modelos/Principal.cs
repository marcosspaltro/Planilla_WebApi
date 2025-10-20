namespace Planilla_WebApi.Modelos
{
    public class Principal
    {
        public int id { get; set; }
        public string Descripcion { get; set; }        
        public string Funcion { get; set; }
        public float Valor { get; set; }
        /// <summary>
        /// Link para mostrar el detalle.
        /// </summary>
        public string Link { get; set; }
        public bool Suma { get; set; }
    }
}