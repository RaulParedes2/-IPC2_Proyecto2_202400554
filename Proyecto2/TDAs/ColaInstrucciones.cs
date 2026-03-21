using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class ColaInstrucciones
    {
        private NodoCola? frente;
        private NodoCola? final;
        private int count;

        public ColaInstrucciones()
        {
            frente = null;
            final = null;
            count = 0;
        }

        public void Encolar(InstruccionEmision instruccion)
        {
            NodoCola nuevo = new NodoCola(instruccion);

            if (frente == null)
            {
                frente = nuevo;
                final = nuevo;
            }
            else
            {
                final!.Siguiente = nuevo;
                final = nuevo;
            }
            count++;
        }

        public InstruccionEmision? Desencolar()
        {
            if (frente == null)
                return null;

            InstruccionEmision valor = frente.Data;
            frente = frente.Siguiente;

            if (frente == null)
                final = null;

            count--;
            return valor;
        }

        public InstruccionEmision? VerFrente()
        {
            if (frente == null)
                return null;
            
            return frente.Data;
        }

        public bool EstaVacia()
        {
            return frente == null;
        }

        public int Count { get { return count; } }

        public void Limpiar()
        {
            frente = null;
            final = null;
            count = 0;
        }
    }
}
