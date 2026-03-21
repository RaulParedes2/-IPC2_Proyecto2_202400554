using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class NodoSistemaDrones
    {
        public SistemaDrones Data { get; set; }
        public NodoSistemaDrones? Siguiente { get; set; }

        public NodoSistemaDrones(SistemaDrones sistema)
        {
            Data = sistema;
            Siguiente = null;
        }
    }
}