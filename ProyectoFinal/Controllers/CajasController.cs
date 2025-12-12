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
    public class CajasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacoraService;

        public CajasController(AppDbContext context, IBitacoraService bitacoraService)
        {
            _context = context;
            _bitacoraService = bitacoraService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Cajas.ToListAsync());
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Cajas _cajas)
        {
            _cajas.FechaDeRegistro = DateTime.Now;
            _cajas.FechaDeModificacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    var existeNombre = await _context.Cajas
                        .AnyAsync(c => c.Nombre != null && c.Nombre.Trim().ToLower() == (_cajas.Nombre ?? "").Trim().ToLower());

                    if (existeNombre)
                    {
                        ModelState.AddModelError("Nombre", "Ya existe una caja con ese nombre.");
                        return View(_cajas);
                    }

                    _context.Add(_cajas);
                    await _context.SaveChangesAsync();

                    await _bitacoraService.RegistrarEventoAsync(
                        tabla: "Cajas",
                        tipoEvento: "Registrar",
                        descripcion: $"Se registró la caja: {_cajas.Nombre} con teléfono SINPE: {_cajas.TelefonoSINPE}",
                        datosAnteriores: null,
                        datosPosteriores: _cajas
                    );

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    await _bitacoraService.RegistrarErrorAsync(
                        tabla: "Cajas",
                        descripcion: "Error al registrar caja",
                        ex: ex
                    );

                    ModelState.AddModelError("", "Ocurrió un error al registrar la caja. Por favor, intente nuevamente.");
                }
            }

            return View(_cajas);
        }

        public async Task<IActionResult> VerSinpe(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var caja = await _context.Cajas.FirstOrDefaultAsync(c => c.TelefonoSINPE == id);
            if (caja == null)
            {
                return NotFound();
            }

            var sinpes = await _context.Sinpes
                .Where(s => s.TelefonoDestinatario == id)
                .OrderByDescending(s => s.FechaDeRegistro)
                .ToListAsync();

            ViewBag.Caja = caja;

            return View(sinpes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SincronizarSinpe(int id)
        {
            try
            {
                var sinpe = await _context.Sinpes.FindAsync(id);
                if (sinpe == null)
                {
                    return NotFound();
                }

                if (!sinpe.Estado)
                {
                    var datosAnteriores = new 
                    { 
                        IdSinpe = sinpe.IdSinpe, 
                        Estado = sinpe.Estado,
                        TelefonoOrigen = sinpe.TelefonoOrigen,
                        TelefonoDestinatario = sinpe.TelefonoDestinatario,
                        Monto = sinpe.Monto
                    };

                    sinpe.Estado = true;

                    var datosPosteriores = new 
                    { 
                        IdSinpe = sinpe.IdSinpe, 
                        Estado = sinpe.Estado,
                        TelefonoOrigen = sinpe.TelefonoOrigen,
                        TelefonoDestinatario = sinpe.TelefonoDestinatario,
                        Monto = sinpe.Monto
                    };

                    _context.Sinpes.Update(sinpe);
                    await _context.SaveChangesAsync();

                    await _bitacoraService.RegistrarEventoAsync(
                        tabla: "SINPE",
                        tipoEvento: "Sincronizar",
                        descripcion: $"Se sincronizó SINPE ID: {sinpe.IdSinpe} - Monto: {sinpe.Monto:C}",
                        datosAnteriores: datosAnteriores,
                        datosPosteriores: datosPosteriores
                    );
                }

                return RedirectToAction("VerSinpe", new { id = sinpe.TelefonoDestinatario });
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(
                    tabla: "SINPE",
                    descripcion: "Error al sincronizar SINPE",
                    ex: ex
                );

                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cajas itemCajas = await _context.Cajas.FindAsync(id);
            if (itemCajas == null)
            {
                return NotFound();
            }
            return View(itemCajas);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Cajas _cajas)
        {
            _cajas.FechaDeModificacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    var datosAnteriores = await _context.Cajas
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.IdCaja == _cajas.IdCaja);

                    _context.Update(_cajas);
                    await _context.SaveChangesAsync();

                    await _bitacoraService.RegistrarEventoAsync(
                        tabla: "Cajas",
                        tipoEvento: "Editar",
                        descripcion: $"Se editó la caja: {_cajas.Nombre} con ID: {_cajas.IdCaja}",
                        datosAnteriores: datosAnteriores,
                        datosPosteriores: _cajas
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await _bitacoraService.RegistrarErrorAsync(
                        tabla: "Cajas",
                        descripcion: "Error al editar caja",
                        ex: ex
                    );

                    ModelState.AddModelError("", "Ocurrió un error al editar la caja. Por favor, intente nuevamente.");
                }
            }

            return View(_cajas);
        }
    }
}