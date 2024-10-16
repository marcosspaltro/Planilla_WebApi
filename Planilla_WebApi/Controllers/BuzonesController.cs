using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuzonesController : ControllerBase
    {
        //private readonly ILogger<BuzonesController> _logger;

        //public BuzonesController(ILogger<BuzonesController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public BuzonesController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        //   GET: api/<Buzones>
        //[HttpGet(Name = "GetBuzones"), Authorize]
        //public IList<Buzones> Get(int suc, int )
        //{
        //    dbDatos datos = new dbDatos();
        //    return datos.Buzones(suc);
        //}

        //// GET api/<Buzones>/5
        //[HttpGet("{prod}"), Authorize]
        //public Buzones Get(int prod)
        //{
        //    dbDatos datos = new dbDatos();

        //    return datos.Buzoness(prod);
        //}

        // POST api/<Buzones>
        //[HttpPost, Authorize]
        //public void Post([FromBody] Buzones value)
        //{
        //    string jsonString = s.id_prod.ToString() + s.fecha.ToString() + s.kilos.ToString() + s.suc.ToString();
        //    System.IO.File.WriteAllText(@"D:\Tarjetas\EscribeTexto.txt", jsonString);
        //    return Ok("crack fiera idolo");
        //}

        // PUT api/<Buzones>/5

        [HttpPost(Name = "PostBuzones"), Authorize]
        public ActionResult POST([FromBody] Buzones s)
        {
            return Ok(0);
            //if (jsonString != null)
            //{
            //    char ch = '}';
            //    int t_prods = jsonString.Count(f => (f == ch));

            //    for (int i = 0; i < t_prods; i++)
            //    {
            //        string k = jsonString.Substring(jsonString.IndexOf("{"), jsonString.IndexOf("}") - jsonString.IndexOf("{") + 1);
            //        dbDatos.datamodel m = JsonConvert.DeserializeObject<dbDatos.datamodel>(k);
            //        jsonString = jsonString.Substring(jsonString.IndexOf("{") + 1);
            //        if (jsonString.IndexOf("{") > 0)
            //        { jsonString = jsonString.Substring(jsonString.IndexOf("{")); }

            //        dbDatos datos = new dbDatos();

            //        datos.Agregar_registro(m.fecha, m.suc, m.id_prod, m.kilos);
            //    }
            //dbDatos datos = new dbDatos();
            //int d = 0;
            //if (s.ID == 0)
            //{
            //   d = datos.Agregar_Buzones(s.Fecha, s.Importe, s.Sucursal);
            //} 
            //else {
            //   datos.Eliminar_Buzones(s.Fecha, s.Sucursal ,s.ID);
            //}
            //return Ok(d);
            //}
            //else { return BadRequest(); }
        }
        //// DELETE api/<Buzones>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
