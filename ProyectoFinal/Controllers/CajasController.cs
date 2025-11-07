using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.AspNetCore.Mvc;
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
            _cajas.FechaDeRegistro = DateTime.Now;
            _cajas.FechaDeModificacion = DateTime.Now;

            if (ModelState.IsValid)
            {

                _context.Add(_cajas);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            return View(_cajas);
        }

        // GET: Cajas/VerSinpe/{telefonoSINPE}
        public async Task<IActionResult> VerSinpe(string id)
        {
            // Verificamos si el ID es nulo o vacio
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Obtenemos la caja con el telefono SINPE especificado
            var caja = await _context.Cajas.FirstOrDefaultAsync(c => c.TelefonoSINPE == id);
            if (caja == null)
            {
                return NotFound();
            }

            // Obtenemos todos los SINPES para esta caja ordenados por fecha más reciente
            var sinpes = await _context.Sinpes
                .Where(s => s.TelefonoDestinatario == id)
                .OrderByDescending(s => s.FechaDeRegistro)
                .ToListAsync();

            // Pasamos la informacion de la caja a la vista
            ViewBag.Caja = caja;

            // Devolvemos la vista con la lista de SINPES
            return View(sinpes);
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