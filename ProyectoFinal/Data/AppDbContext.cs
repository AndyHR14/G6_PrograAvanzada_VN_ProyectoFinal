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
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<Sinpe> Sinpes { get; set; }
        public DbSet<BitacoraEvento> BitacoraEventos { get; set; }
    }
}