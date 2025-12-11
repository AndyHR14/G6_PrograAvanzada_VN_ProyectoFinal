using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace LoginApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Login");
        }

        [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(password))
            {
                return Json(false);
            }

            var login = await _context.Login
                .FirstOrDefaultAsync(l => l.Correo == correo && l.Password == password);

            if (login == null)
            {
                return Json(false);
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> ValidarLogin(string correo, string password)
        {
            var usuario = await _context.Login
                .FirstOrDefaultAsync(l => l.Correo == correo && l.Password == password);

            return Json(usuario != null);
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new Login());
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Login login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var yaExiste = await _context.Login
                .AnyAsync(l => l.Correo == login.Correo);

            if (yaExiste)
            {
                ModelState.AddModelError("Correo", "Ya existe un acceso registrado con este correo.");
                return View(login);
            }

            if (login.Rol == "Cajero")
            {
                var cajeroExiste = await _context.Usuario
                    .AnyAsync(u => u.CorreoElectronico == login.Correo);

                if (!cajeroExiste)
                {
                    ModelState.AddModelError("", "El cajero debe existir previamente en la tabla de Usuarios.");
                    return View(login);
                }
            }

            login.IdNetUser = Guid.NewGuid();

            _context.Login.Add(login);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Usuario registrado correctamente. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login", "Login");
        }
    }
}
