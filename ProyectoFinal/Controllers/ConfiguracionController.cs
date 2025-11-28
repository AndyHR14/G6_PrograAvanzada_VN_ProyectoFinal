using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinal.Controllers
{
    public class ConfiguracionController : Controller
    {
        private readonly AppDbContext _context;

        public ConfiguracionController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Configuracion.Include(c => c.Comercio).ToListAsync());
        }

        public async Task<IActionResult> Registrar()
        {
            var comerciosConConfig = await _context.Configuracion.Select(c => c.IdComercio).ToListAsync();
            var comerciosDisponibles = await _context.Comercios.Where(c => !comerciosConConfig.Contains(c.IdComercio)).ToListAsync();

            ViewBag.IdComercio = new SelectList(comerciosDisponibles, "IdComercio", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Configuracion _Configuracion)
        {
            if (await _context.Configuracion.AnyAsync(c => c.IdComercio == _Configuracion.IdComercio))
            {
                TempData["Error"] = "Este comercio ya tiene configuración.";
                return RedirectToAction(nameof(Registrar));
            }

            _Configuracion.FechaDeRegistro = DateTime.Now;
            _Configuracion.FechaDeModificacion = null;
            _Configuracion.Estado = true;

            _context.Add(_Configuracion);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Configuración registrada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int? Id)
        {
            if (Id == null) return NotFound();

            var config = await _context.Configuracion.Include(c => c.Comercio).FirstOrDefaultAsync(c => c.IdConfiguracion == Id);
            if (config == null) return NotFound();

            return View(config);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Configuracion _Configuracion)
        {
            var config = await _context.Configuracion.FindAsync(_Configuracion.IdConfiguracion);
            if (config == null) return NotFound();

            config.TipoConfiguracion = _Configuracion.TipoConfiguracion;
            config.Comision = _Configuracion.Comision;
            config.Estado = _Configuracion.Estado;
            config.FechaDeModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> VerDetalles(int? id)
        {
            if (id == null) return NotFound();

            var config = await _context.Configuracion.Include(c => c.Comercio).FirstOrDefaultAsync(m => m.IdConfiguracion == id);
            if (config == null) return NotFound();

            return View("VerDetalles", config);
        }
    }
}