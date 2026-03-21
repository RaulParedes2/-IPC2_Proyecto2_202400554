using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class NodoInstruccionEmision
    {
        public InstruccionEmision Data { get; set; }
        public NodoInstruccionEmision? Siguiente { get; set; }

        public NodoInstruccionEmision(InstruccionEmision instruccion)
        {
            Data = instruccion;
            Siguiente = null;
        }
    }
}