namespace Proyecto1
{
    public class Celda
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
        public int Estado { get; set; } // 0 = sana, 1 = contagiada

        public Celda(int fila, int columna, int estado)
        {
            Fila = fila;
            Columna = columna;
            Estado = estado;
        }
    }
}