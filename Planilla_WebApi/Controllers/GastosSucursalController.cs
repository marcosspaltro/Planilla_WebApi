using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Modelos;
using Planilla_WebApi.Services;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GastosSucursalController : Controller
    {

        [HttpGet]
        public IList<gastosSucursales> Get(int Suc)
        {
            Conexiones.dbGastosSucursal datos = new Conexiones.dbGastosSucursal();
            return datos.Gastos(Suc);
        }

        [HttpGet]
        [Route("TiposGastos")]
        public IList<tipoGastosSucursales> GetTiposGastos()
        {
            Conexiones.dbGastosSucursal datos = new Conexiones.dbGastosSucursal();
            return datos.TiposGastos();
        }

        [HttpPost, Authorize]
        public ActionResult Post([FromBody] gastosSucursales gasto)
        {
            Conexiones.dbGastosSucursal datos = new Conexiones.dbGastosSucursal();
            int id = datos.AgregarGasto(gasto);

            if (id > 0)
                return Ok(id);
            else
                return BadRequest("No se pudo agregar el gasto.");
        }

        [HttpDelete("{id}"), Authorize]
        public ActionResult Delete(int id)
        {
            Conexiones.dbGastosSucursal datos = new Conexiones.dbGastosSucursal();
            if (RestriccionFecha.EsSucursal(HttpContext))
            {
                DateTime? fechaGuardada = datos.FechaGasto(id);
                if (fechaGuardada != null && !RestriccionFecha.FechaPermitida(HttpContext, fechaGuardada.Value))
                {
                    return BadRequest(RestriccionFecha.Mensaje);
                }
            }

            bool borrado = datos.BorrarGasto(id);

            if (borrado)
                return Ok($"Gasto con id {id} borrado correctamente.");
            else
                return NotFound($"No se encontró el gasto con id {id} o no se pudo borrar.");
        }
    }
}
