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
        public DbSet<Cajas> Cajas { get; set; }
        public DbSet<Sinpe> Sinpes { get; set; }
        public DbSet<BitacoraEvento> BitacoraEventos { get; set; }

        // Metodo para configurar las relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurar la relacion entre Cajas y Sinpes
            modelBuilder.Entity<Cajas>()
                .HasMany(c => c.Sinpes)  // Una caja tiene muchos Sinpes
                .WithOne()  // Un Sinpe pertenece a una Caja
                .HasForeignKey(s => s.TelefonoDestinatario)  // La clave foranea en Sinpes
                .HasPrincipalKey(c => c.TelefonoSINPE)  // La clave en Cajas
                .IsRequired();  // La relacion es obligatoria
        }
    }
}