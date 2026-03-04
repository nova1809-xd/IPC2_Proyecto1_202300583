namespace Proyecto1
{
    public class NodoMatriz<T>
    {
        public T Dato { get; set; }
        public int Fila { get; set; }
        public int Columna { get; set; }

        // apuntadores a los 4 vecinos ortogonales
        public NodoMatriz<T> Arriba { get; set; }
        public NodoMatriz<T> Abajo { get; set; }
        public NodoMatriz<T> Izquierda { get; set; }
        public NodoMatriz<T> Derecha { get; set; }

        // apuntadores a los 4 vecinos diagonales (para Moore)
        public NodoMatriz<T> ArribaIzquierda { get; set; }
        public NodoMatriz<T> ArribaDerecha { get; set; }
        public NodoMatriz<T> AbajoIzquierda { get; set; }
        public NodoMatriz<T> AbajoDerecha { get; set; }

        public NodoMatriz(T dato, int fila, int columna)
        {
            Dato = dato;
            Fila = fila;
            Columna = columna;
            Arriba = null;
            Abajo = null;
            Izquierda = null;
            Derecha = null;
            ArribaIzquierda = null;
            ArribaDerecha = null;
            AbajoIzquierda = null;
            AbajoDerecha = null;
        }
    }
}
