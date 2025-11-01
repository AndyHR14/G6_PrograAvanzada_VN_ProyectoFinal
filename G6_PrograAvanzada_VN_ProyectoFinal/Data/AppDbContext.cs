using Microsoft.EntityFrameworkCore;
using G6_PrograAvanzada_VN_ProyectoFinal.Models;

namespace G6_PrograAvanzada_VN_ProyectoFinal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Comercio> Comercios { get; set; }

    }
}
