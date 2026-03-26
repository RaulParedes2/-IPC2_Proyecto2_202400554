using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Proyecto2.Services
{
    public class GraphvizService
    {
        private readonly string _graphvizPath;
        private readonly string _wwwrootPath;

        public GraphvizService(IWebHostEnvironment webHostEnvironment)
        {
            _wwwrootPath = webHostEnvironment.WebRootPath ?? Directory.GetCurrentDirectory() + "/wwwroot";
            _graphvizPath = FindGraphvizPath();
        }

        private string FindGraphvizPath()
        {
            // Rutas comunes de instalación de Graphviz
            string[] possiblePaths = new string[]
            {
                @"C:\Program Files\Graphviz\bin\dot.exe",
                @"C:\Program Files (x86)\Graphviz\bin\dot.exe",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Graphviz\bin\dot.exe"
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // Si no se encuentra, retornar "dot" para intentar usar el PATH
            return "dot";
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

                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        string error = process.StandardError.ReadToEnd();

                        if (process.ExitCode != 0)
                        {
                            Console.WriteLine($"Error al generar imagen: {error}");
                            return null;
                        }
                    }
                    else
                    {
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
                    // Eliminar archivos con más de 1 hora
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
    }
}