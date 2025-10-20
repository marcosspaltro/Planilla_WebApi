using Microsoft.AspNetCore.Mvc;

namespace Planilla_WebApi.Controllers
{
    public class SemanasController : Controller
    {
        // GET: api/semanas
        [HttpGet("api/semanas")]
        public IList<Modelos.Semanas> Get(int cantidad = 10)
        {
            Conexiones.dbSemanas datos = new Conexiones.dbSemanas();
            return datos.Semanas(cantidad) ?? new List<Modelos.Semanas>();
        }
    }
}
