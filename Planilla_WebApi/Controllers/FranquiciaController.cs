using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planilla_WebApi.Conexiones;
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

            return datos.MercaderiaSemana(fecha, sucursal);

        }

        // GET: api/mercaderia_dia
        [HttpGet("mercaderia_dia")]
        public IList<frMercaderiaSemana> GetMercaderiaDia(DateTime fecha, int sucursal, int tipo)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();


            return datos.MercaderiaDia(fecha, sucursal, tipo);

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

        // GET: api/stock
        [HttpGet("stock")]
        public IList<Modelos.Stock> GetStock(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            return datos.Stock(fecha, sucursal);
        }
        // GET: api/tipos_entregas
        [HttpGet("tipos_entregas")]
        public IList<Tipo_Entrega> GetTiposEntregas()
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            return datos.TiposEntregas();
        }

        // GET: api/entregas_semana
        [HttpGet("entregas_semana")]
        public IList<Entrega> GetEntregas_semana(DateTime fecha, int sucursal = 1)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            // convertir la fecha al lunes de la semana
            fecha = fecha.AddDays(-(int)fecha.DayOfWeek + (int)DayOfWeek.Monday);

            return datos.Entregas_semana(fecha, sucursal);
        }

        // GET: api/entregas
        [HttpGet("entregas")]
        public IList<Entrega> GetEntregas(DateTime fecha, int sucursal = 1, int tipo = 100)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();

            return datos.Entregas(fecha, sucursal, tipo);
        }

        // POST: api/entrega
        [HttpPost("entrega"), Authorize]
        public ActionResult Post([FromBody] Entrega entrega)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            int id = datos.Nueva_Entrega(entrega);
            if (id > 0)
                return Ok(id);
            else
                return BadRequest("No se pudo agregar la entrega.");
        }

        // DELETE: api/entrega/5
        [HttpDelete("entrega/{id}"), Authorize]
        public ActionResult Delete(int id)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            bool result = datos.Eliminar_Entrega(id);
            if (result)
                return Ok();
            else
                return BadRequest("No se pudo eliminar la entrega.");
        }

        // PUT: api/entrega/5
        [HttpPut("entrega/{id}"), Authorize]
        public ActionResult Put(int id, float importe)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            bool result = datos.Actualizar_Entrega(id, importe);
            if (result)
                return Ok();
            else
                return BadRequest("No se pudo actualizar la entrega.");
        }


        // GET: cuenta_corriente
        [HttpGet("cuenta_corriente/{suc}")]
        public IList<CuentaCorriente> GetCuentaCorriente(int suc, [FromQuery] DateTime semana)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            return datos.CuentaCorriente(suc, semana);
        }

        // GET: cuenta_corriente/detalle
        [HttpGet("cuenta_corriente/detalle/{suc}")]
        public IList<frMercaderiaSemana> GetCuentaCorrienteDetalle(int suc, [FromQuery] DateTime semana, [FromQuery] int id, [FromQuery] int tipo)
        {
            Conexiones.dbFranquicia datos = new Conexiones.dbFranquicia();
            //0.Saldo anterior
            //1.Ventas
            //2.Carne
            //3.Traslados entrada
            //4.Gastos Empresa
            //5.Entregas
            //6.Franquicia
            //7.Saldo
            //8.Traslados salida
            IList<frMercaderiaSemana> _datos = new List<frMercaderiaSemana>();

            switch (id)
            {
                case 1:
                    _datos = datos.MercaderiaDia(semana, suc, tipo, false, false);                    
                    break;

                case 2: 
                    IList<frCarne> carne = datos.Carne(semana, suc, false);
                    foreach(var item in carne)
                    {
                        _datos.Add(new frMercaderiaSemana
                        {
                            ID = 2,
                            ID_Producto = 0,
                            Fecha = item.Fecha,
                            Tipo = 0,
                            Descripcion = item.Producto,
                            Items = 0,
                            Kilos = item.Kilos,
                            Costo = item.Costo,
                            Total = item.Total
                        });
                    }
                    break;
                case 3:
                    // Traslados entrada
                    IList<Traslados> traslados = datos.TrasladosDia(semana, suc, false);
                    foreach (var item in traslados)
                    {
                        _datos.Add(new frMercaderiaSemana
                        {
                            ID = 3,
                            ID_Producto = 0,
                            Fecha = item.Fecha,
                            Tipo = 0,
                            Descripcion = item.Descripcion,
                            Items = 0,
                            Kilos = item.Kilos,
                            Costo = item.Costo_Salida,
                            Total = item.Kilos * item.Costo_Salida
                        });
                    }
                    break;
                case 4:
                    // Gastos Empresa
                    IList<frGastosOficina> g = datos.GastosOficina(semana, suc, true);
                    foreach (var item in g)
                    {
                        _datos.Add(new frMercaderiaSemana
                        {
                            ID = 4,
                            ID_Producto = 0,
                            Fecha = item.Fecha,
                            Tipo = 0,
                            Descripcion = item.Descripcion,
                            Items = 0,
                            Kilos = 0,
                            Costo = 0,
                            Total = item.Importe
                        });
                    }
                    break;

                case 5:
                    // Entregas
                    IList<Entrega> entregas = datos.Entregas(semana, suc, 0);
                    foreach (var item in entregas)
                    {
                        _datos.Add(new frMercaderiaSemana
                        {
                            ID = 5,
                            ID_Producto = 0,
                            Fecha = item.Fecha,
                            Tipo = 0,
                            Descripcion = item.Descripcion,
                            Items = 0,
                            Kilos = 0,
                            Costo = 0,
                            Total = item.Importe
                        });
                    }
                    break;

                case 8:
                    // Traslados salida
                    IList<Traslados> trasladosSalida = datos.TrasladosDia(semana, suc, true);
                    foreach (var item in trasladosSalida)
                    {
                        _datos.Add(new frMercaderiaSemana
                        {
                            ID = 3,
                            ID_Producto = 0,
                            Fecha = item.Fecha,
                            Tipo = 0,
                            Descripcion = item.Descripcion,
                            Items = 0,
                            Kilos = item.Kilos,
                            Costo = item.Costo_Salida,
                            Total = item.Kilos * item.Costo_Salida
                        });
                    }
                    break;

            }

            return _datos;
        }
    }
}
