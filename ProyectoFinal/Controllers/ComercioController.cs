using ProyectoFinal.Data;
using ProyectoFinal.Models;
using ProyectoFinal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ProyectoFinal.Controllers
{
    public class ComercioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacoraService;

        public ComercioController(AppDbContext context, IBitacoraService bitacoraService)
        {
            _context = context;
            _bitacoraService = bitacoraService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Comercios.ToListAsync());
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Comercio _comercio)
        {
            try
            {
                _comercio.FechaDeRegistro = DateTime.UtcNow;
                _comercio.FechaDeModificacion = null;
                _comercio.Estado = true;

                _context.Add(_comercio);
                await _context.SaveChangesAsync();

                await _bitacoraService.RegistrarEventoAsync(
                    tabla: "Comercio",
                    tipoEvento: "Registrar",
                    descripcion: $"Se registró el comercio: {_comercio.Nombre} con ID: {_comercio.IdComercio}",
                    datosAnteriores: null,
                    datosPosteriores: _comercio
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(
                    tabla: "Comercio",
                    descripcion: "Error al crear comercio",
                    ex: ex
                );

                ModelState.AddModelError("", "Ocurrió un error al crear el comercio. Por favor, intente nuevamente.");
                return View(_comercio);
            }
        }

        //Get /Editar/Id
        public async Task<IActionResult> Editar(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            Comercio itemComercio = await _context.Comercios.FindAsync(Id);
            if (itemComercio == null)
            {
                return NotFound();
            }
            return View(itemComercio);
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> Editar(Comercio _comercio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var datosAnteriores = await _context.Comercios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.IdComercio == _comercio.IdComercio);

                    _comercio.FechaDeModificacion = DateTime.UtcNow;

                    _context.Update(_comercio);
                    await _context.SaveChangesAsync();

                    await _bitacoraService.RegistrarEventoAsync(
                        tabla: "Comercio",
                        tipoEvento: "Editar",
                        descripcion: $"Se editó el comercio: {_comercio.Nombre} con ID: {_comercio.IdComercio}",
                        datosAnteriores: datosAnteriores,
                        datosPosteriores: _comercio
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await _bitacoraService.RegistrarErrorAsync(
                        tabla: "Comercio",
                        descripcion: "Error al editar comercio",
                        ex: ex
                    );

                    ModelState.AddModelError("", "Ocurrió un error al editar el comercio. Por favor, intente nuevamente.");
                }
            }

            return View(_comercio);
        }

        public async Task<IActionResult> VerDetalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comercio comercio = await _context.Comercios.FirstOrDefaultAsync(m => m.IdComercio == id);

            if (comercio == null)
            {
                return NotFound();
            }

            return View("VerDetalles", comercio);
        }
    }
}