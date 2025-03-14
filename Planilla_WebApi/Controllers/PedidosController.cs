using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;


namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        public PedidosController(IConfiguration configuration, IUserService userService)
        {            
        }


        //   GET: api/<Pedidos>
        [HttpGet(Name = "GetPedidos")]
        public IList<Modelos.Pedidos> Get(int sucursal)
        {
            Conexiones.dbPedidos datos = new Conexiones.dbPedidos();

            if (sucursal > 1000) { sucursal -= 1000; }
            return datos.Pedidoss(sucursal);
        }

        [HttpGet("{sucursal}/{Prod}")]
        public IList<Modelos.Pedidos> Get(int sucursal, int Prod)
        {
            Conexiones.dbPedidos datos = new Conexiones.dbPedidos();

            if (sucursal > 1000) { sucursal -= 1000; }
            return datos.Detalle_Pedidos(sucursal, Prod);
        }

        //   POST: api/Pedidos
        [HttpPost(Name = "PostPedidos"), Authorize]
        public ActionResult POST([FromBody] Pedidos_sub s)
        {
            Conexiones.dbPedidos datos = new Conexiones.dbPedidos();

            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);

            }
            try
            {
                datos.Agregar_registro(s.suc, s.id_prod, s.kilos);
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