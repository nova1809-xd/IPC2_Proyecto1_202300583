using System;
using System.Xml.Linq;
using System.IO;

namespace Proyecto1
{
    class Program
    {
        // lista global donde guardo todos los pacientes que cargo del xml
        static ListaEnlazada<Paciente> pacientesCargados = new ListaEnlazada<Paciente>();

        static void Main(string[] args)
        {
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════╗");
                Console.WriteLine("║   SISTEMA EPIDEMIOLÓGICO - IPC2 Proyecto 1   ║");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("1. Cargar archivo XML de pacientes");
                Console.WriteLine("2. Ver pacientes cargados");
                Console.WriteLine("3. Limpiar memoria de pacientes");
                Console.WriteLine("4. Salir");
                Console.WriteLine();
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        CargarArchivoXML();
                        break;
                    case "2":
                        MostrarPacientesCargados();
                        break;
                    case "3":
                        LimpiarMemoriaPacientes();
                        break;
                    case "4":
                        salir = true;
                        Console.WriteLine("\n¡Hasta pronto!");
                        break;
                    default:
                        Console.WriteLine("\n❌ Opción inválida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CargarArchivoXML()
        {
            Console.Clear();
            Console.WriteLine("═══ CARGAR ARCHIVO XML ═══\n");
            Console.Write("Ingrese la ruta del archivo XML: ");
            string rutaArchivo = Console.ReadLine();

            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    Console.WriteLine("\n❌ El archivo no existe.");
                    Console.WriteLine("\n💡 Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }

                // cargar el archivo xml con linq
                XDocument doc = XDocument.Load(rutaArchivo);
                XElement raiz = doc.Element("pacientes");

                if (raiz == null)
                {
                    Console.WriteLine("\n❌ Formato XML inválido. No se encontró el elemento raíz <pacientes>.");
                    Console.WriteLine("\n💡 Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }

                int pacientesAgregados = 0;

                // recorrer cada paciente del xml
                foreach (XElement pacienteXml in raiz.Elements("paciente"))
                {
                    // extraer los datos personales del paciente
                    XElement datosPersonales = pacienteXml.Element("datospersonales");
                    string nombre = datosPersonales?.Element("nombre")?.Value ?? "Desconocido";
                    int edad = int.Parse(datosPersonales?.Element("edad")?.Value ?? "0");

                    // sacar los periodos y el tamaño de la rejilla
                    int periodos = int.Parse(pacienteXml.Element("periodos")?.Value ?? "0");
                    int m = int.Parse(pacienteXml.Element("m")?.Value ?? "0");

                    // crear el objeto paciente con los datos
                    Paciente paciente = new Paciente(nombre, edad, periodos, m);

                    // cargar las celdas de la rejilla (solo las que estan contagiadas)
                    XElement rejillaXml = pacienteXml.Element("rejilla");
                    if (rejillaXml != null)
                    {
                        foreach (XElement celdaXml in rejillaXml.Elements("celda"))
                        {
                            int fila = int.Parse(celdaXml.Attribute("f")?.Value ?? "0");
                            int columna = int.Parse(celdaXml.Attribute("c")?.Value ?? "0");
                            
                            // crear la celda como contagiada (estado = 1)
                            Celda celda = new Celda(fila, columna, 1);
                            paciente.Rejilla.Insertar(celda);
                        }
                    }

                    // meter el paciente a la lista global
                    pacientesCargados.Insertar(paciente);
                    pacientesAgregados++;
                }

                Console.WriteLine($"\n✓ Se cargaron {pacientesAgregados} paciente(s) exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error al cargar el archivo: {ex.Message}");
            }

            Console.WriteLine("\n💡 Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void MostrarPacientesCargados()
        {
            Console.Clear();
            Console.WriteLine("═══ PACIENTES CARGADOS EN MEMORIA ═══\n");

            if (pacientesCargados.EstaVacia())
            {
                Console.WriteLine("⚠️  No hay pacientes cargados en memoria.");
            }
            else
            {
                Console.WriteLine($"Total de pacientes: {pacientesCargados.Tamaño}\n");
                int indice = 1;

                pacientesCargados.ParaCada(paciente =>
                {
                    Console.WriteLine($"─────────────────────────────────────");
                    Console.WriteLine($"Paciente #{indice}");
                    Console.WriteLine($"  Nombre:    {paciente.Nombre}");
                    Console.WriteLine($"  Edad:      {paciente.Edad} años");
                    Console.WriteLine($"  Períodos:  {paciente.Periodos}");
                    Console.WriteLine($"  Rejilla:   {paciente.M}x{paciente.M}");
                    Console.WriteLine($"  Celdas contagiadas: {paciente.Rejilla.Tamaño}");
                    indice++;
                });

                Console.WriteLine($"─────────────────────────────────────");
            }

            Console.WriteLine("\n💡 Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void LimpiarMemoriaPacientes()
        {
            Console.Clear();
            Console.WriteLine("═══ LIMPIAR MEMORIA ═══\n");
            
            if (pacientesCargados.EstaVacia())
            {
                Console.WriteLine("⚠️  No hay pacientes en memoria para limpiar.");
            }
            else
            {
                Console.Write($"¿Está seguro de eliminar {pacientesCargados.Tamaño} paciente(s) de memoria? (S/N): ");
                string confirmacion = Console.ReadLine()?.ToUpper();

                if (confirmacion == "S")
                {
                    pacientesCargados.Limpiar();
                    Console.WriteLine("\n✓ Memoria limpiada exitosamente.");
                }
                else
                {
                    Console.WriteLine("\n❌ Operación cancelada.");
                }
            }

            Console.WriteLine("\n💡 Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}