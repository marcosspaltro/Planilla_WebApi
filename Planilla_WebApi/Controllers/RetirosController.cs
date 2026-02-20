using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetirosController : ControllerBase
    {
        dbRetiros retiros = new dbRetiros();

        // GET Listado de tipo de retiros
        [HttpGet("Tipos_Retiro", Name = "Tipo_Retiros")]
        public IList<TipoRetiro> GetTipoRetiros()
        {
            return retiros.GetTipoRetiros();
        }

        // GET Listado de retiros de la semana en la sucursal seleccionada
        [HttpGet("Retiros_Semana", Name = "Retiros_Semana")]
        public IList<Retiro> GetRetirosSemana(int sucursal, DateTime semana)
        {
            dbRetiros datos = new dbRetiros();
            return datos.Retiros_Semana(sucursal, semana);

        }

        // GET Listado de empleados con opcinal de nombre a buscar
        [HttpGet("Empleados", Name = "Empleados")]
        public IList<Empleado> GetEmpleados(int sucursal, string? nombre)
        {
            dbRetiros datos = new dbRetiros();
            return datos.Empleados(sucursal, nombre);
        }

        // POST: api/Retiros
        [HttpPost(Name = "PostRetiros"), Authorize]
        public ActionResult POST([FromBody] Retiro s)
        {
            dbRetiros datos = new dbRetiros();
            // Validar el modelo recibido
            if (!ModelState.IsValid)
            {
                //datos.escribirLog("Modelo invalido");
                return BadRequest(ModelState);
            }
            try
            {
                datos.Agregar(s);
            }
            catch (Exception e)
            {
                return BadRequest("Algo paso");
            }
            return Ok(200);
        }

    }
}
