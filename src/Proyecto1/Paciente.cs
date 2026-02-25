namespace Proyecto1
{
    public class Paciente
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Periodos { get; set; }
        public int M { get; set; } // tamaño de la rejilla mxm
        
        // se usa una lista enlazada para representar la rejilla de celdas
        public ListaEnlazada<Celda> Rejilla { get; set; }

        public Paciente(string nombre, int edad, int periodos, int m)
        {
            Nombre = nombre;
            Edad = edad;
            Periodos = periodos;
            M = m;
            Rejilla = new ListaEnlazada<Celda>();
        }
    }
}