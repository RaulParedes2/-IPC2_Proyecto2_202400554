using System;
using Proyecto2.Models;
using Proyecto2.Models.Enums;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class Planificador
    {
        public PlanVuelo? CalcularPlan(Mensaje mensaje, SistemaDrones sistema)
        {
            if (mensaje.Instrucciones.Count == 0)
                return null;

            Console.WriteLine($"=== Calculando plan para mensaje: {mensaje.Nombre} ===");

            // Crear estado inicial de drones (altura 1)
            ListaDrones dronesEstado = new ListaDrones();
            NodoDron? dronActual = sistema.Drones.GetPrimero();
            while (dronActual != null)
            {
                dronesEstado.Agregar(new Dron(dronActual.Data.Nombre, 1));
                dronActual = dronActual.Siguiente;
            }

            // Crear cola de instrucciones
            ColaInstrucciones cola = new ColaInstrucciones();
            NodoInstruccionEmision? instActual = mensaje.Instrucciones.GetPrimero();
            while (instActual != null)
            {
                cola.Encolar(instActual.Data);
                instActual = instActual.Siguiente;
            }

            PlanVuelo plan = new PlanVuelo();
            plan.NombreMensaje = mensaje.Nombre;
            plan.NombreSistema = sistema.Nombre;
            plan.MensajeOriginal = mensaje.TextoOriginal;

            int tiempo = 1;
            string mensajeRecibido = "";

            while (!cola.EstaVacia())
            {
                PlanVuelo.AccionPorSegundo segundo = new PlanVuelo.AccionPorSegundo(tiempo);
                
                // Inicializar todos los drones con Esperar
                NodoDron? dronEstado = dronesEstado.GetPrimero();
                while (dronEstado != null)
                {
                    segundo.AgregarAccion(dronEstado.Data.Nombre, AccionDron.Esperar, dronEstado.Data.AlturaActual);
                    dronEstado = dronEstado.Siguiente;
                }

                // Procesar la primera instrucción de la cola
                InstruccionEmision? instruccion = cola.VerFrente();
                if (instruccion != null)
                {
                    Dron? dron = dronesEstado.ObtenerPorNombre(instruccion.NombreDron);
                    if (dron != null)
                    {
                        if (dron.AlturaActual == instruccion.AlturaObjetivo)
                        {
                            // Emitir luz
                            segundo.AgregarAccion(dron.Nombre, AccionDron.EmitirLuz, dron.AlturaActual);
                            char letra = sistema.ObtenerLetra(dron.Nombre, dron.AlturaActual);
                            mensajeRecibido += letra;
                            Console.WriteLine($"Segundo {tiempo}: {dron.Nombre} emite '{letra}' a altura {dron.AlturaActual}");
                            cola.Desencolar(); // Eliminar instrucción procesada
                        }
                        else
                        {
                            // Moverse
                            if (dron.AlturaActual < instruccion.AlturaObjetivo)
                            {
                                dron.Subir();
                                segundo.AgregarAccion(dron.Nombre, AccionDron.Subir, dron.AlturaActual);
                                Console.WriteLine($"Segundo {tiempo}: {dron.Nombre} sube a {dron.AlturaActual}");
                            }
                            else if (dron.AlturaActual > instruccion.AlturaObjetivo)
                            {
                                dron.Bajar();
                                segundo.AgregarAccion(dron.Nombre, AccionDron.Bajar, dron.AlturaActual);
                                Console.WriteLine($"Segundo {tiempo}: {dron.Nombre} baja a {dron.AlturaActual}");
                            }
                        }
                    }
                }

                plan.AgregarSegundo(segundo);
                tiempo++;
                
                if (tiempo > 200) break;
            }

            plan.MensajeRecibido = mensajeRecibido;
            plan.TiempoOptimo = tiempo - 1;
            
            Console.WriteLine($"Plan completado en {plan.TiempoOptimo} segundos");
            Console.WriteLine($"Mensaje recibido: {mensajeRecibido}");
            
            return plan;
        }
    }
}