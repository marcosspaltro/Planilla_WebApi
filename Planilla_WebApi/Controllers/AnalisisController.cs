using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
using System.Text.RegularExpressions;


namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalisisController : Controller
    {
        dbAnalisis db = new dbAnalisis();

        //// hhtpget para obtener las sucursales
        //[HttpGet]
        //public IList<Ventas> GetVentasAnuales(int tipo = 0, bool mostrarSucs = false)
        //{
        //    var ventas = db.VentaAnuales(tipo, mostrarSucs)?.ToList();
        //    // Retornar la lista de sucursales
        //    return ventas ?? new List<Ventas>();
        //}

        [HttpGet]
        public IList<Analisis> GetAnalisis(DateTime fecha, int suc = 0)
        {
            var analisis = db.Analisis(fecha, suc)?.ToList();

            return analisis ?? new List<Analisis>();
        }

    }
}

