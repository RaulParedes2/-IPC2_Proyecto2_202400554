using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class NodoCola
    {
        public InstruccionEmision Data { get; set; }
        public NodoCola? Siguiente { get; set; }

        public NodoCola(InstruccionEmision instruccion)
        {
            Data = instruccion;
            Siguiente = null;
        }
    }
}