namespace Proyecto1
{
    public class Matriz<T>
    {
        public NodoMatriz<T> Cabeza { get; set; } // referencia a la esquina superior izquierda (0,0)
        public int M { get; set; } // tamaño de la matriz (M x M)

        public Matriz(int m)
        {
            M = m;
            Cabeza = null;
        }

        // construir la matriz ortogonal completa
        public void Construir(T valorInicial)
        {
            // crear todos los nodos
            NodoMatriz<T>[,] nodos = new NodoMatriz<T>[M, M];

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    nodos[i, j] = new NodoMatriz<T>(valorInicial, i, j);
                }
            }

            // conectar vecinos ortogonales (Arriba, Abajo, Izquierda, Derecha)
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (i > 0)
                        nodos[i, j].Arriba = nodos[i - 1, j];
                    if (i < M - 1)
                        nodos[i, j].Abajo = nodos[i + 1, j];
                    if (j > 0)
                        nodos[i, j].Izquierda = nodos[i, j - 1];
                    if (j < M - 1)
                        nodos[i, j].Derecha = nodos[i, j + 1];

                    // conectar vecinos diagonales (Moore)
                    if (i > 0 && j > 0)
                        nodos[i, j].ArribaIzquierda = nodos[i - 1, j - 1];
                    if (i > 0 && j < M - 1)
                        nodos[i, j].ArribaDerecha = nodos[i - 1, j + 1];
                    if (i < M - 1 && j > 0)
                        nodos[i, j].AbajoIzquierda = nodos[i + 1, j - 1];
                    if (i < M - 1 && j < M - 1)
                        nodos[i, j].AbajoDerecha = nodos[i + 1, j + 1];
                }
            }

            // guardar referencia a la cabeza (esquina superior izquierda)
            Cabeza = nodos[0, 0];
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
