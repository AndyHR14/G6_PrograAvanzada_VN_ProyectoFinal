using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;
using ProyectoFinal.Data;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace PoryectoFinal.Controllers
{
    public class ReportesMensualesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesMensualesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reportes = await _context!.ReportesMensuales
                .OrderByDescending(r => r.FechaDelReporte)
                .ToListAsync();

            return View(reportes);
        }

        public async Task<IActionResult> GetNombreComercio(int id)
        {
            var comercio = await _context!.Comercios
                .Select(c => new { c.IdComercio, c.Nombre })
                .FirstOrDefaultAsync(c => c.IdComercio == id);

            if (comercio == null)
            {
                return NotFound();
            }
            return Json(new { nombre = comercio.Nombre });
        }

        [HttpPost]
        public async Task<IActionResult> GenerarReportes()
        {
            var fechaActual = DateTime.Today;
            var fechaActualCompleta = DateTime.Now;
            var fechaReporteKey = new DateTime(fechaActual.Year, fechaActual.Month, 1);
            var fechaFinMes = fechaReporteKey.AddMonths(1).AddDays(-1);

            var comerciosConComision = await _context!.Comercios
                .Join(_context.Configuracion,
                    comercio => comercio.IdComercio,
                    config => config.IdComercio,
                    (comercio, config) => new { comercio, config })
                .Where(u => u.config.Comision != 0)
                .Select(u => new
                {
                    IdComercio = u.comercio.IdComercio,
                    Comision = (decimal)u.config.Comision
                })
                .ToListAsync();

            foreach (var comercioData in comerciosConComision)
            {
                int cantidadDeCajas = await _context.Cajas
                    .CountAsync(c => c.IdComercio == comercioData.IdComercio);

                var sinpesDelMes = await _context.Sinpes
                    .Where(s => _context.Cajas.Any(caja =>
                                 caja.TelefonoSINPE == s.TelefonoDestinatario &&
                                 caja.IdComercio == comercioData.IdComercio)
                          )
                    .Where(s => s.FechaDeRegistro >= fechaReporteKey &&
                                s.FechaDeRegistro <= fechaFinMes)
                    .OrderBy(s => s.IdSinpe)
                    .Take(1000)
                    .ToListAsync();

                decimal montoTotalRecaudado = sinpesDelMes.Sum(s => s.Monto);
                int cantidadDeSINPES = sinpesDelMes.Count();

                decimal porcentaje = comercioData.Comision / 100.00m;
                decimal montoTotalComision = montoTotalRecaudado * porcentaje;

                var reporteExistente = await _context.ReportesMensuales
                    .FirstOrDefaultAsync(r => r.IdComercio == comercioData.IdComercio &&
                                              r.FechaDelReporte.Year == fechaReporteKey.Year &&
                                              r.FechaDelReporte.Month == fechaReporteKey.Month);

                if (reporteExistente != null)
                {
                    reporteExistente.CantidadDeCajas = cantidadDeCajas;
                    reporteExistente.MontoTotalRecaudado = montoTotalRecaudado;
                    reporteExistente.CantidadDeSINPES = cantidadDeSINPES;
                    reporteExistente.MontoTotalComision = montoTotalComision;
                    reporteExistente.FechaDelReporte = fechaActualCompleta;
                    _context.ReportesMensuales.Update(reporteExistente);
                }
                else
                {
                    var nuevoReporte = new ReporteMensual
                    {
                        IdComercio = comercioData.IdComercio,
                        CantidadDeCajas = cantidadDeCajas,
                        MontoTotalRecaudado = montoTotalRecaudado,
                        CantidadDeSINPES = cantidadDeSINPES,
                        MontoTotalComision = montoTotalComision,
                        FechaDelReporte = fechaActualCompleta
                    };
                    _context.ReportesMensuales.Add(nuevoReporte);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}