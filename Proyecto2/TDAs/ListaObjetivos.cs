using System;

namespace Proyecto2.TDAs
{
    public class ListaObjetivos
    {
        private class NodoObjetivo
        {
            public int Data { get; set; }
            public NodoObjetivo? Siguiente { get; set; }
            
            public NodoObjetivo(int data)
            {
                Data = data;
                Siguiente = null;
            }
        }
        
        private NodoObjetivo? primero;
        private int count;
        
        public ListaObjetivos()
        {
            primero = null;
            count = 0;
        }
        
        public void Agregar(int valor)
        {
            NodoObjetivo nuevo = new NodoObjetivo(valor);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoObjetivo actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }
        
        public int Obtener(int indice)
        {
            if (indice < 0 || indice >= count)
                return 1;
                
            NodoObjetivo actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }
        
        public void Actualizar(int indice, int valor)
        {
            if (indice < 0 || indice >= count)
                return;
                
            NodoObjetivo actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            actual.Data = valor;
        }
        
        public int Count { get { return count; } }
    }
}