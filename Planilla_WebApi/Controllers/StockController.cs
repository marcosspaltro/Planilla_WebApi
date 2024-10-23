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
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public StockController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }


        //   GET: api/<Stock>
        [HttpGet(Name = "GetStock"), Authorize]
        public IList<Stock> Get(int suc, int tipo = 0, string semana = "1/1/2000")
        {
            dbStock datos = new dbStock();
            return datos.Stocks(suc, tipo, semana);
        }


        //   POST: api/Stock
        [HttpPost(Name = "PostStock"), Authorize]
        public ActionResult POST([FromBody] Stock_sub s)
        {
            dbStock datos = new dbStock();

            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);

            }
            try
            {
                datos.Agregar_registro(s.fecha, s.suc, s.id_prod, s.kilos);
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
