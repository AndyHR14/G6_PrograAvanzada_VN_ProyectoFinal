using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoFinal.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class BitacoraController : Controller
    {
        private readonly AppDbContext _context;

        public BitacoraController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bitacora
        public async Task<IActionResult> Index()
        {
            // Obtener todos los eventos ordenados por fecha descendente (más recientes primero)
            var eventos = await _context.BitacoraEventos
                .OrderByDescending(b => b.FechaDeEvento)
                .ToListAsync();

            return View(eventos);
        }

        // GET: Bitacora/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.BitacoraEventos
                .FirstOrDefaultAsync(m => m.IdEvento == id);

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }
    }
}