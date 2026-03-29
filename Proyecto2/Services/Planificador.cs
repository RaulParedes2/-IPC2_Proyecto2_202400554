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

            // Obtener lista de drones
            ListaDrones dronesSistema = sistema.Drones;
            int cantidadDrones = dronesSistema.Count;

            // Nombres de drones
            string[] nombresDrones = new string[cantidadDrones];
            int idx = 0;
            NodoDron? dronActual = dronesSistema.GetPrimero();
            while (dronActual != null)
            {
                nombresDrones[idx] = dronActual.Data.Nombre;
                idx++;
                dronActual = dronActual.Siguiente;
            }

            // Altura actual de cada dron (comienzan en 1)
            int[] alturasActuales = new int[cantidadDrones];
            for (int i = 0; i < cantidadDrones; i++)
            {
                alturasActuales[i] = 1;
            }

            // Crear lista de instrucciones en orden (usar lista enlazada)
            ListaInstruccionesEmision instruccionesLista = new ListaInstruccionesEmision();
            NodoInstruccionEmision? instActual = mensaje.Instrucciones.GetPrimero();
            while (instActual != null)
            {
                instruccionesLista.Agregar(instActual.Data);
                Console.WriteLine($"Instrucción {instruccionesLista.Count}: {instActual.Data.NombreDron} a altura {instActual.Data.AlturaObjetivo}");
                instActual = instActual.Siguiente;
            }

            PlanVuelo plan = new PlanVuelo();
            plan.NombreMensaje = mensaje.Nombre;
            plan.NombreSistema = sistema.Nombre;
            plan.MensajeOriginal = mensaje.TextoOriginal;

            int tiempo = 1;
            string mensajeRecibido = "";
            int instruccionIndex = 0;

            // Estado de cada dron
            int[] objetivoActual = new int[cantidadDrones];
            bool[] ocupado = new bool[cantidadDrones];
            for (int i = 0; i < cantidadDrones; i++)
            {
                objetivoActual[i] = 1;
                ocupado[i] = false;
            }

            while (instruccionIndex < instruccionesLista.Count || HayDronesOcupados(ocupado, cantidadDrones))
            {
                PlanVuelo.AccionPorSegundo segundo = new PlanVuelo.AccionPorSegundo(tiempo);

                // Inicializar todos con Esperar
                for (int i = 0; i < cantidadDrones; i++)
                {
                    segundo.AgregarAccion(nombresDrones[i], "Esperar", alturasActuales[i]);
                }

                // Asignar nueva instrucción si hay un dron libre
                if (instruccionIndex < instruccionesLista.Count)
                {
                    InstruccionEmision? siguienteInst = instruccionesLista.ObtenerPorIndice(instruccionIndex);
                    if (siguienteInst != null)
                    {
                        int dronIdx = ObtenerIndiceDron(nombresDrones, siguienteInst.NombreDron);
                        if (dronIdx >= 0 && !ocupado[dronIdx])
                        {
                            objetivoActual[dronIdx] = siguienteInst.AlturaObjetivo;
                            ocupado[dronIdx] = true;
                            instruccionIndex++;
                            Console.WriteLine($"Asignando instrucción {instruccionIndex}: {siguienteInst.NombreDron} a altura {siguienteInst.AlturaObjetivo}");
                        }
                    }
                }

                // Procesar movimientos y emisiones
                for (int i = 0; i < cantidadDrones; i++)
                {
                    if (ocupado[i])
                    {
                        if (alturasActuales[i] == objetivoActual[i])
                        {
                            // Emitir luz
                            segundo.ReemplazarAccion(nombresDrones[i], "Emitir Luz", alturasActuales[i]);
                            char letra = sistema.ObtenerLetra(nombresDrones[i], alturasActuales[i]);
                            mensajeRecibido += letra;
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} emite '{letra}' a altura {alturasActuales[i]}");
                            ocupado[i] = false;
                        }
                        else if (alturasActuales[i] < objetivoActual[i])
                        {
                            alturasActuales[i]++;
                            segundo.ReemplazarAccion(nombresDrones[i], "Subir", alturasActuales[i]);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} sube a {alturasActuales[i]}");
                        }
                        else if (alturasActuales[i] > objetivoActual[i])
                        {
                            alturasActuales[i]--;
                            segundo.ReemplazarAccion(nombresDrones[i], "Bajar", alturasActuales[i]);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} baja a {alturasActuales[i]}");
                        }
                    }
                }

                plan.AgregarSegundo(segundo);
                tiempo++;

                if (tiempo > 500) break;
            }

            plan.MensajeRecibido = mensajeRecibido;
            plan.TiempoOptimo = tiempo - 1;

            Console.WriteLine($"Plan completado en {plan.TiempoOptimo} segundos");
            Console.WriteLine($"Mensaje recibido: {mensajeRecibido}");

            return plan;
        }

        private int ObtenerIndiceDron(string[] nombres, string nombreDron)
        {
            for (int i = 0; i < nombres.Length; i++)
            {
                if (nombres[i] == nombreDron)
                    return i;
            }
            return -1;
        }

        private bool HayDronesOcupados(bool[] ocupado, int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (ocupado[i])
                    return true;
            }
            return false;
        }
    }
}