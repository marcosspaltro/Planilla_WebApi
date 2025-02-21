using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreciosController : ControllerBase
    {
        public PreciosController(IConfiguration configuration, IUserService userService)
        {
        }


        //   GET: api/<Precios>
        [HttpGet(Name = "GetPrecios")]
        public IList<Modelos.Precios> Get(int sucursal)
        {
            Conexiones.dbPrecios datos = new Conexiones.dbPrecios();

            if (sucursal > 1000) { sucursal -= 1000; }
            return datos.Precios(sucursal);
        }
    }
}
