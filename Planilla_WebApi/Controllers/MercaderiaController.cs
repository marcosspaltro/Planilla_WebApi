using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MercaderiaController : ControllerBase
    {
        //// GET: api/<MercaderiaController>
        //[HttpGet(Name = "GetMercaderia"), Authorize]
        //public IList<Stock> Get(int suc, int tipo = 0, string fecha = "1/1/2000", string fecha2 = "1/1/2000")
        //{
        //    //dbDatos datos = new dbDatos();
        //    //return datos.Mercaderia(suc, tipo, fecha, fecha2);
        //    //return Ok(null);
        //}

        // GET api/<MercaderiaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MercaderiaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MercaderiaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MercaderiaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
