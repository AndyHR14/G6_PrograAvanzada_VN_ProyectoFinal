using ProyectoFinal.Data;
using ProyectoFinal.Models;
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

        public ComercioController(AppDbContext context)
        {
            _context = context;
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

            _comercio.FechaDeRegistro = DateTime.UtcNow;
            _comercio.FechaDeModificacion = null;
            _comercio.Estado = true;

            _context.Add(_comercio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
