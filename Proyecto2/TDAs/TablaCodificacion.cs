using System;
using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class TablaCodificacion
    {
        // Hacemos la clase interna pública para que pueda ser accesible
        public class CeldaCodificacion
        {
            public string NombreDron { get; set; }
            public int Altura { get; set; }
            public char Letra { get; set; }
            public CeldaCodificacion? Siguiente { get; set; }

            public CeldaCodificacion(string dron, int altura, char letra)
            {
                NombreDron = dron;
                Altura = altura;
                Letra = letra;
                Siguiente = null;
            }
        }

        private CeldaCodificacion? primera;
        private int count;

        public TablaCodificacion()
        {
            primera = null;
            count = 0;
        }

        public void Agregar(string nombreDron, int altura, char letra)
        {
            // Verificar si ya existe la combinación
            if (ExisteCombinacion(nombreDron, altura))
                return;

            CeldaCodificacion nueva = new CeldaCodificacion(nombreDron, altura, letra);

            if (primera == null)
            {
                primera = nueva;
            }
            else
            {
                CeldaCodificacion actual = primera;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nueva;
            }
            count++;
        }

        public char ObtenerLetra(string nombreDron, int altura)
        {
            CeldaCodificacion? actual = primera;
            while (actual != null)
            {
                if (actual.NombreDron == nombreDron && actual.Altura == altura)
                    return actual.Letra;
                actual = actual.Siguiente;
            }
            return '?'; // Carácter no encontrado
        }

        public bool ExisteCombinacion(string nombreDron, int altura)
        {
            CeldaCodificacion? actual = primera;
            while (actual != null)
            {
                if (actual.NombreDron == nombreDron && actual.Altura == altura)
                    return true;
                actual = actual.Siguiente;
            }
            return false;
        }

        public ListaDrones? ObtenerDronesConLetra(char letra)
        {
            ListaDrones? dronesConLetra = new ListaDrones();
            CeldaCodificacion? actual = primera;

            while (actual != null)
            {
                if (actual.Letra == letra)
                {
                    // Aquí solo tenemos el nombre del dron, necesitamos el objeto Dron
                    // Este método se usará en conjunto con un sistema de drones
                }
                actual = actual.Siguiente;
            }

            return dronesConLetra;
        }

        public int Count { get { return count; } }

        // Cambiamos el tipo de retorno a publico
        public CeldaCodificacion? GetPrimero()
        {
            return primera;
        }

        public void Limpiar()
        {
            primera = null;
            count = 0;
        }

        // Método para recorrer todas las celdas
        public void ParaCada(Action<CeldaCodificacion> accion)
        {
            CeldaCodificacion? actual = primera;
            while (actual != null)
            {
                accion(actual);
                actual = actual.Siguiente;
            }
        }

        // Método para obtener todas las combinaciones como string
        public string ObtenerListadoComoString()
        {
            string resultado = "";
            CeldaCodificacion? actual = primera;
            while (actual != null)
            {
                resultado += $"{actual.NombreDron} a {actual.Altura}m = '{actual.Letra}'";
                if (actual.Siguiente != null)
                    resultado += "\n";
                actual = actual.Siguiente;
            }
            return resultado;
        }

        // Método para obtener las letras disponibles para un dron específico
        public ListaInstruccionesEmision? ObtenerInstruccionesPorDron(string nombreDron)
        {
            ListaInstruccionesEmision instrucciones = new ListaInstruccionesEmision();
            CeldaCodificacion? actual = primera;

            while (actual != null)
            {
                if (actual.NombreDron == nombreDron)
                {
                    instrucciones.Agregar(new InstruccionEmision(
                        actual.NombreDron,
                        actual.Altura,
                        actual.Letra
                    ));
                }
                actual = actual.Siguiente;
            }

            return instrucciones;
        }
    }
}
