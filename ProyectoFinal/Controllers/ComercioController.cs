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

                _context.Update(_comercio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
