using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;

namespace Planilla_WebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // Esta es para enviar el resumen en el index.php
        // [HttpGet("{suc}"), Authorize]
        // GET: api/Ventas/1
        [HttpGet("{sucursal}")]
        public async Task<ActionResult<IEnumerable<VentasTipo>>> GetVentasByFecha(int sucursal)
        {
            DateTime fecha = DateTime.Today;
            
            try
            {
                var Ventas = await _context.vw_VentasTipo
                    .Where(o => o.Fecha == fecha && o.Id_Sucursales == sucursal)
                    .OrderBy(o => o.Tipo)
                    .ToListAsync();

                if (Ventas == null || Ventas.Count == 0)
                {
                    return NotFound();
                }

                return Ventas;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Ventas/1/7
        [HttpGet("{suc}/{tipo}")]
        public async Task<ActionResult<IEnumerable<Ventas>>> GetVentasByFecha(int suc, int tipo)
        {
            DateTime fecha = new DateTime(2025, 2, 18);
            //suc = 10;
            fecha = DateTime.Today;

            dbVentas db = new dbVentas();
            IList<Ventas> Ventas = db.Ventas(suc, fecha, tipo);

            if (Ventas == null || Ventas.Count == 0)
            {
                return NotFound();
            }

            return Ok(Ventas);
        }

        // Suc / fecha / tipo
        // GET: api/Ventas/1/2023-10-15/1
        [HttpGet("{suc}/{fecha}/{tipo}"), Authorize]
        public async Task<ActionResult<IEnumerable<Ventas>>> GetVentasByFechaSuc(int suc, DateTime fecha, int tipo)
        {
            var Ventas = await _context.vw_Ventas
                .Where(o => o.Fecha >= fecha && o.Id_Sucursales == suc && o.Tipo == tipo)
                .OrderBy(o => o.Id_Productos)
                .ToListAsync();

            if (Ventas == null || Ventas.Count == 0)
            {
                return NotFound();
            }

            return Ventas;
        }
               

        // POST: api/Ventas
        [HttpPost, Authorize]
        public async Task<ActionResult<Ventas>> CreateVenta([FromBody] Ventas nuevaVenta)
        {
            _context.vw_Ventas.Add(nuevaVenta);

            try
            {
                dbVentas db = new dbVentas ();
                db.Agregar(nuevaVenta);
            }
            catch (Exception er)
            {
                return StatusCode(500, er.Message);
            }            
            return Ok(nuevaVenta);
        }

        // DELETE: api/Ventas/2023-10-15/sucursal1/producto1
        [HttpDelete("{fecha}/{sucursal}/{producto}"), Authorize]
        public async Task<IActionResult> DeleteVenta(DateTime fecha, int sucursal, int producto)
        {
            var Venta = await _context.vw_Ventas
                .FirstOrDefaultAsync(o => o.Fecha == fecha && o.Id_Sucursales == sucursal && o.Id_Productos == producto);

            if (Venta == null)
            {
                return NotFound();
            }

            _context.vw_Ventas.Remove(Venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
