namespace Proyecto2.TDAs
{
    public class ListaColasInstrucciones
    {
        public class NodoColaInstruccion
        {
            public ColaInstrucciones Data { get; set; }
            public NodoColaInstruccion? Siguiente { get; set; }
            
            public NodoColaInstruccion(ColaInstrucciones data)
            {
                Data = data;
                Siguiente = null;
            }
        }
        
        private NodoColaInstruccion? primero;
        private int count;
        
        public ListaColasInstrucciones()
        {
            primero = null;
            count = 0;
        }
        
        public void Agregar(ColaInstrucciones cola)
        {
            NodoColaInstruccion nuevo = new NodoColaInstruccion(cola);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoColaInstruccion actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }
        
        public ColaInstrucciones? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= count)
                return null;
                
            NodoColaInstruccion actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }
        
        public int Count { get { return count; } }
    }
}