using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace ProyectoFinal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Comercio> Comercios { get; set; }
        //public DbSet<Models.Caja> Cajas { get; set; }
        //public DbSet<Models.Sinpe> Sinpes { get; set; }
        //public DbSet<Models.BitacoraEvento> BitacoraEventos { get; set; }
    }
}