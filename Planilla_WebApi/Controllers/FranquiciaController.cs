using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Modelos;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FranquiciaController : Controller
    {
        public FranquiciaController(IConfiguration configuration, IUserService userService)
        {
        }

        // GET: api/<Principal>
        [HttpGet(Name = "Principal")]
        public IList<Modelos.Principal> Get(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);

            return datos.Principal(fecha, sucursal);
        }

        // GET: api/carne
        [HttpGet("carne")]
        public IList<frCarne> GetCarne(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);

            return datos.Carne(fecha, sucursal);

        }

        // GET: api/mercaderia_semana
        [HttpGet("mercaderia_semana")]
        public IList<frMercaderiaSemana> GetMercaderiaSemana(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);

            return datos.MercadeiaSemana(fecha, sucursal);

        }

        // GET: api/mercaderia_dia
        [HttpGet("mercaderia_dia")]
        public IList<Ventas> GetMercaderiaDia(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbVentas datos = new Conexiones.dbVentas();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);

            return datos.Ventas(sucursal, fecha);

        }

        // GET: api/gastos_semana
        [HttpGet("gastos_semana")]
        public IList<frMercaderiaSemana> GetGastosSemana(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();


            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);

            return datos.GastosSemana(fecha, sucursal);

        }

        // GET: api/descartes_semana
        [HttpGet("descartes_semana")]
        public IList<frMercaderiaSemana> GetDescartesSemana(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);
            return datos.DescartesSemana(fecha, sucursal);
        }

        // GET: api/traslados_salida
        [HttpGet("traslados_salida")]
        public IList<Traslados> GetTrasldosSalida(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);
            return datos.TrasladosSemana(fecha, sucursal, true);
        }
        // GET: api/traslados_entrada
        [HttpGet("traslados_entrada")]
        public IList<Traslados> GetTrasldosEntrada(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);
            return datos.TrasladosSemana(fecha, sucursal, false);
        }

        // GET: api/gastos_oficina
        [HttpGet("gastos_oficina")]
        public IList<frGastosOficina> GetGastosOficina(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);
            return datos.GastosOficina(fecha, sucursal);
        }

        // GET: api/efectivos_semana
        [HttpGet("efectivos_semana")]
        public IList<frGastosOficina> GetEfectivosSemana(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);
            return datos.EfectivosSemana(fecha, sucursal);
        }

        // POST: api/efectivo
        [HttpPost("efectivo"), Authorize]
        public ActionResult Post([FromBody] Efectivo efectivo)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            int id = datos.AgregarEfectivo(efectivo);

            if (id > 0)
                return Ok(id);
            else
                return BadRequest("No se pudo agregar el gasto.");
        }

        // GET: api/tarjetas
        [HttpGet("tarjetas")]
        public IList<frTarjetas> GetTarjetas(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);
            return datos.TarjetasSemana(fecha, sucursal, false);
        }
        // GET: api/tarjetas
        [HttpGet("tarjetassemana")]
        public IList<frTarjetas> GetTarjetasSemana(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);
            return datos.TarjetasSemana(fecha, sucursal, true);
        }

        // GET: api/stockant
        [HttpGet("stockant")]
        public IList<Modelos.Stock> GetStockAnt(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            // convertir la fecha al lunes de la semana anterior
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday).AddDays(-7);
            return datos.Stock(fecha, sucursal);
        }
    }
}
