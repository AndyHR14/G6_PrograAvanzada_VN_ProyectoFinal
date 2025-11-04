using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<Models.Comercio> Comercios { get; set; }
        //public DbSet<Models.Caja> Cajas { get; set; }
        //public DbSet<Models.Sinpe> Sinpes { get; set; }
        //public DbSet<Models.BitacoraEvento> BitacoraEventos { get; set; }
    }
}