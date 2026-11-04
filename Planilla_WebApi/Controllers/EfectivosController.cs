using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EfectivosController : ControllerBase
    {
        private dbEfectivos db = new dbEfectivos();

        public EfectivosController()
        {
        }

        // GET:
        [HttpGet("Efectivos")]
        public IList<Efectivo> GET(DateTime fecha, int suc)
        {
            return db.Efectivos(fecha, suc);
        }
        // GET:
        [HttpGet("semana")]
        public IList<Efectivo> GETsemana(DateTime fecha, int suc)
        {
            return db.Efectivos_semana(fecha, suc);
        }

        // POST
        [HttpPost, Authorize]
        public ActionResult Post(Efectivo efectivo)
        {
            int id = db.Nuevo_Efectivo(efectivo);
            if (id == 0)
            {
                return BadRequest();
            }
            else
            {
                return Ok(id);
            }
        }

        // PUT
        [HttpPut(), Authorize]
        public ActionResult Put(Efectivo efectivo)
        {
            int filas = db.Modificar_Efectivo(efectivo.Id, efectivo);
            if (filas == 0)
            {
                return BadRequest();
            }
            else
            {
                return Ok(filas);
            }
        }
    }
}

