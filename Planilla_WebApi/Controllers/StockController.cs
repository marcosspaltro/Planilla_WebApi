using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;


namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        public StockController(IConfiguration configuration, IUserService userService)
        {            
        }


        //   GET: api/<Stock>
        [HttpGet(Name = "GetStock")]
        public IList<Modelos.Stock> Get(int sucursal)
        {
            Conexiones.dbStock datos = new Conexiones.dbStock();

            if (sucursal > 1000) { sucursal -= 1000; }
            return datos.Stocks(sucursal);
        }


        //   POST: api/Stock
        [HttpPost(Name = "PostStock"), Authorize]
        public ActionResult POST([FromBody] Stock_sub s)
        {
            Conexiones.dbStock datos = new Conexiones.dbStock();

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
