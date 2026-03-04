namespace Proyecto1
{
    public class Paciente
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Periodos { get; set; }
        public int M { get; set; } // el tamaño de la rejilla mxm
        
        // aqui guardo todas las celdas de la rejilla usando mi lista enlazada propia
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