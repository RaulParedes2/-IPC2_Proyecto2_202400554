using System;
using Proyecto2.Models.Enums;
using Proyecto2.TDAs;

namespace Proyecto2.Models
{
    public class PlanVuelo
    {
        // Clase para la acción de un dron en un segundo
        public class AccionDronEnSegundo
        {
            public string NombreDron { get; set; } = string.Empty;
            public AccionDron Accion { get; set; }
             public int AlturaResultante { get; set; }
            
            public AccionDronEnSegundo(string nombreDron, AccionDron accion, int alturaResultante = 0)
            {
                NombreDron = nombreDron;
                Accion = accion;
                AlturaResultante = alturaResultante;
            }
        }
        
        // Nodo para la lista de acciones de drones
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
        
        // Lista de acciones de drones
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
            
            public NodoAccionDron? GetPrimero()
            {
                return primero;
            }
            
            public int Count { get { return count; } }
            public bool EstaVacia { get { return count == 0; } }
        }
        
        // Clase para las acciones en un segundo específico
        public class AccionPorSegundo
        {
            public int Segundo { get; set; }
            public ListaAccionesDron Acciones { get; set; }
            
            public AccionPorSegundo(int segundo)
            {
                Segundo = segundo;
                Acciones = new ListaAccionesDron();
            }
            
            public void AgregarAccion(string nombreDron, AccionDron accion, int alturaResultante = 0)
            {
                Acciones.Agregar(new AccionDronEnSegundo(nombreDron, accion, alturaResultante));
            }
        }
        
        // Nodo para la lista de segundos
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
        
        public AccionPorSegundo? ObtenerSegundo(int indice)
        {
            if (indice < 0 || indice >= count)
                return null;
                
            NodoSegundo actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }
        
        public NodoSegundo? GetPrimero()
        {
            return primero;
        }
        
        public int Count { get { return count; } }
        public bool EstaVacio { get { return count == 0; } }
        
        // Método para generar un string con el resumen del plan
        public string ObtenerResumen()
        {
            string resultado = $"Plan de vuelo para '{NombreMensaje}'\n";
            resultado += $"Sistema: {NombreSistema}\n";
            resultado += $"Mensaje original: {MensajeOriginal}\n";
            resultado += $"Mensaje recibido: {MensajeRecibido}\n";
            resultado += $"Tiempo óptimo: {tiempoOptimo} segundos\n";
            resultado += $"Total de segundos en plan: {count}\n\n";
            
            resultado += "=== SECUENCIA DE ACCIONES ===\n";
            NodoSegundo? actual = primero;
            int segundoNum = 1;
            while (actual != null)
            {
                resultado += $"Segundo {segundoNum}:\n";
                AccionPorSegundo segundo = actual.Data;
                
                NodoAccionDron? accionActual = segundo.Acciones.GetPrimero();
                while (accionActual != null)
                {
                    string nombreAccion = accionActual.Data.Accion.ToString();
                    resultado += $"  - {accionActual.Data.NombreDron}: {nombreAccion}\n";
                    accionActual = accionActual.Siguiente;
                }
                
                actual = actual.Siguiente;
                segundoNum++;
            }
            
            return resultado;
        }
        
        // Método para generar el XML de salida
        public string GenerarXML()
        {
            string xml = "<?xml version=\"1.0\"?>\n";
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
                    string nombreAccion = accionActual.Data.Accion.ToString();
                    xml += $"            <dron nombre=\"{accionActual.Data.NombreDron}\">";
                    xml += $"{nombreAccion}</dron>\n";
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
        
        // Método para obtener una representación en texto del plan (para Graphviz)
        public string ObtenerGrafico()
        {
            string dot = "digraph PlanVuelo {\n";
            dot += "  rankdir=TB;\n";
            dot += "  node [shape=box];\n\n";
            
            dot += "  // Nodos de tiempo\n";
            NodoSegundo? actual = primero;
            int segundoNum = 1;
            NodoSegundo? anterior = null;
            
            while (actual != null)
            {
                dot += $"  segundo{segundoNum} [label=\"Segundo {segundoNum}\"];\n";
                
                if (anterior != null)
                {
                    dot += $"  segundo{segundoNum - 1} -> segundo{segundoNum};\n";
                }
                
                // Agregar acciones como subnodos
                NodoAccionDron? accionActual = actual.Data.Acciones.GetPrimero();
                int accionNum = 1;
                while (accionActual != null)
                {
                    string nombreAccion = accionActual.Data.Accion.ToString();
                    dot += $"  accion{segundoNum}_{accionNum} [label=\"{accionActual.Data.NombreDron}: {nombreAccion}\", shape=ellipse];\n";
                    dot += $"  segundo{segundoNum} -> accion{segundoNum}_{accionNum};\n";
                    accionActual = accionActual.Siguiente;
                    accionNum++;
                }
                
                anterior = actual;
                actual = actual.Siguiente;
                segundoNum++;
            }
            
            dot += "}\n";
            return dot;
        }
    }
}