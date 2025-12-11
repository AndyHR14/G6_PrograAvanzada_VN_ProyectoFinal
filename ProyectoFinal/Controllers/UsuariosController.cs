using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET Usuarios
        public async Task<IActionResult> Index()
        {
            var usuario = await _context.Usuario
                .Include(u => u.Comercio)
                .ToListAsync();
            return View(usuario);
        }

        //GET Crear Usuarios
        public async Task<IActionResult> Crear(int? idComercio)
        {
            var model = new Usuario();
            if (idComercio.HasValue)
            {
                model.IdComercio = idComercio.Value;
            }
            else
            {
              
                var comerciosCount = await _context.Comercios.CountAsync();
                if (comerciosCount == 1)
                {
                    var single = await _context.Comercios.FirstAsync();
                    model.IdComercio = single.IdComercio;
                }
            }

            if (model.IdComercio != 0)
            {
                var comercio = await _context.Comercios.FindAsync(model.IdComercio);
                if (comercio != null)
                    ViewBag.ComercioNombre = comercio.Nombre;
            }

            return View(model);
        }

        //POST Crear Usuarios
        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (usuario.IdNetUser == null || usuario.IdNetUser == Guid.Empty)
            {
                usuario.IdNetUser = Guid.NewGuid();
            }

            usuario.FechaDeRegistro = DateTime.Now;
            usuario.FechaDeModificacion = null;
            usuario.Estado = true;

            if (!ModelState.IsValid)
            {
                if (usuario.IdComercio != 0)
                {
                    var comercio = await _context.Comercios.FindAsync(usuario.IdComercio);
                    if (comercio != null)
                        ViewBag.ComercioNombre = comercio.Nombre;
                }
                return View(usuario);
            }

            //Validar  que IdComercio exista
            var comercioExists = await _context.Comercios.AnyAsync(c => c.IdComercio == usuario.IdComercio);
            if (!comercioExists)
            {
                ModelState.AddModelError("IdComercio", "El comercio seleccionado no es válido.");
                if (usuario.IdComercio != 0)
                {
                    var comercio = await _context.Comercios.FindAsync(usuario.IdComercio);
                    if (comercio != null)
                        ViewBag.ComercioNombre = comercio.Nombre;
                }
                return View(usuario);
            }

            //Validacion de identificacion unica
            var existe = await _context.Usuario.AnyAsync(u => u.Identificacion == usuario.Identificacion);
            if (existe)
            {
                TempData["Error"] = "Ya existe un usuario con esa identificación.";
                return RedirectToAction(nameof(Crear), new { idComercio = usuario.IdComercio });
            }

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        //GET Editar Usuario
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.IdComercio != 0)
            {
                var comercio = await _context.Comercios.FindAsync(usuario.IdComercio);
                if (comercio != null)
                    ViewBag.ComercioNombre = comercio.Nombre;
            }

            return View(usuario);
        }

        //POST Editar Usuario
        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                if (usuario.IdComercio != 0)
                {
                    var comercio = await _context.Comercios.FindAsync(usuario.IdComercio);
                    if (comercio != null)
                        ViewBag.ComercioNombre = comercio.Nombre;
                }
                return View(usuario);
            }

            usuario.FechaDeModificacion = DateTime.Now;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}