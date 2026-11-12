using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
using Planilla_WebApi.Services;
namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Las entregas solo se cargan, modifican o borran el día de hoy, para todos los usuarios
    [SoloHoyParaTodos]
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

        // PUT: api/Entregas/entrega/5?importe=100
        [HttpPut("entrega/{id}"), Authorize]
        public ActionResult Put(int id, float importe)
        {
            Conexiones.dbEntregas datos = new Conexiones.dbEntregas();
            if (!RestriccionFecha.PuedeModificarGuardado(HttpContext, datos.FechaEntrega(id)))
            {
                return BadRequest(RestriccionFecha.Mensaje);
            }

            if (datos.Actualizar_Entrega(id, importe))
                return Ok();
            else
                return BadRequest("No se pudo modificar la entrega.");
        }

        // DELETE: api/Entregas/entrega/5
        [HttpDelete("entrega/{id}"), Authorize]
        public ActionResult Delete(int id)
        {
            Conexiones.dbEntregas datos = new Conexiones.dbEntregas();
            if (!RestriccionFecha.PuedeModificarGuardado(HttpContext, datos.FechaEntrega(id)))
            {
                return BadRequest(RestriccionFecha.Mensaje);
            }

            if (datos.Eliminar_Entrega(id))
                return Ok();
            else
                return BadRequest("No se pudo eliminar la entrega.");
        }
    }
}
