using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using ProyectoFinal.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoFinal.Controllers
{
    public class SinpeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacoraService;

        public SinpeController(AppDbContext context, IBitacoraService bitacoraService)
        {
            _context = context;
            _bitacoraService = bitacoraService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sinpe sinpe)
        {
            try
            {
                sinpe.FechaDeRegistro = DateTime.Now;
                sinpe.Estado = false;

                var cajaExists = _context.Cajas
                    .Any(c => c.TelefonoSINPE == sinpe.TelefonoDestinatario && c.Estado == true);

                if (!cajaExists)
                {
                    ModelState.AddModelError("TelefonoDestinatario", "El número de teléfono destinatario no existe o está inactivo");
                    return View(sinpe);
                }

                _context.Sinpes.Add(sinpe);
                await _context.SaveChangesAsync();

                await _bitacoraService.RegistrarEventoAsync(
                    tabla: "SINPE",
                    tipoEvento: "Registrar",
                    descripcion: $"Se registró un nuevo SINPE - Origen: {sinpe.TelefonoOrigen} - Destino: {sinpe.TelefonoDestinatario} - Monto: {sinpe.Monto:C}",
                    datosAnteriores: null,
                    datosPosteriores: sinpe
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(
                    tabla: "SINPE",
                    descripcion: "Error al registrar SINPE",
                    ex: ex
                );

                ModelState.AddModelError("", "Ocurrió un error al registrar el SINPE. Por favor, intente nuevamente.");
                return View(sinpe);
            }
        }
    }
}