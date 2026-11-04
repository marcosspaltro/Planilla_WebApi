using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CantClientesController : ControllerBase
    {
        // GET: api/CantClientes
        [HttpGet(Name = "Cant_Clientes")]
        public IList<Cant_Clientes> GetCantClientes(int sucursal, DateTime fecha, bool sumado = true)
        {
            dbCantClientes datos = new dbCantClientes();
            return datos.CantClientes(sucursal, fecha, sumado);
        }

        // POST: api/CantClientes
        [HttpPost(Name = "PostCantClientes"), Authorize]
        public ActionResult POST([FromBody] Cant_Clientes c)
        {
            dbCantClientes datos = new dbCantClientes();
            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                //datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);
            }
            try
            {
                datos.Agregar(c);
            }
            catch (Exception e)
            {
                return BadRequest("Algo paso");
            }
            return Ok(200);
        }
    }
}
