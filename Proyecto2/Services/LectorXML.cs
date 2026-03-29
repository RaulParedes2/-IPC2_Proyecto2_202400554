using System;
using System.IO;
using System.Xml;
using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class LectorXML
    {
        private readonly GestorSistemas _gestorSistemas;
        private readonly GestorMensajes _gestorMensajes;
        private readonly GestorDrones _gestorDrones;

        public LectorXML(
            GestorSistemas gestorSistemas,
            GestorMensajes gestorMensajes,
            GestorDrones gestorDrones)
        {
            _gestorSistemas = gestorSistemas;
            _gestorMensajes = gestorMensajes;
            _gestorDrones = gestorDrones;
        }

        public bool CargarConfiguracion(string rutaArchivo)
        {
            try
            {
                if (!System.IO.File.Exists(rutaArchivo))
                {
                    Console.WriteLine($"Archivo no encontrado: {rutaArchivo}");
                    return false;
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(rutaArchivo);

                XmlNode? root = doc.SelectSingleNode("config");
                if (root == null)
                {
                    Console.WriteLine("No se encontró el nodo 'config'");
                    return false;
                }

                // 1. Cargar drones
                XmlNode? listaDronesNode = root.SelectSingleNode("listaDrones");
                if (listaDronesNode != null)
                {
                    CargarDrones(listaDronesNode);
                }

                // 2. Cargar sistemas de drones
                XmlNode? listaSistemasNode = root.SelectSingleNode("listaSistemasDrones");
                if (listaSistemasNode != null)
                {
                    CargarSistemas(listaSistemasNode);
                }

                // 3. Cargar mensajes
                XmlNode? listaMensajesNode = root.SelectSingleNode("listaMensajes");
                if (listaMensajesNode != null)
                {
                    CargarMensajes(listaMensajesNode);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar XML: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private void CargarDrones(XmlNode listaDronesNode)
        {
            XmlNodeList? drones = listaDronesNode.SelectNodes("dron");
            if (drones == null) return;

            foreach (XmlNode dronNode in drones)
            {
                string? nombreDron = dronNode.InnerText?.Trim();
                if (!string.IsNullOrEmpty(nombreDron) && !_gestorDrones.ExisteDron(nombreDron))
                {
                    _gestorDrones.AgregarDron(nombreDron);
                    Console.WriteLine($"Dron agregado: {nombreDron}");
                }
                else if (!string.IsNullOrEmpty(nombreDron))
                {
                    Console.WriteLine($"Dron ya existe, no se agrega: {nombreDron}");
                }
            }
        }

        private void CargarSistemas(XmlNode listaSistemasNode)
        {
            XmlNodeList? sistemas = listaSistemasNode.SelectNodes("sistemaDrones");
            if (sistemas == null) return;

            foreach (XmlNode sistemaNode in sistemas)
            {
                string? nombreSistema = sistemaNode.Attributes?["nombre"]?.Value;
                if (string.IsNullOrEmpty(nombreSistema)) continue;

                Console.WriteLine($"=== Procesando sistema: {nombreSistema} ===");

                if (!_gestorSistemas.ExisteSistema(nombreSistema))
                {
                    _gestorSistemas.AgregarSistema(nombreSistema);
                    Console.WriteLine($"Sistema creado: {nombreSistema}");
                }

                XmlNodeList? contenidos = sistemaNode.SelectNodes("contenido");
                if (contenidos != null)
                {
                    foreach (XmlNode contenidoNode in contenidos)
                    {
                        XmlNode? dronNode = contenidoNode.SelectSingleNode("dron");
                        string? nombreDron = dronNode?.InnerText?.Trim();

                        if (!string.IsNullOrEmpty(nombreDron))
                        {
                            Console.WriteLine($"  Procesando dron: {nombreDron}");

                            if (_gestorDrones.ExisteDron(nombreDron))
                            {
                                _gestorSistemas.AgregarDronASistema(nombreSistema, nombreDron);
                                Console.WriteLine($"    Dron {nombreDron} agregado al sistema");
                            }

                            XmlNode? alturasNode = contenidoNode.SelectSingleNode("alturas");
                            if (alturasNode != null)
                            {
                                XmlNodeList? alturas = alturasNode.SelectNodes("altura");
                                if (alturas != null)
                                {
                                    foreach (XmlNode alturaNode in alturas)
                                    {
                                        string? alturaValor = alturaNode.Attributes?["valor"]?.Value;
                                        string? letra = alturaNode.InnerText;

                                        Console.WriteLine($"      Leyendo: valor={alturaValor}, texto='{letra}', longitud={letra?.Length ?? 0}");

                                        char letraChar;

                                        if (letra == null)
                                        {
                                            letraChar = '?';
                                        }
                                        else if (letra.Length == 0)
                                        {
                                            // Si está vacío, verificar si debería ser espacio
                                            // Podemos usar un marcador como '_' pero es mejor usar espacio
                                            letraChar = ' ';  // Tratar vacío como espacio
                                            Console.WriteLine($"      >>> ETIQUETA VACÍA, USANDO ESPACIO <<<");
                                        }
                                        else if (letra == " " || letra == "&nbsp;" || letra == "\u00A0")
                                        {
                                            letraChar = ' ';
                                            Console.WriteLine($"      >>> ESPACIO DETECTADO! <<<");
                                        }
                                        else
                                        {
                                            letraChar = letra[0];
                                        }

                                        if (!string.IsNullOrEmpty(alturaValor) &&
                                            int.TryParse(alturaValor, out int altura))
                                        {
                                            _gestorSistemas.ConfigurarCodificacion(nombreSistema, nombreDron, altura, letraChar);
                                            Console.WriteLine($"      Guardado: {nombreDron} @ {altura}m = '{letraChar}'");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CargarMensajes(XmlNode listaMensajesNode)
        {
            XmlNodeList? mensajes = listaMensajesNode.SelectNodes("Mensaje");
            if (mensajes == null) return;

            foreach (XmlNode mensajeNode in mensajes)
            {
                string? nombreMensaje = mensajeNode.Attributes?["nombre"]?.Value;
                if (string.IsNullOrEmpty(nombreMensaje)) continue;

                if (!_gestorMensajes.ExisteMensaje(nombreMensaje))
                {
                    _gestorMensajes.AgregarMensaje(nombreMensaje, "");
                    Console.WriteLine($"Mensaje agregado: {nombreMensaje}");
                }

                XmlNode? sistemaDronesNode = mensajeNode.SelectSingleNode("sistemaDrones");
                string? nombreSistema = sistemaDronesNode?.InnerText?.Trim();

                XmlNode? instruccionesNode = mensajeNode.SelectSingleNode("instrucciones");
                if (instruccionesNode != null)
                {
                    XmlNodeList? instrucciones = instruccionesNode.SelectNodes("instruccion");
                    if (instrucciones != null)
                    {
                        foreach (XmlNode instNode in instrucciones)
                        {
                            string? nombreDron = instNode.Attributes?["dron"]?.Value;
                            string? alturaStr = instNode.InnerText?.Trim();

                            if (!string.IsNullOrEmpty(nombreDron) &&
                                !string.IsNullOrEmpty(alturaStr) &&
                                int.TryParse(alturaStr, out int altura))
                            {
                                char letra = '?';

                                if (!string.IsNullOrEmpty(nombreSistema))
                                {
                                    SistemaDrones? sistema = _gestorSistemas.ObtenerPorNombre(nombreSistema);
                                    if (sistema != null)
                                    {
                                        letra = sistema.ObtenerLetra(nombreDron, altura);
                                        Console.WriteLine($"  Letra obtenida: {nombreDron} @ {altura}m = '{letra}'");
                                    }
                                }

                                _gestorMensajes.AgregarInstruccion(nombreMensaje, nombreDron, altura, letra);
                            }
                        }
                    }
                }
            }
        }
    }
}