using System;

namespace Proyecto2.TDAs
{
    public class MiDiccionario<TClave, TValor> where TClave : IComparable<TClave>
    {
        // Hacemos la clase NodoDiccionario pública
        public class NodoDiccionario
        {
            public TClave Clave { get; set; }
            public TValor Valor { get; set; }
            public NodoDiccionario? Siguiente { get; set; }
            
            public NodoDiccionario(TClave clave, TValor valor)
            {
                Clave = clave;
                Valor = valor;
                Siguiente = null;
            }
        }
        
        private NodoDiccionario? primero;
        private int count;
        
        public MiDiccionario()
        {
            primero = null;
            count = 0;
        }
        
        public void Agregar(TClave clave, TValor valor)
        {
            // Verificar si ya existe
            if (ContieneClave(clave))
                return;
            
            NodoDiccionario nuevo = new NodoDiccionario(clave, valor);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoDiccionario actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }
        
        public TValor? Obtener(TClave clave)
        {
            NodoDiccionario? actual = primero;
            while (actual != null)
            {
                if (actual.Clave.Equals(clave))
                    return actual.Valor;
                actual = actual.Siguiente;
            }
            return default(TValor);
        }
        
        public bool ContieneClave(TClave clave)
        {
            NodoDiccionario? actual = primero;
            while (actual != null)
            {
                if (actual.Clave.Equals(clave))
                    return true;
                actual = actual.Siguiente;
            }
            return false;
        }
        
        public bool Eliminar(TClave clave)
        {
            if (primero == null) return false;
            
            if (primero.Clave.Equals(clave))
            {
                primero = primero.Siguiente;
                count--;
                return true;
            }
            
            NodoDiccionario actual = primero;
            while (actual.Siguiente != null)
            {
                if (actual.Siguiente.Clave.Equals(clave))
                {
                    actual.Siguiente = actual.Siguiente.Siguiente;
                    count--;
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }
        
        public NodoDiccionario? GetPrimero()
        {
            return primero;
        }
        
        public int Count { get { return count; } }
        
        public void ParaCada(Action<TClave, TValor> accion)
        {
            NodoDiccionario? actual = primero;
            while (actual != null)
            {
                accion(actual.Clave, actual.Valor);
                actual = actual.Siguiente;
            }
        }
        
        public bool EstaVacia()
        {
            return count == 0;
        }
        
        public void Limpiar()
        {
            primero = null;
            count = 0;
        }
    }
}