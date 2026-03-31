using System;

namespace Proyecto2.TDAs
{
    public class ListaBooleanos
    {
        private class NodoBoolean
        {
            public bool Data { get; set; }
            public NodoBoolean? Siguiente { get; set; }
            
            public NodoBoolean(bool data)
            {
                Data = data;
                Siguiente = null;
            }
        }
        
        private NodoBoolean? primero;
        private int count;
        
        public ListaBooleanos()
        {
            primero = null;
            count = 0;
        }
        
        public void Agregar(bool valor)
        {
            NodoBoolean nuevo = new NodoBoolean(valor);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoBoolean actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }
        
        public bool Obtener(int indice)
        {
            if (indice < 0 || indice >= count)
                return false;
                
            NodoBoolean actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }
        
        public void Actualizar(int indice, bool valor)
        {
            if (indice < 0 || indice >= count)
                return;
                
            NodoBoolean actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            actual.Data = valor;
        }
        
        public int Count { get { return count; } }
    }
}