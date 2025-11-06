using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace Biblioteca.Controllers
{
    public class CajasController : Controller
    {
        //Invoacion de instancia
        private readonly AppDbContext _context;

        //Instancia de contexto
        public CajasController(AppDbContext context)
        {
            _context = context;
        }

        //Metodos asincronico
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cajas.ToListAsync());
        }

        //Crear???
        public IActionResult Crear()
        {

            return View();
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> Crear(Cajas _Cajas)
        {

            if (ModelState.IsValid)
            {

                _context.Add(_Cajas);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            return View(_Cajas);
        }

        //Get /Editar/Id
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

        //Post
        [HttpPost]
        public async Task<IActionResult> Editar(Cajas _Cajas)
        {
            if (ModelState.IsValid)
            {

                _context.Update(_Cajas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(_Cajas);
        }
       
    }
}

