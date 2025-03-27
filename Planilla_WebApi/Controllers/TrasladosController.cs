using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrasladosController : ControllerBase
    {
        public TrasladosController(IConfiguration configuration, IUserService userService)
        {
        }


        //   GET: api/<Traslados>
        [HttpGet(Name = "GetTraslados")]
        public IList<Modelos.Traslados> Get(int sucS, int sucE, int tipo)
        {
            DateTime fecha = DateTime.Today;
            Conexiones.dbTraslados datos = new Conexiones.dbTraslados();
                        
            return datos.Traslados(sucS, sucE, fecha, tipo);
        }


        //   POST: api/Traslados
        [HttpPost(Name = "PostTraslados"), Authorize]
        public ActionResult POST([FromBody] Traslados t)
        {
            Conexiones.dbTraslados datos = new Conexiones.dbTraslados();

            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                //datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);

            }
            try
            {
                datos.Agregar(t);
            }
            catch (Exception e)
            {
                return BadRequest("Algo paso");
            }


            if (datos.Id > 0)
            {
                return Ok(datos.Id);
            }
            else
            {

                return BadRequest("Algo paso");
            }

        }



    }
}
