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

        // agregar un elemento al final
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

        // obtener elemento por posición (índice)
        public T ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= Tamaño)
            {
                throw new IndexOutOfRangeException("Índice fuera de rango.");
            }

            int posicion = 0;
            Nodo<T> actual = Cabeza;
            while (actual != null)
            {
                if (posicion == indice)
                {
                    return actual.Dato;
                }
                actual = actual.Siguiente;
                posicion++;
            }

            throw new IndexOutOfRangeException("Índice fuera de rango.");
        }

        // limpiar la lista (resetear)
        public void Limpiar()
        {
            Cabeza = null;
            Tamaño = 0;
        }

        // verificar si está vacía
        public bool EstaVacia()
        {
            return Cabeza == null;
        }

        // recorrer y ejecutar acción en cada elemento
        public void ParaCada(Action<T> accion)
        {
            Nodo<T> actual = Cabeza;
            while (actual != null)
            {
                accion(actual.Dato);
                actual = actual.Siguiente;
            }
        }

        // buscar el primer elemento que cumpla la condición
        public T BuscarPrimero(Predicate<T> condicion)
        {
            Nodo<T> actual = Cabeza;
            while (actual != null)
            {
                if (condicion(actual.Dato))
                {
                    return actual.Dato;
                }
                actual = actual.Siguiente;
            }
            return default(T);
        }

        // verificar si existe algún elemento con la condición
        public bool Existe(Predicate<T> condicion)
        {
            Nodo<T> actual = Cabeza;
            while (actual != null)
            {
                if (condicion(actual.Dato))
                {
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }
    }
}