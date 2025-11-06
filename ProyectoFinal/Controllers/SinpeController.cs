using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using System;
using System.Linq;

namespace ProyectoFinal.Controllers
{
    public class SinpeController : Controller
    {
        private readonly AppDbContext _context;

        public SinpeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Sinpe
        public IActionResult Index()
        {
            return View();
        }

        // GET: Sinpe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sinpe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sinpe sinpe)
        {
            // Establecer valores por defecto
            sinpe.FechaDeRegistro = DateTime.Now;
            sinpe.Estado = false; // No sincronizado por defecto

            // Verificar si el teléfono destinatario existe y está activo
            var cajaExists = _context.Cajas
                .Any(c => c.TelefonoSINPE == sinpe.TelefonoDestinatario && c.Estado == true);

            if (!cajaExists)
            {
                ModelState.AddModelError("TelefonoDestinatario", "El número de teléfono destinatario no existe o está inactivo");
                return View(sinpe);
            }

            // Guardar el SINPE
            _context.Sinpes.Add(sinpe);
            _context.SaveChanges();

            // Registrar en bitácora
            var bitacora = new BitacoraEvento
            {
                TablaDeEvento = "SINPE",
                TipoDeEvento = "Registrar",
                FechaDeEvento = DateTime.Now,
                DescripcionDeEvento = "Se ha registrado un nuevo SINPE",
                StackTrace = "",
                DatosAnteriores = null,
                DatosPosteriores = System.Text.Json.JsonSerializer.Serialize(sinpe)
            };

            _context.BitacoraEventos.Add(bitacora);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}