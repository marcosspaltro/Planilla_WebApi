using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertasController : ControllerBase
    {
        public OfertasController(IConfiguration configuration, IUserService userService)
        {
        }


        //   GET: api/<Ofertas>
        [HttpGet(Name = "GetOfertas"), Authorize]
        public IList<Modelos.Ofertas> Get(int suc, int tipo = 0, string semana = "1/1/2000")
        {
            Conexiones.dbOfertas datos = new Conexiones.dbOfertas();
            return datos.Ofertas(suc, tipo, semana);
        }


        //   POST: api/Ofertas
        [HttpPost(Name = "PostOfertas"), Authorize]
        public ActionResult POST([FromBody] Ofertas s)
        {
            Conexiones.dbOfertas datos = new Conexiones.dbOfertas();

            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);

            }
            try
            {
                datos.Agregar_registro(s.fecha.ToString(), s.id_sucursal, s.id_productos, s.kilos);
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