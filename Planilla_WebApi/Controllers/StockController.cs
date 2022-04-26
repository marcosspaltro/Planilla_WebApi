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
        // GET: api/<Stock>
        [HttpGet]
        public IList<Stock> Get()
        {
            dbDatos datos = new dbDatos();
            return datos.Stocks();
        }

        // GET api/<Stock>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Stock>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Stock>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Stock>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
