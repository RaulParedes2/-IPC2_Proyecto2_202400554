using System;
using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class ListaInstruccionesEmision
    {
        private NodoInstruccionEmision? primero;
        private int count;

        public ListaInstruccionesEmision()
        {
            primero = null;
            count = 0;
        }

        public void Agregar(InstruccionEmision instruccion)
        {
            NodoInstruccionEmision nuevo = new NodoInstruccionEmision(instruccion);

            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoInstruccionEmision actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }

        public InstruccionEmision? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= count)
                return null;

            NodoInstruccionEmision actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }

        public bool Eliminar(int indice)
        {
            if (indice < 0 || indice >= count)
                return false;

            if (indice == 0)
            {
                primero = primero!.Siguiente;
                count--;
                return true;
            }

            NodoInstruccionEmision actual = primero!;
            for (int i = 0; i < indice - 1; i++)
            {
                actual = actual.Siguiente!;
            }
            actual.Siguiente = actual.Siguiente!.Siguiente;
            count--;
            return true;
        }

        public int Count { get { return count; } }
        
        public bool EstaVacia { get { return count == 0; } }

        public NodoInstruccionEmision? GetPrimero()
        {
            return primero;
        }

        public void Limpiar()
        {
            primero = null;
            count = 0;
        }

        public void ParaCada(Action<InstruccionEmision> accion)
        {
            NodoInstruccionEmision? actual = primero;
            while (actual != null)
            {
                accion(actual.Data);
                actual = actual.Siguiente;
            }
        }
    }
}