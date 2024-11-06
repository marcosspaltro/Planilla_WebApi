using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
using Microsoft.AspNetCore.Authorization;

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

        // GET: api/Ventas/2023-10-15
        [HttpGet("{fecha}"), Authorize]
        public async Task<ActionResult<IEnumerable<Ventas>>> GetVentasByFecha(DateTime fecha)
        {
            var Ventas = await _context.vw_Ventas
                .Where(o => o.Fecha == fecha)
                .OrderBy(o => o.Id)
                .ToListAsync();

            if (Ventas == null || Ventas.Count == 0)
            {
                return NotFound();
            }

            return Ventas;
        }
        // Suc / fechaDesde / fechaHasta
        // GET: api/Ventas/1/2023-10-15/2023-10-15
        [HttpGet("{suc}/{fechaD}/{fechaH}"), Authorize]
        public async Task<ActionResult<IEnumerable<Ventas>>> GetVentasByFechaSuc(int suc, DateTime fechaD, DateTime fechaH)
        {
            var Ventas = await _context.vw_Ventas
                .Where(o => o.Fecha >= fechaD && o.Fecha <= fechaH && o.Id_Sucursales == suc)
                .OrderBy(o => o.Fecha)
                .ThenBy(o => o.Id)
                .ToListAsync();

            if (Ventas == null || Ventas.Count == 0)
            {
                return NotFound();
            }

            return Ventas;
        }

        // PUT: api/Ventas/2023-10-15/sucursal1/producto1
        [HttpPut("{fecha}/{sucursal}/{producto}"), Authorize]
        public async Task<IActionResult> UpdateVenta(DateTime fecha, int sucursal, int producto, [FromBody] Ventas updatedVenta)
        {
            var Venta = await _context.vw_Ventas
                .FirstOrDefaultAsync(o => o.Fecha == fecha && o.Id_Sucursales == sucursal && o.Id_Productos == producto);

            if (Venta == null)
            {
                return NotFound();
            }

            // Actualizamos los valores
            Venta.Kilos = updatedVenta.Kilos;

            _context.Entry(Venta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al actualizar la Venta.");
            }

            return NoContent();
        }

        // POST: api/Ventas
        [HttpPost, Authorize]
        public async Task<ActionResult<Ventas>> CreateVenta([FromBody] Ventas nuevaVenta)
        {
            _context.vw_Ventas.Add(nuevaVenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVentasByFecha), new { fecha = nuevaVenta.Fecha }, nuevaVenta);
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
