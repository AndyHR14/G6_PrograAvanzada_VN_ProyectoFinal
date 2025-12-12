using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            // 1. Buscar usuario
            var usuarioLogin = await _context.Login
                .FirstOrDefaultAsync(l => l.Correo == correo && l.Password == password);

            if (usuarioLogin != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuarioLogin.Correo),
            new Claim(ClaimTypes.Role, usuarioLogin.Rol),
            new Claim("IdLogin", usuarioLogin.IdLogin.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Mantener sesión abierta aunque cierre navegador (opcional)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Json(true);
            }

            return Json(false);
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

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
