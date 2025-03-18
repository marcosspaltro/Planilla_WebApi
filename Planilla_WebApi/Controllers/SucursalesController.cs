using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planilla_WebApi.Conexiones;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalesController : Controller
    {
        private readonly AppDbContext _context;
        public SucursalesController(AppDbContext context)
        {
            _context = context;
        }

        // hhtpget para obtener las sucursales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sucursales>>> GetSucursales()
        {
            // Crear una lista de sucursales
            var sucursales = await _context.Sucursales
                .Where(o => o.Ver == true && o.Propio == true || o.Id==6002)
                .OrderBy(o => o.Id)
                .ToListAsync();
            // Retornar la lista de sucursales
            return sucursales;
        }
    }
}
