using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntregasController : ControllerBase
    {

        public EntregasController() { }

        // GET: api/tipos_entregas
        [HttpGet("tipos_entregas")]
        public IList<Tipo_Entrega> GetTiposEntregas()
        {
            Conexiones.dbEntregas datos = new Conexiones.dbEntregas();
            return datos.TiposEntregas();
        }

        // GET: api/entregas_semana
        [HttpGet("entregas_semana")]
        public IList<Entrega> GetEntregas_semana(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbEntregas datos = new Conexiones.dbEntregas();
            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);

            return datos.Entregas_semana(fecha, sucursal);
        }

        // GET: api/entregas
        [HttpGet("entregas")]
        public IList<Entrega> GetEntregas(DateTime fecha, int sucursal = 1, int tipo = 100)
        {
            Conexiones.dbEntregas datos = new Conexiones.dbEntregas();

            return datos.Entregas(fecha, sucursal, tipo);
        }

        // POST: api/entrega
        [HttpPost("entrega"), Authorize]
        public ActionResult Post([FromBody] Entrega entrega)
        {
            Conexiones.dbEntregas datos = new Conexiones.dbEntregas();
            int id = datos.Nueva_Entrega(entrega);
            if (id > 0)
                return Ok(id);
            else
                return BadRequest("No se pudo agregar la entrega.");
        }
    }
}
