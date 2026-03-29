using System;
using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class GestorSistemas
    {
        private ListaSistemasDrones sistemas;

        public GestorSistemas()
        {
            sistemas = new ListaSistemasDrones();
        }

        public ListaSistemasDrones ObtenerTodos()
        {
            return sistemas;
        }

        public SistemaDrones? ObtenerPorNombre(string nombre)
        {
            return sistemas.ObtenerPorNombre(nombre);
        }

        public bool AgregarSistema(string nombre)
        {
            if (sistemas.Existe(nombre))
                return false;

            sistemas.Agregar(new SistemaDrones(nombre));
            Console.WriteLine($"Sistema agregado: {nombre}");
            return true;
        }

        public bool AgregarDronASistema(string nombreSistema, string nombreDron)
        {
            SistemaDrones? sistema = sistemas.ObtenerPorNombre(nombreSistema);
            if (sistema == null)
            {
                Console.WriteLine($"Sistema {nombreSistema} no encontrado");
                return false;
            }

            if (sistema.Drones.Existe(nombreDron))
            {
                Console.WriteLine($"Dron {nombreDron} ya existe en sistema {nombreSistema}");
                return false;
            }

            Dron? dron = new Dron(nombreDron);
            sistema.AgregarDron(dron);
            Console.WriteLine($"Dron {nombreDron} agregado al sistema {nombreSistema}");
            return true;
        }

        public bool ConfigurarCodificacion(string nombreSistema, string nombreDron, int altura, char letra)
        {
            SistemaDrones? sistema = sistemas.ObtenerPorNombre(nombreSistema);
            if (sistema == null)
            {
                Console.WriteLine($"Sistema {nombreSistema} no encontrado");
                return false;
            }

            if (!sistema.Drones.Existe(nombreDron))
            {
                Console.WriteLine($"Dron {nombreDron} no existe en sistema {nombreSistema}");
                return false;
            }

            sistema.ConfigurarCodificacion(nombreDron, altura, letra);
            Console.WriteLine($"Codificacion configurada: {nombreDron} @ {altura}m = '{letra}'");
            return true;
        }
        public bool ExisteSistema(string nombre)
        {
            return sistemas.Existe(nombre);
        }

        public bool Eliminar(string nombre)
        {
            return sistemas.Eliminar(nombre);
        }

        public int CantidadSistemas()
        {
            return sistemas.Count;
        }
    }
}