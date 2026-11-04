using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Modelos;
using Planilla_WebApi.Conexiones;
namespace Planilla_WebApi.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class APicadaController : ControllerBase
    {
        private readonly dbAPicada _dbAPicada;

        public APicadaController()
        {
            _dbAPicada = new dbAPicada();
        }

        //   GET: api/APicada
        [HttpGet(Name = "GetSemana")]
        public IList<APicada> GetSemana(int suc, DateTime fecha)
        {
            return _dbAPicada.A_PicadaSemana(fecha, suc);
        }

        //   GET: api/tabla
        [HttpGet("tabla", Name = "GetTabla")]
        public IList<APicada> GetTabla()
        {
            return _dbAPicada.Tabla();
        }

        // POST
        [HttpPost, Authorize]
        public IActionResult POST([FromBody] APicada aPicada)
        {
            bool result = _dbAPicada.Agregar(aPicada);
            if (result)
            {
                return Ok(new { message = "Registro agregado exitosamente." });
            }
            else
            {
                return BadRequest("Error al agregar el registro.");
            }
        }
    }
}
