using System;
using Proyecto2.Models.Enums;

namespace Proyecto2.Models
{
    public class PlanVuelo
    {
        public class AccionDronEnSegundo
        {
            public string NombreDron { get; set; } = string.Empty;
            public string Accion { get; set; } = string.Empty;
            public int AlturaResultante { get; set; }
            
            public AccionDronEnSegundo(string nombreDron, string accion, int alturaResultante = 0)
            {
                NombreDron = nombreDron;
                Accion = accion;
                AlturaResultante = alturaResultante;
            }
        }
        
        public class NodoAccionDron
        {
            public AccionDronEnSegundo Data { get; set; }
            public NodoAccionDron? Siguiente { get; set; }
            
            public NodoAccionDron(AccionDronEnSegundo accion)
            {
                Data = accion;
                Siguiente = null;
            }
        }
        
        public class ListaAccionesDron
        {
            private NodoAccionDron? primero;
            private int count;
            
            public void Agregar(AccionDronEnSegundo accion)
            {
                NodoAccionDron nuevo = new NodoAccionDron(accion);
                
                if (primero == null)
                {
                    primero = nuevo;
                }
                else
                {
                    NodoAccionDron actual = primero;
                    while (actual.Siguiente != null)
                    {
                        actual = actual.Siguiente;
                    }
                    actual.Siguiente = nuevo;
                }
                count++;
            }
            
            public void Reemplazar(string nombreDron, string accion, int alturaResultante)
            {
                NodoAccionDron? actual = primero;
                while (actual != null)
                {
                    if (actual.Data.NombreDron == nombreDron)
                    {
                        actual.Data.Accion = accion;
                        actual.Data.AlturaResultante = alturaResultante;
                        return;
                    }
                    actual = actual.Siguiente;
                }
                // Si no existe, agregar nueva
                Agregar(new AccionDronEnSegundo(nombreDron, accion, alturaResultante));
            }
            
            public NodoAccionDron? GetPrimero()
            {
                return primero;
            }
            
            public int Count { get { return count; } }
        }
        
        public class AccionPorSegundo
        {
            public int Segundo { get; set; }
            public ListaAccionesDron Acciones { get; set; }
            
            public AccionPorSegundo(int segundo)
            {
                Segundo = segundo;
                Acciones = new ListaAccionesDron();
            }
            
            public void AgregarAccion(string nombreDron, string accion, int alturaResultante = 0)
            {
                Acciones.Agregar(new AccionDronEnSegundo(nombreDron, accion, alturaResultante));
            }
            
            public void ReemplazarAccion(string nombreDron, string accion, int alturaResultante = 0)
            {
                Acciones.Reemplazar(nombreDron, accion, alturaResultante);
            }
        }
        
        public class NodoSegundo
        {
            public AccionPorSegundo Data { get; set; }
            public NodoSegundo? Siguiente { get; set; }
            
            public NodoSegundo(AccionPorSegundo accion)
            {
                Data = accion;
                Siguiente = null;
            }
        }
        
        private NodoSegundo? primero;
        private int count;
        private int tiempoOptimo;
        
        public string NombreMensaje { get; set; } = string.Empty;
        public string NombreSistema { get; set; } = string.Empty;
        public string MensajeOriginal { get; set; } = string.Empty;
        public string MensajeRecibido { get; set; } = string.Empty;
        
        public int TiempoOptimo 
        { 
            get { return tiempoOptimo; }
            set { tiempoOptimo = value; }
        }
        
        public PlanVuelo()
        {
            primero = null;
            count = 0;
            tiempoOptimo = 0;
        }
        
        public void AgregarSegundo(AccionPorSegundo segundo)
        {
            NodoSegundo nuevo = new NodoSegundo(segundo);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoSegundo actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
            tiempoOptimo = count;
        }
        
        public NodoSegundo? GetPrimero()
        {
            return primero;
        }
        
        public int Count { get { return count; } }
        
        public string GenerarXML()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            xml += "<respuesta>\n";
            xml += "  <listaMensajes>\n";
            xml += $"    <mensaje nombre=\"{NombreMensaje}\">\n";
            xml += $"      <sistemaDrones>{NombreSistema}</sistemaDrones>\n";
            xml += $"      <tiempoOptimo>{tiempoOptimo}</tiempoOptimo>\n";
            xml += $"      <mensajeRecibido>{MensajeRecibido}</mensajeRecibido>\n";
            xml += "      <instrucciones>\n";
            
            NodoSegundo? actual = primero;
            while (actual != null)
            {
                xml += $"        <tiempo valor=\"{actual.Data.Segundo}\">\n";
                xml += "          <acciones>\n";
                
                NodoAccionDron? accionActual = actual.Data.Acciones.GetPrimero();
                while (accionActual != null)
                {
                    xml += $"            <dron nombre=\"{accionActual.Data.NombreDron}\">{accionActual.Data.Accion}</dron>\n";
                    accionActual = accionActual.Siguiente;
                }
                
                xml += "          </acciones>\n";
                xml += "        </tiempo>\n";
                actual = actual.Siguiente;
            }
            
            xml += "      </instrucciones>\n";
            xml += "    </mensaje>\n";
            xml += "  </listaMensajes>\n";
            xml += "</respuesta>";
            
            return xml;
        }
    }
}