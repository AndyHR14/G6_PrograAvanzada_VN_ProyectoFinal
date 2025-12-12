using ProyectoFinal.Data;
using ProyectoFinal.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProyectoFinal.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly AppDbContext _context;

        public BitacoraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarEventoAsync(string tabla, string tipoEvento, string descripcion, 
            object datosAnteriores = null, object datosPosteriores = null)
        {
            try
            {
                var bitacora = new BitacoraEvento
                {
                    TablaDeEvento = tabla,
                    TipoDeEvento = tipoEvento,
                    FechaDeEvento = DateTime.Now,
                    DescripcionDeEvento = descripcion,
                    StackTrace = null,
                    DatosAnteriores = datosAnteriores != null 
                        ? JsonSerializer.Serialize(datosAnteriores, new JsonSerializerOptions { WriteIndented = true }) 
                        : null,
                    DatosPosteriores = datosPosteriores != null 
                        ? JsonSerializer.Serialize(datosPosteriores, new JsonSerializerOptions { WriteIndented = true }) 
                        : null
                };

                _context.BitacoraEventos.Add(bitacora);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log del error sin crear un loop infinito
                Console.WriteLine($"Error al registrar en bitácora: {ex.Message}");
            }
        }

        public async Task RegistrarErrorAsync(string tabla, string descripcion, Exception ex)
        {
            try
            {
                var bitacora = new BitacoraEvento
                {
                    TablaDeEvento = tabla,
                    TipoDeEvento = "Error",
                    FechaDeEvento = DateTime.Now,
                    DescripcionDeEvento = descripcion,
                    StackTrace = ex.StackTrace ?? ex.Message,
                    DatosAnteriores = null,
                    DatosPosteriores = ex.Message
                };

                _context.BitacoraEventos.Add(bitacora);
                await _context.SaveChangesAsync();
            }
            catch (Exception error)
            {
                Console.WriteLine($"Error crítico al registrar error en bitácora: {error.Message}");
            }
        }
    }
}