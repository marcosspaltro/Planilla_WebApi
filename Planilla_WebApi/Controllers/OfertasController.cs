using Planilla_WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Carga semanal: la fecha es la de la semana, no la del día
    [PermitirFechasAnteriores]
    public class OfertasController : ControllerBase
    {
        public OfertasController(IConfiguration configuration, IUserService userService)
        {
        }


        //   GET: api/<Ofertas>
        [HttpGet(Name = "GetOfertas")]
        public IList<Modelos.Ofertas> Get(int sucursal, DateTime dia)
        {
            
            Conexiones.dbOfertas datos = new Conexiones.dbOfertas();
                        
            return datos.Ofertas(sucursal, dia);
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
                datos.Agregar_registro(s.id_sucursal, s.id_productos, s.oferta, s.kilos, s.fecha);
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