﻿namespace Planilla_WebApi.Modelos
{
    public class Ofertas
    {
        public int id { get; set; }
        public int Orden { get; set; }
        public int id_sucursal { get; set; }
        public int id_productos { get; set; }
        public string descripcion { get; set; }
        public string oferta { get; set; }
        public float kilos { get; set; }
        public float precio { get; set; }
    }
}