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
    public class Cant_prodsController : ControllerBase
    {
        //private readonly ILogger<StockController> _logger;

        //public StockController(ILogger<StockController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public Cant_prodsController(IConfiguration configuration, IUserService userService)
{
            _configuration = configuration;
            _userService = userService;
        }

        //   GET: api/<Stock>
        [HttpGet(Name = "GetCant_prods"), Authorize]
        public int Get()
        {
            dbStock datos = new dbStock();
            return datos.cant_prods();
        }

    }
}
