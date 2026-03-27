using Microsoft.AspNetCore.Mvc;
using Proyecto2.Services;
using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Controllers
{
    public class SistemasController : Controller
    {
        private readonly GestorSistemas _gestorSistemas;
        private readonly GestorDrones _gestorDrones;
        private readonly GraphvizService _graphvizService;

        public SistemasController(
            GestorSistemas gestorSistemas, 
            GestorDrones gestorDrones,
            GraphvizService graphvizService)
        {
            _gestorSistemas = gestorSistemas;
            _gestorDrones = gestorDrones;
            _graphvizService = graphvizService;
        }

        public IActionResult Index()
        {
            ListaSistemasDrones sistemas = _gestorSistemas.ObtenerTodos();
            return View(sistemas);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                ViewBag.Error = "El nombre del sistema es requerido";
                return View();
            }

            bool resultado = _gestorSistemas.AgregarSistema(nombre);
            
            if (!resultado)
            {
                ViewBag.Error = $"Ya existe un sistema con el nombre {nombre}";
                return View();
            }

            TempData["Mensaje"] = $"Sistema '{nombre}' creado correctamente";
            return RedirectToAction("Index");
        }

        public IActionResult Detalle(string nombre)
        {
            SistemaDrones? sistema = _gestorSistemas.ObtenerPorNombre(nombre);
            if (sistema == null)
            {
                return NotFound();
            }

            ViewBag.DronesDisponibles = _gestorDrones.ObtenerTodos();
            return View(sistema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarDron(string nombreSistema, string nombreDron)
        {
            bool resultado = _gestorSistemas.AgregarDronASistema(nombreSistema, nombreDron);
            
            if (resultado)
            {
                TempData["Mensaje"] = $"Dron '{nombreDron}' agregado al sistema";
            }
            else
            {
                TempData["Error"] = "No se pudo agregar el dron al sistema";
            }
            
            return RedirectToAction("Detalle", new { nombre = nombreSistema });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfigurarCodificacion(string nombreSistema, string nombreDron, int altura, char letra)
        {
            bool resultado = _gestorSistemas.ConfigurarCodificacion(nombreSistema, nombreDron, altura, letra);
            
            if (resultado)
            {
                TempData["Mensaje"] = $"Codificación configurada: {nombreDron} a {altura}m = '{letra}'";
            }
            else
            {
                TempData["Error"] = "No se pudo configurar la codificación";
            }
            
            return RedirectToAction("Detalle", new { nombre = nombreSistema });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(string nombre)
        {
            _gestorSistemas.Eliminar(nombre);
            TempData["Mensaje"] = $"Sistema '{nombre}' eliminado";
            return RedirectToAction("Index");
        }

        public IActionResult GraficoPNG(string nombre)
        {
            SistemaDrones? sistema = _gestorSistemas.ObtenerPorNombre(nombre);
            if (sistema == null)
            {
                return NotFound();
            }

            string dot = GenerarDotParaSistema(sistema);
            string nombreArchivo = $"sistema_{nombre}_{DateTime.Now.Ticks}";
            string? imagenPath = _graphvizService.GenerarImagen(dot, nombreArchivo);

            ViewBag.DotCode = dot;
            ViewBag.NombreSistema = nombre;

            if (imagenPath == null)
            {
                TempData["Error"] = "No se pudo generar la imagen. Verifica que Graphviz esté instalado.";
            }
            else
            {
                ViewBag.ImagenPath = imagenPath;
            }
            
            return View();
        }

        private string GenerarDotParaSistema(SistemaDrones sistema)
        {
            string dot = "digraph SistemaDrones {\n";
            dot += "  rankdir=TB;\n";
            dot += "  node [shape=box, style=filled, fillcolor=lightblue];\n";
            dot += "  fontname=\"Arial\";\n";
            dot += "  fontsize=12;\n\n";
            
            dot += $"  titulo [label=\"Sistema: {sistema.Nombre}\", shape=plaintext, fontsize=16];\n\n";
            dot += $"  sistema [label=\"Sistema\\n{sistema.Nombre}\", fillcolor=lightgreen];\n\n";
            
            dot += "  subgraph cluster_drones {\n";
            dot += "    label=\"Drones\";\n";
            dot += "    style=filled;\n";
            dot += "    fillcolor=lightyellow;\n\n";
            
            NodoDron? actualDron = sistema.Drones.GetPrimero();
            while (actualDron != null)
            {
                dot += $"    dron_{actualDron.Data.Nombre} [label=\"{actualDron.Data.Nombre}\\nAltura: {actualDron.Data.AlturaActual}m\"];\n";
                dot += $"    sistema -> dron_{actualDron.Data.Nombre};\n";
                actualDron = actualDron.Siguiente;
            }
            
            dot += "  }\n\n";
            
            dot += "  subgraph cluster_codificacion {\n";
            dot += "    label=\"Tabla de Codificación\";\n";
            dot += "    style=filled;\n";
            dot += "    fillcolor=lightgray;\n\n";
            
            TablaCodificacion.CeldaCodificacion? actualCelda = sistema.Codificacion.GetPrimero();
            while (actualCelda != null)
            {
                dot += $"    cod_{actualCelda.NombreDron}_{actualCelda.Altura} [label=\"{actualCelda.NombreDron} @ {actualCelda.Altura}m = '{actualCelda.Letra}'\", shape=ellipse];\n";
                actualCelda = actualCelda.Siguiente;
            }
            
            dot += "  }\n";
            dot += "}\n";
            return dot;
        }
    }
}