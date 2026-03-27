using System;
using System.IO;
using System.Text;
using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class GeneradorXML
    {
        private readonly GestorMensajes _gestorMensajes;
        private readonly Planificador _planificador;

        public GeneradorXML(GestorMensajes gestorMensajes, Planificador planificador)
        {
            _gestorMensajes = gestorMensajes;
            _planificador = planificador;
        }

        public string GenerarXMLSalida(string nombreSistema, GestorSistemas gestorSistemas)
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\"?>");
            xml.AppendLine("<respuesta>");
            xml.AppendLine("  <listaMensajes>");

            SistemaDrones? sistema = gestorSistemas.ObtenerPorNombre(nombreSistema);
            if (sistema == null)
            {
                xml.AppendLine("    <mensaje>");
                xml.AppendLine("      <error>Sistema no encontrado</error>");
                xml.AppendLine("    </mensaje>");
            }
            else
            {
                ListaMensajes mensajes = _gestorMensajes.ObtenerTodos();
                NodoMensaje? actual = mensajes.GetPrimero();

                while (actual != null)
                {
                    PlanVuelo? plan = _planificador.CalcularPlan(actual.Data, sistema);
                    
                    xml.AppendLine($"    <mensaje nombre=\"{actual.Data.Nombre}\">");
                    xml.AppendLine($"      <sistemaDrones>{sistema.Nombre}</sistemaDrones>");
                    
                    if (plan != null)
                    {
                        xml.AppendLine($"      <tiempoOptimo>{plan.TiempoOptimo}</tiempoOptimo>");
                        xml.AppendLine($"      <mensajeRecibido>{plan.MensajeRecibido}</mensajeRecibido>");
                        xml.AppendLine("      <instrucciones>");
                        
                        PlanVuelo.NodoSegundo? segundoActual = plan.GetPrimero();
                        while (segundoActual != null)
                        {
                            xml.AppendLine($"        <tiempo valor=\"{segundoActual.Data.Segundo}\">");
                            xml.AppendLine("          <acciones>");
                            
                            PlanVuelo.NodoAccionDron? accionActual = segundoActual.Data.Acciones.GetPrimero();
                            while (accionActual != null)
                            {
                                string accion = accionActual.Data.Accion.ToString();
                                xml.AppendLine($"            <dron nombre=\"{accionActual.Data.NombreDron}\"> {accion} </dron>");
                                accionActual = accionActual.Siguiente;
                            }
                            
                            xml.AppendLine("          </acciones>");
                            xml.AppendLine("        </tiempo>");
                            segundoActual = segundoActual.Siguiente;
                        }
                        
                        xml.AppendLine("      </instrucciones>");
                    }
                    else
                    {
                        xml.AppendLine("      <tiempoOptimo>0</tiempoOptimo>");
                        xml.AppendLine("      <mensajeRecibido></mensajeRecibido>");
                        xml.AppendLine("      <instrucciones></instrucciones>");
                    }
                    
                    xml.AppendLine("    </mensaje>");
                    actual = actual.Siguiente;
                }
            }

            xml.AppendLine("  </listaMensajes>");
            xml.AppendLine("</respuesta>");

            return xml.ToString();
        }

        public void GuardarXMLSalida(string nombreArchivo, string contenido)
        {
            string ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "xml", nombreArchivo);
            string? directorio = Path.GetDirectoryName(ruta);
            
            if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }
            
            File.WriteAllText(ruta, contenido);
        }
    }
}