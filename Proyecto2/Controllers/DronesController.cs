using Microsoft.AspNetCore.Mvc;
using IPC2_Proyecto2.Services;
using IPC2_Proyecto2.Models;
using IPC2_Proyecto2.TDAs;

namespace IPC2_Proyecto2.Controllers
{
    public class DronesController : Controller
    {
        private readonly GestorDrones _gestorDrones;

        public DronesController(GestorDrones gestorDrones)
        {
            _gestorDrones = gestorDrones;
        }

        public IActionResult Index()
        {
            ListaDrones drones = _gestorDrones.ObtenerTodos();
            return View(drones);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(string nombre, int alturaInicial = 1)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                ViewBag.Error = "El nombre del dron es requerido";
                return View();
            }

            bool resultado = _gestorDrones.AgregarDron(nombre, alturaInicial);

            if (!resultado)
            {
                ViewBag.Error = $"Ya existe un dron con el nombre {nombre}";
                return View();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Eliminar(string nombre)
        {
            _gestorDrones.EliminarDron(nombre);
            return RedirectToAction("Index");
        }
    }
}
