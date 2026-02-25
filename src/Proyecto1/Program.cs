using System;

namespace Proyecto1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Sistema Epidemiológico...");

            // probando la lista enlazada
            ListaEnlazada<int> miLista = new ListaEnlazada<int>();
            miLista.Insertar(10);
            miLista.Insertar(20);

            Console.WriteLine($"Tamaño de la lista: {miLista.Tamaño}");
        }
    }
}