using System;
using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class TablaCodificacion
    {
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
            return '?';
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

        public bool EliminarCombinacion(string nombreDron, int altura)
        {
            if (primera == null) return false;

            if (primera.NombreDron == nombreDron && primera.Altura == altura)
            {
                primera = primera.Siguiente;
                count--;
                return true;
            }

            CeldaCodificacion actual = primera;
            while (actual.Siguiente != null)
            {
                if (actual.Siguiente.NombreDron == nombreDron && actual.Siguiente.Altura == altura)
                {
                    actual.Siguiente = actual.Siguiente.Siguiente;
                    count--;
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }

        public int Count { get { return count; } }

        public CeldaCodificacion? GetPrimero()
        {
            return primera;
        }

        public void Limpiar()
        {
            primera = null;
            count = 0;
        }

        public void ParaCada(Action<CeldaCodificacion> accion)
        {
            CeldaCodificacion? actual = primera;
            while (actual != null)
            {
                accion(actual);
                actual = actual.Siguiente;
            }
        }
    }
}