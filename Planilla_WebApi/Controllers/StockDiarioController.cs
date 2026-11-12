using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    // Stock diario de carne y pollo (kilos) que carga cada sucursal desde el inicio de Sistema-Web
    [Route("api/[controller]")]
    [ApiController]
    public class StockDiarioController : ControllerBase
    {
        private readonly string _conexion;

        public StockDiarioController(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/StockDiario?sucursal=1&fecha=2026-11-04
        [HttpGet(Name = "GetStockDiario")]
        public IList<Stock_Diario> GetStockDiario(int sucursal, DateTime fecha)
        {
            dbStockDiario datos = new dbStockDiario(_conexion);
            return datos.StockDiario(sucursal, fecha);
        }

        // POST: api/StockDiario
        [HttpPost(Name = "PostStockDiario"), Authorize]
        public ActionResult POST([FromBody] Stock_Diario s)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (s.sucursal <= 0 || s.carne < 0 || s.pollo < 0)
            {
                return BadRequest("Datos inválidos");
            }
            try
            {
                dbStockDiario datos = new dbStockDiario(_conexion);
                datos.Guardar(s);
            }
            catch (Exception)
            {
                return BadRequest("Algo paso");
            }
            return Ok(200);
        }
    }
}
