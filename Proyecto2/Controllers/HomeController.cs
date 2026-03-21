using Microsoft.AspNetCore.Mvc;
using Proyecto2.Services;

namespace Proyecto2.Controllers
{
    public class HomeController : Controller
    {
        private readonly GestorDrones _gestorDrones;
        private readonly GestorSistemas _gestorSistemas;
        private readonly GestorMensajes _gestorMensajes;

        public HomeController(
            GestorDrones gestorDrones,
            GestorSistemas gestorSistemas,
            GestorMensajes gestorMensajes)
        {
            _gestorDrones = gestorDrones;
            _gestorSistemas = gestorSistemas;
            _gestorMensajes = gestorMensajes;
        }

        public IActionResult Index()
        {
            ViewBag.TotalDrones = _gestorDrones.ObtenerTodos().Count;
            ViewBag.TotalSistemas = _gestorSistemas.ObtenerTodos().Count;
            ViewBag.TotalMensajes = _gestorMensajes.ObtenerTodos().Count;
            return View();
        }

        public IActionResult Ayuda()
        {
            return View();
        }
    }
}