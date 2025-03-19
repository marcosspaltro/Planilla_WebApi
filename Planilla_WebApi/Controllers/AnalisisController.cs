using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;


namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalisisController : Controller
    {
        dbAnalisis db = new dbAnalisis();
                
        // hhtpget para obtener las sucursales
        [HttpGet]
        public IList<Ventas> GetVentasAnuales(int tipo = 0, bool mostrarSuc = false)
        {
            var ventas = db.VentaAnuales(tipo, mostrarSuc)?.ToList();
            // Retornar la lista de sucursales
            return ventas ?? new List<Ventas>();
        }
    }


}
