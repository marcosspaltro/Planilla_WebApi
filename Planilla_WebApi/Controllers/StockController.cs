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
        private readonly ILogger<StockController> _logger;

        public StockController(ILogger<StockController> logger)
        {
            _logger = logger;
        }

        // GET: api/<Stock>
        [HttpGet(Name = "GetStock"), Authorize]
        public IList<Stock> Get()
        {
            dbDatos datos = new dbDatos();
            return datos.Stocks();
        }

        //// GET api/<Stock>/5
        //[HttpGet("{prod}"), Authorize]
        //public Stock Get(int prod)
        //{
        //    dbDatos datos = new dbDatos();

        //    return datos.Stocks(prod);
        //}

        //// POST api/<Stock>
        //[HttpPost, Authorize]
        //public void Post([FromBody] Stock value)
        //{
        //    dbDatos datos = new dbDatos();
        //    datos.Actualizar(value);
        //}

        //// PUT api/<Stock>/5
        //[HttpPut, Authorize]
        //public void Put([FromBody] Stock value)
        //{
        //    dbDatos datos = new dbDatos();
        //    datos.Actualizar(value);
        //}

        //// DELETE api/<Stock>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
