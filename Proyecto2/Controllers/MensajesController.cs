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
        private readonly GraphvizService _graphvizService;


        public MensajesController(
            GestorMensajes gestorMensajes,
            GestorSistemas gestorSistemas,
            Planificador planificador, GraphvizService graphvizService)
        {
            _gestorMensajes = gestorMensajes;
            _gestorSistemas = gestorSistemas;
            _planificador = planificador;
            _graphvizService = graphvizService;
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
        public IActionResult Detalle(string nombre, string sistema = "")
        {
            Mensaje? mensaje = _gestorMensajes.ObtenerPorNombre(nombre);
            if (mensaje == null)
            {
                return NotFound();
            }

            ListaSistemasDrones sistemas = _gestorSistemas.ObtenerTodosOrdenados();
            ViewBag.Sistemas = sistemas;

            // Sistema seleccionado por defecto (el primero o el que viene en la URL)
            SistemaDrones? sistemaSeleccionado = null;
            if (!string.IsNullOrEmpty(sistema))
            {
                sistemaSeleccionado = _gestorSistemas.ObtenerPorNombre(sistema);
            }

            if (sistemaSeleccionado == null)
            {
                sistemaSeleccionado = sistemas.GetPrimero()?.Data;
            }

            ViewBag.SistemaSeleccionado = sistemaSeleccionado;
            ViewBag.SistemaSeleccionadoNombre = sistemaSeleccionado?.Nombre ?? "";

            return View(mensaje);
        }
        // Agregar instrucción al mensaje
        [HttpPost]
        public IActionResult AgregarInstruccion(string nombreMensaje, string nombreDron, int altura)
        {
            // Obtener el sistema seleccionado de la sesión o del parámetro
            // Por ahora, usamos el primer sistema disponible
            var sistemas = _gestorSistemas.ObtenerTodos();
            SistemaDrones? sistema = sistemas.GetPrimero()?.Data;

            if (sistema == null)
            {
                TempData["Error"] = "No hay sistemas de drones disponibles para obtener la letra";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

            // Obtener la letra de la codificación del sistema
            char letra = sistema.ObtenerLetra(nombreDron, altura);

            if (letra == '?')
            {
                TempData["Error"] = $"No se encontró codificación para {nombreDron} a altura {altura} en el sistema {sistema.Nombre}";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

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
            Console.WriteLine($"=== Enviando mensaje: {nombreMensaje} al sistema: {nombreSistema} ===");

            if (string.IsNullOrEmpty(nombreSistema))
            {
                TempData["Error"] = "Debe seleccionar un sistema de drones";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

            Mensaje? mensaje = _gestorMensajes.ObtenerPorNombre(nombreMensaje);
            SistemaDrones? sistema = _gestorSistemas.ObtenerPorNombre(nombreSistema);

            if (mensaje == null || sistema == null)
            {
                TempData["Error"] = "Mensaje o sistema no encontrado";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

            // VALIDAR: Verificar que todos los drones existan en el sistema
            NodoInstruccionEmision? instActual = mensaje.Instrucciones.GetPrimero();
            bool compatible = true;
            while (instActual != null)
            {
                if (!sistema.Drones.Existe(instActual.Data.NombreDron))
                {
                    compatible = false;
                    Console.WriteLine($"ERROR: Dron '{instActual.Data.NombreDron}' no existe en sistema '{sistema.Nombre}'");
                    break;
                }
                instActual = instActual.Siguiente;
            }

            if (!compatible)
            {
                TempData["Error"] = $"El mensaje '{mensaje.Nombre}' no es compatible con el sistema '{sistema.Nombre}'. Los drones no coinciden.";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

            PlanVuelo? plan = _planificador.CalcularPlan(mensaje, sistema);

            if (plan == null)
            {
                TempData["Error"] = "No se pudo calcular el plan de vuelo";
                return RedirectToAction("Detalle", new { nombre = nombreMensaje });
            }

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
            ViewBag.PlanVuelo = HttpContext.Session.GetString("PlanVuelo");
            ViewBag.TiempoOptimo = HttpContext.Session.GetInt32("TiempoOptimo");
            ViewBag.MensajeRecibido = HttpContext.Session.GetString("MensajeRecibido");
            ViewBag.NombreMensaje = HttpContext.Session.GetString("NombreMensaje");
            ViewBag.NombreSistema = HttpContext.Session.GetString("NombreSistema");

            return View();
        }
        // Ver gráfico del plan de vuelo
        public IActionResult GraficoPlan(string nombre)
        {
            Console.WriteLine($"=== GraficoPlan para mensaje: {nombre} ===");

            Mensaje? mensaje = _gestorMensajes.ObtenerPorNombre(nombre);
            if (mensaje == null)
            {
                TempData["Error"] = $"Mensaje '{nombre}' no encontrado";
                return RedirectToAction("Index");
            }

            ViewBag.Mensaje = mensaje;
            ViewBag.Sistemas = _gestorSistemas.ObtenerTodos();
            return View();
        }
        [HttpPost]
        public IActionResult GenerarGrafico(string nombreMensaje, string nombreSistema)
        {
            Console.WriteLine($"=== GenerarGrafico: {nombreMensaje} en {nombreSistema} ===");

            if (string.IsNullOrEmpty(nombreMensaje) || string.IsNullOrEmpty(nombreSistema))
            {
                TempData["Error"] = "Mensaje o sistema no especificado";
                return RedirectToAction("GraficoPlan", new { nombre = nombreMensaje });
            }

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

            string nombreArchivo = $"plan_{mensaje.Nombre}_{DateTime.Now.Ticks}";
            string? imagenPath = _graphvizService.GenerarTablaPlanVuelo(plan, nombreArchivo);

            ViewBag.ImagenPath = imagenPath;
            ViewBag.NombreMensaje = mensaje.Nombre;
            ViewBag.NombreSistema = sistema.Nombre;
            ViewBag.TiempoOptimo = plan.TiempoOptimo;
            ViewBag.MensajeRecibido = plan.MensajeRecibido;

            return View("GraficoResultado");
        }
    }
}