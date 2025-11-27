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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Cajas _cajas)
        {
            _cajas.FechaDeRegistro = DateTime.Now;
            _cajas.FechaDeModificacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                // Validar que no exista otra caja con el mismo nombre (único)
                var existeNombre = await _context.Cajas
                    .AnyAsync(c => c.Nombre != null && c.Nombre.Trim().ToLower() == (_cajas.Nombre ?? "").Trim().ToLower());

                if (existeNombre)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una caja con ese nombre.");
                    return View(_cajas);
                }

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

        //Acción para sincronizar un SINPE (marca como sincronizado)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SincronizarSinpe(int id)
        {
            var sinpe = await _context.Sinpes.FindAsync(id);
            if (sinpe == null)
            {
                return NotFound();
            }

            if (!sinpe.Estado)
            {
                // Guardar estado anterior
                var estadoAnterior = sinpe.Estado;

                // Cambiar estado a sincronizado
                sinpe.Estado = true;

                // Registrar en bitácora el cambio de estado
                var bitacora = new BitacoraEvento
                {
                    TablaDeEvento = "SINPE",
                    TipoDeEvento = "Sincronizar",
                    FechaDeEvento = DateTime.Now,
                    DescripcionDeEvento = $"Se sincronizó SINPE Id={sinpe.IdSinpe}",
                    StackTrace = "",
                    DatosAnteriores = System.Text.Json.JsonSerializer.Serialize(new { IdSinpe = sinpe.IdSinpe, Estado = estadoAnterior }),
                    DatosPosteriores = System.Text.Json.JsonSerializer.Serialize(new { IdSinpe = sinpe.IdSinpe, Estado = sinpe.Estado })
                };

                _context.BitacoraEventos.Add(bitacora);
                _context.Sinpes.Update(sinpe);

                await _context.SaveChangesAsync();
            }

            // Redirigimos de vuelta a la vista de SINPEs de la caja correspondiente
            return RedirectToAction("VerSinpe", new { id = sinpe.TelefonoDestinatario });
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