using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            dbDatos datos = new dbDatos();
            return datos.Stocks(suc, tipo, semana);
        }


        //   POST: api/Stock
        [HttpPost(Name = "PostStock"), Authorize]
        public ActionResult POST([FromBody] Stock_sub s)
        {
            dbDatos datos = new dbDatos();

            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);

            }
            try
            {
                //datos.escribirLog(s.fecha);
                datos.Agregar_registro(s.fecha, s.suc, s.id_prod, s.kilos);
            }
            catch (Exception e)
            {
                datos.escribirLog(e.Message);
                return BadRequest("Algo paso");
            }

            return Ok(datos.Id);

            //if (datos.Id > 0)
            //{
            //    return Ok(datos.Id);
            //}
            //else
            //{

            //    datos.escribirLog("Algo paso aca");
            //    return BadRequest("Algo paso");
            //}

        }

    }
}
