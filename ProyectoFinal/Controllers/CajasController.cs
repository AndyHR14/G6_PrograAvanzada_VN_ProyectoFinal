using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ProyectoFinal.Controllers
{
    public class CajasController : Controller
    {
        //Invocar o hacer una instancia
        private readonly AppDbContext _context;

        //Instanciarme
        public CajasController(AppDbContext context)
        {
            _context = context;
        }


        //Los metodos ahora son asyncronicos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cajas.ToListAsync());
        }


        public IActionResult Registrar()
        {

            return View();
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> Registrar(Cajas _cajas)
        {
            _cajas.FechaDeModificacion = DateTime.Now;

            if (ModelState.IsValid)
            {

                _context.Add(_cajas);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            return View(_cajas);
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
        public async Task<IActionResult> Editar(Cajas _cajas)
        {
            _cajas.FechaDeModificacion = DateTime.Now;

            if (ModelState.IsValid)
            {

                _context.Update(_cajas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            return View(_cajas);
        }

    }
}