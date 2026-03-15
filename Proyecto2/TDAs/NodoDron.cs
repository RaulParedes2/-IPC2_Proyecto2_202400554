using System;
using IPC2_Proyecto2.Models;

namespace IPC2_Proyecto2.TDAs
{
    public class NodoDron
    {
        private Dron data;
        private NodoDron? siguiente;

        public Dron Data
        {
            set
            {
                data = value;
            }
            get
            {
                return data;
            }
        }
        public NodoDron? Siguiente
        {
            set
            {
                siguiente = value;
            }
            get
            {
                return siguiente;
            }
        }

        public NodoDron(Dron dron)
        {
            this.data = dron;
            Siguiente = null;
        }
    }
}