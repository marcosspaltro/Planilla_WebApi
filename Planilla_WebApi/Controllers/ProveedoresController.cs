using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores/Ver
        [HttpGet("Ver")]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedoresVerTrue()
        {
            // Obtener proveedores con Ver en true
            var proveedores = await _context.Proveedores
                .Where(p => p.Ver == true)
                .OrderBy(p => p.Tipo)
                .ThenBy(p => p.Id)
                .ToListAsync();

            return Ok(proveedores);
        }
    }
}
