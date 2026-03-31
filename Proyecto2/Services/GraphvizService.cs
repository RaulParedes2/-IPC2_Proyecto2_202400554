using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
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
            // Usar lista enlazada para las rutas posibles
            ListaRutas rutas = new ListaRutas();
            rutas.Agregar(@"C:\Program Files\Graphviz\bin\dot.exe");
            rutas.Agregar(@"C:\Program Files (x86)\Graphviz\bin\dot.exe");
            rutas.Agregar(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Graphviz\bin\dot.exe");
            rutas.Agregar(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Graphviz\bin\dot.exe");

            ListaRutas.NodoRuta? actual = rutas.GetPrimero();
            while (actual != null)
            {
                if (File.Exists(actual.Data))
                {
                    Console.WriteLine($"Graphviz encontrado en: {actual.Data}");
                    return actual.Data;
                }
                actual = actual.Siguiente;
            }

            // Buscar en variables de entorno PATH
            string? pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (pathEnv != null)
            {
                ListaDirectorios directorios = new ListaDirectorios();
                
                // Parsear PATH manualmente sin usar Split
                string temp = "";
                for (int i = 0; i < pathEnv.Length; i++)
                {
                    if (pathEnv[i] == ';')
                    {
                        if (temp.Length > 0)
                        {
                            directorios.Agregar(temp.Trim());
                            temp = "";
                        }
                    }
                    else
                    {
                        temp += pathEnv[i];
                    }
                }
                if (temp.Length > 0)
                {
                    directorios.Agregar(temp.Trim());
                }

                ListaDirectorios.NodoDirectorio? dirActual = directorios.GetPrimero();
                while (dirActual != null)
                {
                    string fullPath = Path.Combine(dirActual.Data, "dot.exe");
                    if (File.Exists(fullPath))
                    {
                        Console.WriteLine($"Graphviz encontrado en PATH: {fullPath}");
                        return fullPath;
                    }
                    dirActual = dirActual.Siguiente;
                }
            }

            Console.WriteLine("Graphviz NO encontrado en el sistema");
            return "dot";
        }

        public string? GenerarImagen(string dotCode, string nombreArchivo)
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
                    string? error = process.StandardError.ReadToEnd();

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

        public void LimpiarArchivosTemporales()
        {
            string tempFolder = Path.Combine(_wwwrootPath, "temp");
            if (Directory.Exists(tempFolder))
            {
                try
                {
                    string[] archivos = Directory.GetFiles(tempFolder);
                    for (int i = 0; i < archivos.Length; i++)
                    {
                        FileInfo fileInfo = new FileInfo(archivos[i]);
                        if (fileInfo.CreationTime < DateTime.Now.AddHours(-1))
                        {
                            File.Delete(archivos[i]);
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
            ListaDrones dronesOrdenados = sistema.Drones.ObtenerOrdenadosAlfabeticamente();

            int alturaMaxima = 0;
            TablaCodificacion.CeldaCodificacion? celda = sistema.Codificacion.GetPrimero();
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

            NodoDron? dronActual = dronesOrdenados.GetPrimero();
            while (dronActual != null)
            {
                dot += $"        <td bgcolor=\"lightblue\"><b>{dronActual.Data.Nombre}</b></td>\n";
                dronActual = dronActual.Siguiente;
            }
            dot += "        </tr>\n";

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

            PlanVuelo.NodoSegundo? primerosSegundos = plan.GetPrimero();
            if (primerosSegundos != null)
            {
                PlanVuelo.NodoAccionDron? acciones = primerosSegundos.Data.Acciones.GetPrimero();
                while (acciones != null)
                {
                    dot += $"        <td bgcolor=\"lightblue\"><b>{acciones.Data.NombreDron}</b></td>\n";
                    acciones = acciones.Siguiente;
                }
            }
            dot += "        </tr>\n";

            PlanVuelo.NodoSegundo? actual = plan.GetPrimero();
            while (actual != null)
            {
                dot += "        <tr>\n";
                dot += $"         <td bgcolor=\"lightgray\"><b>{actual.Data.Segundo}</b></td>\n";

                PlanVuelo.NodoAccionDron? accionActual = actual.Data.Acciones.GetPrimero();
                while (accionActual != null)
                {
                    string accion = accionActual.Data.Accion;
                    string bgColor = "";

                    if (accion == "Emitir Luz")
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