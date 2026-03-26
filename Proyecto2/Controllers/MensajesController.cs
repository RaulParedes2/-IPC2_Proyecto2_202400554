using Microsoft.AspNetCore.Mvc;
using Proyecto2.Services;
using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Controllers
{
    public class MensajesController : Controller
    {
        private readonly GestorMensajes _gestorMensajes;
        private readonly GestorSistemas _gestorSistemas;
        private readonly Planificador _planificador;

        public MensajesController(
            GestorMensajes gestorMensajes,
            GestorSistemas gestorSistemas,
            Planificador planificador)
        {
            _gestorMensajes = gestorMensajes;
            _gestorSistemas = gestorSistemas;
            _planificador = planificador;
        }

        // Listar todos los mensajes
        public IActionResult Index()
        {
            ListaMensajes mensajes = _gestorMensajes.ObtenerTodos();
            return View(mensajes);
        }

        // Crear nuevo mensaje
        public IActionResult Crear()
        {
            ViewBag.Sistemas = _gestorSistemas.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult Crear(string nombre, string textoOriginal, string sistemaSeleccionado)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                ViewBag.Error = "El nombre del mensaje es requerido";
                ViewBag.Sistemas = _gestorSistemas.ObtenerTodos();
                return View();
            }

            bool resultado = _gestorMensajes.AgregarMensaje(nombre, textoOriginal ?? "");
            
            if (!resultado)
            {
                ViewBag.Error = $"Ya existe un mensaje con el nombre {nombre}";
                ViewBag.Sistemas = _gestorSistemas.ObtenerTodos();
                return View();
            }

            TempData["Mensaje"] = $"Mensaje '{nombre}' creado correctamente";
            return RedirectToAction("Index");
        }

        // Ver detalle de un mensaje
        public IActionResult Detalle(string nombre)
        {
            Mensaje? mensaje = _gestorMensajes.ObtenerPorNombre(nombre);
            if (mensaje == null)
            {
                return NotFound();
            }

            ViewBag.Sistemas = _gestorSistemas.ObtenerTodos();
            return View(mensaje);
        }

        // Agregar instrucción al mensaje
        [HttpPost]
        public IActionResult AgregarInstruccion(string nombreMensaje, string nombreDron, int altura, char letra)
        {
            bool resultado = _gestorMensajes.AgregarInstruccion(nombreMensaje, nombreDron, altura, letra);
            
            if (resultado)
            {
                TempData["Mensaje"] = $"Instrucción agregada: {nombreDron} a {altura}m = '{letra}'";
            }
            else
            {
                TempData["Error"] = "No se pudo agregar la instrucción";
            }
            
            return RedirectToAction("Detalle", new { nombre = nombreMensaje });
        }

        // Eliminar mensaje
        [HttpPost]
        public IActionResult Eliminar(string nombre)
        {
            _gestorMensajes.EliminarMensaje(nombre);
            TempData["Mensaje"] = $"Mensaje '{nombre}' eliminado";
            return RedirectToAction("Index");
        }

        // Enviar mensaje a un sistema de drones
        [HttpPost]
        public IActionResult EnviarMensaje(string nombreMensaje, string nombreSistema)
        {
            Mensaje? mensaje = _gestorMensajes.ObtenerPorNombre(nombreMensaje);
            SistemaDrones? sistema = _gestorSistemas.ObtenerPorNombre(nombreSistema);

            if (mensaje == null || sistema == null)
            {
                TempData["Error"] = "Mensaje o sistema no encontrado";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

            // Calcular el plan de vuelo óptimo
            PlanVuelo? plan = _planificador.CalcularPlan(mensaje, sistema);
            
            if (plan == null)
            {
                TempData["Error"] = "No se pudo calcular el plan de vuelo";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

            // Guardar el plan en sesión o TempData para mostrarlo
            TempData["PlanVuelo"] = plan.GenerarXML();
            TempData["TiempoOptimo"] = plan.TiempoOptimo;
            TempData["MensajeRecibido"] = plan.MensajeRecibido;
            TempData["NombreMensaje"] = mensaje.Nombre;
            TempData["NombreSistema"] = sistema.Nombre;

            return RedirectToAction("Resultado");
        }

        // Mostrar resultado del envío
        public IActionResult Resultado()
        {
            ViewBag.PlanVuelo = TempData["PlanVuelo"] as string;
            ViewBag.TiempoOptimo = TempData["TiempoOptimo"];
            ViewBag.MensajeRecibido = TempData["MensajeRecibido"];
            ViewBag.NombreMensaje = TempData["NombreMensaje"];
            ViewBag.NombreSistema = TempData["NombreSistema"];
            
            return View();
        }

        // Ver gráfico del plan de vuelo
        public IActionResult GraficoPlan(string nombre)
        {
            Mensaje? mensaje = _gestorMensajes.ObtenerPorNombre(nombre);
            if (mensaje == null)
            {
                return NotFound();
            }

            ViewBag.Mensaje = mensaje;
            ViewBag.Sistemas = _gestorSistemas.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult GenerarGrafico(string nombreMensaje, string nombreSistema)
        {
            Mensaje? mensaje = _gestorMensajes.ObtenerPorNombre(nombreMensaje);
            SistemaDrones? sistema = _gestorSistemas.ObtenerPorNombre(nombreSistema);

            if (mensaje == null || sistema == null)
            {
                TempData["Error"] = "Mensaje o sistema no encontrado";
                return RedirectToAction("GraficoPlan", new { nombre = nombreMensaje });
            }

            PlanVuelo? plan = _planificador.CalcularPlan(mensaje, sistema);
            
            if (plan == null)
            {
                TempData["Error"] = "No se pudo calcular el plan de vuelo";
                return RedirectToAction("GraficoPlan", new { nombre = nombreMensaje });
            }

            string dotCode = plan.ObtenerGrafico();
            ViewBag.DotCode = dotCode;
            ViewBag.NombreMensaje = mensaje.Nombre;
            ViewBag.NombreSistema = sistema.Nombre;
            ViewBag.TiempoOptimo = plan.TiempoOptimo;
            
            return View("GraficoResultado");
        }
    }
}