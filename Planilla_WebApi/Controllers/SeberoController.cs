using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Modelos;


namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeberoController : ControllerBase
    {
        public SeberoController(IConfiguration configuration, IUserService userService)
        {
        }

        [HttpGet(Name = "GetSebo")]
        public IList<Modelos.Sebero> Get(int suc)
        {
            DateTime fecha = DateTime.Today;
            Conexiones.dbSebero datos = new Conexiones.dbSebero();
            return datos.Sebo(suc, fecha);
        }

        //   POST: api/Sebero
        [HttpPost(Name = "PostSebero"), Authorize]
        public ActionResult POST([FromBody] Sebero s)
        {
            Conexiones.dbSebero datos = new Conexiones.dbSebero();

            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                //datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);

            }
            try
            {
                datos.Agregar(s);
            }
            catch (Exception e)
            {
                return BadRequest("Algo paso");
            }

            return Ok(datos.Id);

        }


    }
}
