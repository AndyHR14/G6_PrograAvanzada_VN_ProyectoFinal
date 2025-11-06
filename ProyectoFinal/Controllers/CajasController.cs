using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers
{
    public class CajasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
