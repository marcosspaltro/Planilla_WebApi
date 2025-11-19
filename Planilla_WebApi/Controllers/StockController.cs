using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        //   GET: api/Stock
        [HttpGet(Name = "GetStock")]
        public IList<Modelos.Stock> Get(int sucursal, DateTime semana)
        {
            Conexiones.dbStock datos = new Conexiones.dbStock();
            
          
            return datos.Stocks(sucursal, semana, false);
        }

        [HttpGet]
        [Route("StockAnt")]
        public IList<Modelos.Stock> GetAnt(int sucursal, DateTime semana)
        {
            Conexiones.dbStock datos = new Conexiones.dbStock();
                        
            return datos.Stocks(sucursal, semana, true);
        }


        //   POST: api/Stock
        [HttpPost(Name = "PostStock"), Authorize]
        public ActionResult POST([FromBody] Stock_sub s)
        {
            Conexiones.dbStock datos = new Conexiones.dbStock();

            //s.fecha = DateTime.Today.AddDays(-7);
            //datos.Agregar_registro(s.suc, s.fecha, s.id_prod, s.kilos);
            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);

            }
            try
            {
                s.fecha = DateTime.Today.AddDays(-7);
                datos.Agregar_registro(s.suc, s.fecha, s.id_prod, s.kilos);
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
