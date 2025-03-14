using Microsoft.EntityFrameworkCore;
using Planilla_WebApi.Modelos;
using System.Collections.Generic;

namespace Planilla_WebApi.Conexiones
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ventas> vw_Ventas { get; set; }
        public DbSet<VentasTipo> vw_VentasTipo { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Sucursales> Sucursales { get; set; }

    }

}
