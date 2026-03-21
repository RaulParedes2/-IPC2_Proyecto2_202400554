using System;
using Proyecto2.Models.Enums;

namespace Proyecto2.Models
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
                alturaActual = value;
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
        public void Subir()
        {
            if (AlturaActual < 100) //Límite máximo de 100 metros
            {
                AlturaActual++;
            }
            EstaEmitiendo = false;
        }

        public void Bajar()
        {
            if (AlturaActual > 1) //Límite mínimo de 1 metro
            {
                AlturaActual--;
            }
            EstaEmitiendo = false;
        }
        public void Esperar()
        {
            // No hace nada, solo permanece en la misma altura
            EstaEmitiendo = false;
        }

        public void EmitirLuz()
        {
            EstaEmitiendo = true;
        }
        // Reiniciar a la altura inicial

        public void Reiniciar()
        {
            AlturaActual = AlturaInicial;
            EstaEmitiendo = false;
        }

        // Calcular distancia a una altura objetivo
        public int DistanciaA(int alturaObjetivo)
        {
            return Math.Abs(AlturaActual - alturaObjetivo);
        }

        // Determinar qué acción hacer para llegar a una altura

        public AccionDron AccionNecesaria(int alturaObjetivo)
        {
            if (AlturaActual < alturaObjetivo)
                return AccionDron.Subir;
            else if (AlturaActual > alturaObjetivo)
                return AccionDron.Bajar;
            else
                return AccionDron.EmitirLuz; // Si ya está en la altura, emitir
        }

        public override string ToString()
        {
            return $"Dron: {Nombre}, Altura: {AlturaActual}m";
        }
    }
}
