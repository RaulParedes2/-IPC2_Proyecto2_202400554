using Microsoft.AspNetCore.Mvc;
using Proyecto2.TDAs;
using Proyecto2.Models;
using Proyecto2.Services;

namespace Proyecto2.Controllers
{
    public class HomeController : Controller
    {
        private readonly GestorDrones _gestorDrones;
        private readonly GestorSistemas _gestorSistemas;
        private readonly GestorMensajes _gestorMensajes;
        private readonly LectorXML _lectorXML;
        private readonly GeneradorXML _generadorXML;

        public HomeController(
            GestorDrones gestorDrones,
            GestorSistemas gestorSistemas,
            GestorMensajes gestorMensajes,
            LectorXML lectorXML,
            GeneradorXML generadorXML)
        {
            _gestorDrones = gestorDrones;
            _gestorSistemas = gestorSistemas;
            _gestorMensajes = gestorMensajes;
            _lectorXML = lectorXML;
            _generadorXML = generadorXML;
        }

        public IActionResult Index()
        {
            ViewBag.TotalDrones = _gestorDrones.CantidadDrones();
            ViewBag.TotalSistemas = _gestorSistemas.CantidadSistemas();
            ViewBag.TotalMensajes = _gestorMensajes.CantidadMensajes();
            ViewBag.SistemasDisponibles = _gestorSistemas.ObtenerTodos(); // Agregar esta línea
            return View();
        }

        public IActionResult Ayuda()
        {
            return View();
        }

        [HttpPost("CargarXML")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CargarXML(IFormFile archivoXML)
        {
            Console.WriteLine("=== MÉTODO CARGARXML EJECUTADO ===");

            if (archivoXML == null || archivoXML.Length == 0)
            {
                TempData["Error"] = "No se seleccionó ningún archivo";
                Console.WriteLine("Error: No se seleccionó archivo");
                return RedirectToAction("Index");
            }

            try
            {
                Console.WriteLine($"Archivo recibido: {archivoXML.FileName}, Tamaño: {archivoXML.Length} bytes");

                if (!archivoXML.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "El archivo debe tener extensión .xml";
                    Console.WriteLine("Error: Extensión no válida");
                    return RedirectToAction("Index");
                }

                string tempFilePath = Path.Combine(Path.GetTempPath(), $"upload_{Guid.NewGuid()}_{archivoXML.FileName}");
                Console.WriteLine($"Archivo temporal: {tempFilePath}");

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await archivoXML.CopyToAsync(stream);
                }

                string contenido = System.IO.File.ReadAllText(tempFilePath);
                Console.WriteLine($"Contenido XML (primeros 500 chars): {contenido.Substring(0, Math.Min(500, contenido.Length))}");

                bool resultado = _lectorXML.CargarConfiguracion(tempFilePath);
                Console.WriteLine($"Resultado de carga: {resultado}");

                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }

                if (resultado)
                {
                    TempData["Mensaje"] = $"Archivo XML cargado correctamente. " +
                                           $"Drones: {_gestorDrones.CantidadDrones()}, " +
                                           $"Sistemas: {_gestorSistemas.CantidadSistemas()}, " +
                                           $"Mensajes: {_gestorMensajes.CantidadMensajes()}";
                    Console.WriteLine("Carga exitosa");
                }
                else
                {
                    TempData["Error"] = "Error al procesar el archivo XML. Verifique el formato.";
                    Console.WriteLine("Error: Fallo al procesar XML");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar archivo: {ex.Message}";
                Console.WriteLine($"Excepción: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return RedirectToAction("Index");
        }

        [HttpPost("GenerarXMLSalida")]
        [ValidateAntiForgeryToken]
        public IActionResult GenerarXMLSalida(string nombreSistema)
        {
            Console.WriteLine("=== MÉTODO GENERARXMLSALIDA EJECUTADO ===");

            if (string.IsNullOrEmpty(nombreSistema))
            {
                var sistemas = _gestorSistemas.ObtenerTodos();
                if (sistemas.GetPrimero() != null)
                {
                    nombreSistema = sistemas.GetPrimero()!.Data.Nombre;
                }
                else
                {
                    TempData["Error"] = "No hay sistemas de drones disponibles";
                    return RedirectToAction("Index");
                }
            }

            try
            {
                string xml = _generadorXML.GenerarXMLSalida(nombreSistema, _gestorSistemas);
                string nombreArchivo = $"respuesta_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                _generadorXML.GuardarXMLSalida(nombreArchivo, xml);

                TempData["Mensaje"] = $"XML de salida generado: /xml/{nombreArchivo}";
                Console.WriteLine($"XML generado: {nombreArchivo}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al generar XML: {ex.Message}";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerarXMLSalidaDescargarForm(string nombreSistema)
        {
            try
            {
                Console.WriteLine($"=== Generando XML para descargar (form), sistema: {nombreSistema} ===");

                if (string.IsNullOrEmpty(nombreSistema))
                {
                    TempData["Error"] = "Debe seleccionar un sistema de drones";
                    return RedirectToAction("Index");
                }

                SistemaDrones? sistema = _gestorSistemas.ObtenerPorNombre(nombreSistema);
                if (sistema == null)
                {
                    TempData["Error"] = $"Sistema '{nombreSistema}' no encontrado";
                    return RedirectToAction("Index");
                }

                // Generar el XML
                string xml = _generadorXML.GenerarXMLSalida(nombreSistema, _gestorSistemas);

                // Generar nombre de archivo con fecha
                string nombreArchivo = $"respuesta_{DateTime.Now:yyyyMMdd_HHmmss}.xml";

                // Convertir a bytes
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xml);

                // Retornar como archivo para descargar
                return File(bytes, "application/xml", nombreArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar XML: {ex.Message}");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}