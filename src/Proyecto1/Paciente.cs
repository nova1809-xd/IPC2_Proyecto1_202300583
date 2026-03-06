namespace Proyecto1
{
    public class Paciente
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Periodos { get; set; }
        public int M { get; set; } // tamaño de la rejilla MxM
        
        // almacenar celdas contagiadas usando la lista enlazada propia
        public ListaEnlazada<Celda> Rejilla { get; set; }

        public Paciente(string id, string nombre, int edad, int periodos, int m)
        {
            Id = id;
            Nombre = nombre;
            Edad = edad;
            Periodos = periodos;
            M = m;
            Rejilla = new ListaEnlazada<Celda>();
        }
    }
}