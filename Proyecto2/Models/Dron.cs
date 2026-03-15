using System;
using IPC2_Proyecto2.Models.Enums;

namespace IPC2_Proyecto2.Models
{
    public class Dron
    {
        private string nombre;
        private int alturaActual;
        private int alturaInicial;
        private bool estaEmitiendo;

        public string Nombre
        {
            set
            {
                nombre = value;
            }
            get
            {
                return nombre;
            }
        }
        public int AlturaActual
        {
            set
            {
                alturaActual= value;
            }
            get
            {
                return alturaActual;
            }
        }
        public int AlturaInicial
        {
            set
            {
                alturaInicial = value;
            }
            get
            {
                return alturaInicial;
            }
        }
        public bool EstaEmitiendo
        {
            set
            {
                estaEmitiendo = value;
            }
            get
            {
                return estaEmitiendo;
            }
        }
        public Dron(string nombre)
        {
            this.nombre = nombre;
            AlturaInicial = 1; //empiezan en 1 metro
            AlturaActual = AlturaInicial;
            EstaEmitiendo = false;
        }

        public Dron(string nombre, int alturaInicial)
        {
            this.nombre = nombre;
            this.alturaInicial = alturaInicial;
            this.alturaActual = alturaInicial;
            EstaEmitiendo = false;
        }
         // Acciones que puede realizar el dron
         
    }
}
