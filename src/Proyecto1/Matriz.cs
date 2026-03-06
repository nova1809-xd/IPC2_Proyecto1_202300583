namespace Proyecto1
{
    public class Matriz<T>
    {
        public NodoMatriz<T> Cabeza { get; set; } // referencia a la esquina superior izquierda (0,0)
        public int M { get; set; } // tamaño de la matriz (M x M)
        private ListaEnlazada<NodoMatriz<T>> todosLosNodos { get; set; } // almacenar referencias para búsqueda rápida

        public Matriz(int m)
        {
            M = m;
            Cabeza = null;
            todosLosNodos = new ListaEnlazada<NodoMatriz<T>>();
        }

        // construir la matriz ortogonal completa sin usar arrays 2D
        public void Construir(T valorInicial)
        {
            // crear todos los nodos y guardarlos en lista para referencia rápida
            ListaEnlazada<ListaEnlazada<NodoMatriz<T>>> filas = new ListaEnlazada<ListaEnlazada<NodoMatriz<T>>>();
            
            // crear nodos fila por fila
            for (int i = 0; i < M; i++)
            {
                ListaEnlazada<NodoMatriz<T>> filaActual = new ListaEnlazada<NodoMatriz<T>>();
                for (int j = 0; j < M; j++)
                {
                    NodoMatriz<T> nuevoNodo = new NodoMatriz<T>(valorInicial, i, j);
                    filaActual.Insertar(nuevoNodo);
                    todosLosNodos.Insertar(nuevoNodo);
                }
                filas.Insertar(filaActual);
            }

            // conectar vecinos ortogonales y diagonales
            for (int i = 0; i < M; i++)
            {
                ListaEnlazada<NodoMatriz<T>> filaActual = filas.ObtenerPorIndice(i);
                
                for (int j = 0; j < M; j++)
                {
                    NodoMatriz<T> nodoActual = filaActual.ObtenerPorIndice(j);

                    // conectar Arriba
                    if (i > 0)
                    {
                        ListaEnlazada<NodoMatriz<T>> filaArriba = filas.ObtenerPorIndice(i - 1);
                        nodoActual.Arriba = filaArriba.ObtenerPorIndice(j);
                    }

                    // conectar Abajo
                    if (i < M - 1)
                    {
                        ListaEnlazada<NodoMatriz<T>> filaAbajo = filas.ObtenerPorIndice(i + 1);
                        nodoActual.Abajo = filaAbajo.ObtenerPorIndice(j);
                    }

                    // conectar Izquierda y Derecha dentro de la misma fila
                    if (j > 0)
                        nodoActual.Izquierda = filaActual.ObtenerPorIndice(j - 1);
                    if (j < M - 1)
                        nodoActual.Derecha = filaActual.ObtenerPorIndice(j + 1);

                    // conectar diagonales
                    if (i > 0 && j > 0)
                    {
                        ListaEnlazada<NodoMatriz<T>> filaArriba = filas.ObtenerPorIndice(i - 1);
                        nodoActual.ArribaIzquierda = filaArriba.ObtenerPorIndice(j - 1);
                    }
                    if (i > 0 && j < M - 1)
                    {
                        ListaEnlazada<NodoMatriz<T>> filaArriba = filas.ObtenerPorIndice(i - 1);
                        nodoActual.ArribaDerecha = filaArriba.ObtenerPorIndice(j + 1);
                    }
                    if (i < M - 1 && j > 0)
                    {
                        ListaEnlazada<NodoMatriz<T>> filaAbajo = filas.ObtenerPorIndice(i + 1);
                        nodoActual.AbajoIzquierda = filaAbajo.ObtenerPorIndice(j - 1);
                    }
                    if (i < M - 1 && j < M - 1)
                    {
                        ListaEnlazada<NodoMatriz<T>> filaAbajo = filas.ObtenerPorIndice(i + 1);
                        nodoActual.AbajoDerecha = filaAbajo.ObtenerPorIndice(j + 1);
                    }
                }
            }

            // guardar cabeza (0,0)
            Cabeza = filas.ObtenerPorIndice(0).ObtenerPorIndice(0);
        }

        // obtener el nodo en una posicion especifica
        private NodoMatriz<T> ObtenerNodo(int fila, int columna)
        {
            if (fila < 0 || fila >= M || columna < 0 || columna >= M)
                return null;

            NodoMatriz<T> actual = Cabeza;

            // recorrer hasta llegar a la fila correcta
            for (int i = 0; i < fila; i++)
            {
                actual = actual.Abajo;
                if (actual == null)
                    return null;
            }

            // recorrer hasta llegar a la columna correcta
            for (int j = 0; j < columna; j++)
            {
                actual = actual.Derecha;
                if (actual == null)
                    return null;
            }

            return actual;
        }

        // establecer el dato en una celda
        public void Establecer(int fila, int columna, T dato)
        {
            NodoMatriz<T> nodo = ObtenerNodo(fila, columna);
            if (nodo != null)
            {
                nodo.Dato = dato;
            }
        }

        // obtener el dato en una celda
        public T Obtener(int fila, int columna)
        {
            NodoMatriz<T> nodo = ObtenerNodo(fila, columna);
            if (nodo != null)
                return nodo.Dato;
            return default(T);
        }

        // obtener todos los 8 vecinos (Moore) de una celda
        public ListaEnlazada<T> ObtenerVecinos(int fila, int columna)
        {
            ListaEnlazada<T> vecinos = new ListaEnlazada<T>();
            NodoMatriz<T> nodo = ObtenerNodo(fila, columna);

            if (nodo == null)
                return vecinos;

            // agregar los 8 vecinos en orden (si existen)
            if (nodo.Arriba != null)
                vecinos.Insertar(nodo.Arriba.Dato);
            if (nodo.AbajoDerecha != null)
                vecinos.Insertar(nodo.AbajoDerecha.Dato);
            if (nodo.Derecha != null)
                vecinos.Insertar(nodo.Derecha.Dato);
            if (nodo.ArribaDerecha != null)
                vecinos.Insertar(nodo.ArribaDerecha.Dato);
            if (nodo.Abajo != null)
                vecinos.Insertar(nodo.Abajo.Dato);
            if (nodo.AbajoIzquierda != null)
                vecinos.Insertar(nodo.AbajoIzquierda.Dato);
            if (nodo.Izquierda != null)
                vecinos.Insertar(nodo.Izquierda.Dato);
            if (nodo.ArribaIzquierda != null)
                vecinos.Insertar(nodo.ArribaIzquierda.Dato);

            return vecinos;
        }

        // copiar la matriz completa (para guardar historial)
        public Matriz<T> Copiar()
        {
            Matriz<T> copia = new Matriz<T>(M);
            copia.Construir(default(T));

            // recorrer toda la matriz y copiar los datos
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    T dato = Obtener(i, j);
                    copia.Establecer(i, j, dato);
                }
            }

            return copia;
        }

        // recorrer toda la matriz y hacer algo con cada celda
        public void ParaCada(System.Action<T, int, int> accion)
        {
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    T dato = Obtener(i, j);
                    accion(dato, i, j);
                }
            }
        }
    }
}
