using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Proyecto2.Controllers;
using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class GraphvizService
    {
        private readonly string _graphvizPath;
        private readonly string _wwwrootPath;

        public GraphvizService(IWebHostEnvironment webHostEnvironment)
        {
            _wwwrootPath = webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _graphvizPath = FindGraphvizPath();
        }

        private string FindGraphvizPath()
        {
            // Rutas comunes de instalación de Graphviz en Windows
            string[] possiblePaths = new string[]
            {
        @"C:\Program Files\Graphviz\bin\dot.exe",
        @"C:\Program Files (x86)\Graphviz\bin\dot.exe",
        @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Graphviz\bin\dot.exe",
        @"C:\Users\" + Environment.UserName + @"\AppData\Local\Graphviz\bin\dot.exe"
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    Console.WriteLine($"Graphviz encontrado en: {path}");
                    return path;
                }
            }

            // Buscar en variables de entorno PATH
            string? pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (pathEnv != null)
            {
                foreach (string dir in pathEnv.Split(';'))
                {
                    string fullPath = Path.Combine(dir.Trim(), "dot.exe");
                    if (File.Exists(fullPath))
                    {
                        Console.WriteLine($"Graphviz encontrado en PATH: {fullPath}");
                        return fullPath;
                    }
                }
            }

            Console.WriteLine("Graphviz NO encontrado en el sistema");
            return "dot"; // Intentar usar el comando directo
        }
        public string? GenerarImagen(string dotCode, string nombreArchivo)
        {
            try
            {
                // Crear carpeta temporal si no existe
                string tempFolder = Path.Combine(_wwwrootPath, "temp");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                // Guardar el código DOT en un archivo temporal
                string dotFilePath = Path.Combine(tempFolder, $"{nombreArchivo}.dot");
                string pngFilePath = Path.Combine(tempFolder, $"{nombreArchivo}.png");
                string relativePath = $"/temp/{nombreArchivo}.png";

                File.WriteAllText(dotFilePath, dotCode);

                // Ejecutar Graphviz para generar la imagen
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _graphvizPath,
                    Arguments = $"-Tpng \"{dotFilePath}\" -o \"{pngFilePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                using (Process? process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        return null;
                    }

                    process.WaitForExit();
                    string? error = process.StandardError.ReadToEnd();

                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine($"Error al generar imagen: {error}");
                        return null;
                    }
                }

                // Eliminar el archivo .dot temporal
                if (File.Exists(dotFilePath))
                {
                    File.Delete(dotFilePath);
                }

                return relativePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GraphvizService: {ex.Message}");
                return null;
            }
        }

        public void LimpiarArchivosTemporales()
        {
            string tempFolder = Path.Combine(_wwwrootPath, "temp");
            if (Directory.Exists(tempFolder))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(tempFolder))
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < DateTime.Now.AddHours(-1))
                        {
                            File.Delete(file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error limpiando archivos temporales: {ex.Message}");
                }
            }
        }

        public string? GenerarTablaCodificacion(SistemaDrones sistema, string nombreArchivo)
        {
            try
            {
                string tempFolder = Path.Combine(_wwwrootPath, "temp");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                string dotFilePath = Path.Combine(tempFolder, $"{nombreArchivo}.dot");
                string pngFilePath = Path.Combine(tempFolder, $"{nombreArchivo}.png");
                string relativePath = $"/temp/{nombreArchivo}.png";

                // Generar codigo DOT para tabla HTML
                string dotCode = GenerarDotTabla(sistema);
                File.WriteAllText(dotFilePath, dotCode);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _graphvizPath,
                    Arguments = $"-Tpng \"{dotFilePath}\" -o \"{pngFilePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                using (Process? process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        return null;
                    }

                    process.WaitForExit();
                    string error = process.StandardError.ReadToEnd();

                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine($"Error al generar imagen: {error}");
                        return null;
                    }
                }

                if (File.Exists(dotFilePath))
                {
                    File.Delete(dotFilePath);
                }

                return relativePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GraphvizService: {ex.Message}");
                return null;
            }
        }

        private string GenerarDotTabla(SistemaDrones sistema)
        {
            // Obtener drones ordenados alfabéticamente
            ListaDrones dronesOrdenados = sistema.Drones.ObtenerOrdenadosAlfabeticamente();

            // Encontrar la altura máxima que tiene codificación
            int alturaMaxima = 0;
            var celda = sistema.Codificacion.GetPrimero();
            while (celda != null)
            {
                if (celda.Altura > alturaMaxima)
                {
                    alturaMaxima = celda.Altura;
                }
                celda = celda.Siguiente;
            }

            if (alturaMaxima == 0)
            {
                alturaMaxima = 10;
            }

            string dot = "digraph G {\n";
            dot += "  node [shape=plaintext]\n";
            dot += "  fontname=\"Arial\"\n";
            dot += "  fontsize=12\n\n";

            dot += "  tablerank [label=<\n";
            dot += "    <table border=\"1\" cellborder=\"1\" cellspacing=\"0\" cellpadding=\"8\">\n";
            dot += "        <tr>\n";
            dot += "        <td bgcolor=\"lightblue\"><b>Altura (mts)</b></td>\n";

            // Agregar columnas de drones en orden alfabético
            NodoDron? dronActual = dronesOrdenados.GetPrimero();
            while (dronActual != null)
            {
                dot += $"        <td bgcolor=\"lightblue\"><b>{dronActual.Data.Nombre}</b></td>\n";
                dronActual = dronActual.Siguiente;
            }
            dot += "        </tr>\n";

            // Agregar filas de alturas
            for (int altura = 1; altura <= alturaMaxima; altura++)
            {
                dot += "        <tr>\n";
                dot += $"         <td bgcolor=\"lightgray\"><b>{altura}</b></td>\n";

                dronActual = dronesOrdenados.GetPrimero();
                while (dronActual != null)
                {
                    char letra = sistema.ObtenerLetra(dronActual.Data.Nombre, altura);
                    string mostrarLetra = "";
                    string bgColor = "";

                    if (letra == ' ')
                    {
                        mostrarLetra = " ";
                        bgColor = "bgcolor=\"lightyellow\"";
                    }
                    else if (letra == '?')
                    {
                        mostrarLetra = "?";
                        bgColor = "bgcolor=\"lightcoral\"";
                    }
                    else
                    {
                        mostrarLetra = letra.ToString();
                        bgColor = "bgcolor=\"lightgreen\"";
                    }

                    dot += $"        <td {bgColor}>{mostrarLetra}</td>\n";
                    dronActual = dronActual.Siguiente;
                }
                dot += "        </tr>\n";
            }

            dot += "      </table>\n";
            dot += "  >];\n";
            dot += "}\n";

            return dot;
        }

        public string? GenerarTablaPlanVuelo(PlanVuelo plan, string nombreArchivo)
        {
            try
            {
                string tempFolder = Path.Combine(_wwwrootPath, "temp");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                string dotFilePath = Path.Combine(tempFolder, $"{nombreArchivo}.dot");
                string pngFilePath = Path.Combine(tempFolder, $"{nombreArchivo}.png");
                string relativePath = $"/temp/{nombreArchivo}.png";

                string dotCode = GenerarDotTablaPlanVuelo(plan);
                File.WriteAllText(dotFilePath, dotCode);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _graphvizPath,
                    Arguments = $"-Tpng \"{dotFilePath}\" -o \"{pngFilePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                using (Process? process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        return null;
                    }

                    process.WaitForExit();
                    string error = process.StandardError.ReadToEnd();

                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine($"Error al generar imagen: {error}");
                        return null;
                    }
                }

                if (File.Exists(dotFilePath))
                {
                    File.Delete(dotFilePath);
                }

                return relativePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GraphvizService: {ex.Message}");
                return null;
            }
        }

        private string GenerarDotTablaPlanVuelo(PlanVuelo plan)
        {
            string dot = "digraph G {\n";
            dot += "  node [shape=plaintext]\n";
            dot += "  fontname=\"Arial\"\n";
            dot += "  fontsize=10\n\n";

            dot += "  plan [label=<\n";
            dot += "    <table border=\"1\" cellborder=\"1\" cellspacing=\"0\" cellpadding=\"5\">\n";
            dot += "        <tr>\n";
            dot += "        <td bgcolor=\"lightblue\"><b>Tiempo (seg)</b></td>\n";

            // Obtener todos los nombres de drones del plan
            var primerosSegundos = plan.GetPrimero();
            if (primerosSegundos != null)
            {
                var acciones = primerosSegundos.Data.Acciones.GetPrimero();
                while (acciones != null)
                {
                    dot += $"        <td bgcolor=\"lightblue\"><b>{acciones.Data.NombreDron}</b></td>\n";
                    acciones = acciones.Siguiente;
                }
            }
            dot += "        </tr>\n";

            // Agregar filas por cada segundo
            PlanVuelo.NodoSegundo? actual = plan.GetPrimero();
            while (actual != null)
            {
                dot += "        <tr>\n";
                dot += $"         <td bgcolor=\"lightgray\"><b>{actual.Data.Segundo}</b></td>\n";

                PlanVuelo.NodoAccionDron? accionActual = actual.Data.Acciones.GetPrimero();
                while (accionActual != null)
                {
                    string accion = accionActual.Data.Accion.ToString();
                    string bgColor = "";

                    if (accion == "EmitirLuz")
                    {
                        bgColor = "bgcolor=\"lightgreen\"";
                    }
                    else if (accion == "Subir")
                    {
                        bgColor = "bgcolor=\"lightblue\"";
                    }
                    else if (accion == "Bajar")
                    {
                        bgColor = "bgcolor=\"lightcoral\"";
                    }

                    dot += $"        <td {bgColor}>{accion}</td>\n";
                    accionActual = accionActual.Siguiente;
                }
                dot += "        </tr>\n";
                actual = actual.Siguiente;
            }

            dot += "      </table>\n";
            dot += "  >];\n";
            dot += "}\n";

            return dot;
        }
    }
}