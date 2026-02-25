using System;

namespace Proyecto1
{
    public class ListaEnlazada<T>
    {
        public Nodo<T> Cabeza { get; private set; }
        public int Tamaño { get; private set; }

        public ListaEnlazada()
        {
            Cabeza = null;
            Tamaño = 0;
        }

        // Método para agregar al final
        public void Insertar(T dato)
        {
            Nodo<T> nuevoNodo = new Nodo<T>(dato);

            if (Cabeza == null)
            {
                Cabeza = nuevoNodo;
            }
            else
            {
                Nodo<T> actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
            Tamaño++;
        }
    }
}