using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
using Planilla_WebApi.Services;
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
                // Ya hay un efectivo cargado para esa sucursal y ese día
                return BadRequest("Ya hay un efectivo cargado para ese día. Para cambiar el importe, toque la fila en la tabla.");
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
            if (RestriccionFecha.EsSucursal(HttpContext))
            {
                DateTime? fechaGuardada = db.FechaEfectivo(efectivo.Id);
                if (fechaGuardada != null && !RestriccionFecha.FechaPermitida(HttpContext, fechaGuardada.Value))
                {
                    return BadRequest(RestriccionFecha.Mensaje);
                }
            }

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

