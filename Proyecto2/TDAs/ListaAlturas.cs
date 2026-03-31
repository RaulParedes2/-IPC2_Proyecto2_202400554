using System;
using Proyecto2.Models;
using Proyecto2.Services;

namespace Proyecto2.TDAs
{
    public class ListaAlturas
    {
        public class NodoAltura
        {
            public int Data { get; set; }
            public NodoAltura? Siguiente { get; set; }
            
            public NodoAltura(int data)
            {
                Data = data;
                Siguiente = null;
            }
        }
        
        private NodoAltura? primero;
        private int count;
        
        public ListaAlturas()
        {
            primero = null;
            count = 0;
        }
        
        public void Agregar(int altura)
        {
            NodoAltura nuevo = new NodoAltura(altura);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoAltura actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }
        
        public int ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= count)
                return 1;
                
            NodoAltura actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }
        
        public void ActualizarPorIndice(int indice, int valor)
        {
            if (indice < 0 || indice >= count)
                return;
                
            NodoAltura actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            actual.Data = valor;
        }
        
        public int Count { get { return count; } }
        
        public NodoAltura? GetPrimero()
        {
            return primero;
        }
    }
}