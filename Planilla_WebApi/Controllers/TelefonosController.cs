using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;


namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelefonosController : ControllerBase
    {
        public TelefonosController(IConfiguration configuration, IUserService userService)
        {            
        }


        //   GET: api/<Tels>
        [HttpGet(Name = "GetTels")]
        public IList<Modelos.Telefonos> Get(int sucursal)
        {
            Conexiones.dbTel datos = new Conexiones.dbTel();

            if (sucursal > 1000) { sucursal -= 1000; }
            return datos.Telefonos(sucursal);
        }
    }
}
