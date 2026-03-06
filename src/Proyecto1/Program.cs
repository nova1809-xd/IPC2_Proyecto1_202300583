using System;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;

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
                Console.WriteLine("SISTEMA EPIDEMIOLÓGICO - IPC2 Proyecto 1");
                Console.WriteLine();
                Console.WriteLine("1. Cargar archivo XML de pacientes");
                Console.WriteLine("2. Ver pacientes cargados");
                Console.WriteLine("3. Analizar paciente (Simulación)");
                Console.WriteLine("4. Exportar resultados a XML");
                Console.WriteLine("5. Generar visualización Graphviz de TDA");
                Console.WriteLine("6. Limpiar memoria de pacientes");
                Console.WriteLine("7. Salir");
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
                        AnalizarPaciente();
                        break;
                    case "4":
                        ExportarResultadosXML();
                        break;
                    case "5":
                        GenerarGraphvizTDA();
                        break;
                    case "6":
                        LimpiarMemoriaPacientes();
                        break;
                    case "7":
                        salir = true;
                        Console.WriteLine("\n¡Hasta pronto!");
                        break;
                    default:
                        Console.WriteLine("\nOpción inválida.");
                        break;
                }
            }
        }

        static void CargarArchivoXML()
        {
            Console.Clear();
            Console.WriteLine("CARGAR ARCHIVO XML\n");
            Console.Write("Ingrese la ruta del archivo XML: ");
            string rutaArchivo = Console.ReadLine();

            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    Console.WriteLine("\nEl archivo no existe.");
                    return;
                }

                // cargar el archivo xml con linq
                XDocument doc = XDocument.Load(rutaArchivo);
                XElement raiz = doc.Element("pacientes");

                if (raiz == null)
                {
                    Console.WriteLine("\nFormato XML inválido. No se encontró el elemento raíz <pacientes>.");
                    return;
                }

                int pacientesAgregados = 0;

                // recorrer cada paciente del xml
                foreach (XElement pacienteXml in raiz.Elements("paciente"))
                {
                    // extraer los datos personales del paciente
                    XElement datosPersonales = pacienteXml.Element("datospersonales");
                    string id = datosPersonales?.Element("id")?.Value ?? "SIN_ID";
                    string nombre = datosPersonales?.Element("nombre")?.Value ?? "Desconocido";
                    int edad = int.Parse(datosPersonales?.Element("edad")?.Value ?? "0");

                    // sacar los periodos y el tamaño de la rejilla
                    int periodos = int.Parse(pacienteXml.Element("periodos")?.Value ?? "0");
                    int m = int.Parse(pacienteXml.Element("m")?.Value ?? "0");

                    // crear el objeto paciente con los datos
                    Paciente paciente = new Paciente(id, nombre, edad, periodos, m);

                    // cargar las celdas de la rejilla (solo las que estan contagiadas)
                    XElement rejillaXml = pacienteXml.Element("rejilla");
                    if (rejillaXml != null)
                    {
                        foreach (XElement celdaXml in rejillaXml.Elements("celda"))
                        {
                            // leer atributos f y c del XML (base-1, convertir a base-0)
                            int fila = int.Parse(celdaXml.Attribute("f")?.Value ?? "1") - 1;
                            int columna = int.Parse(celdaXml.Attribute("c")?.Value ?? "1") - 1;
                            
                            // crear la celda como contagiada (estado = 1)
                            Celda celda = new Celda(fila, columna, 1);
                            paciente.Rejilla.Insertar(celda);
                        }
                    }

                    // meter el paciente a la lista global
                    pacientesCargados.Insertar(paciente);
                    pacientesAgregados++;
                }

                Console.WriteLine($"\nSe cargaron {pacientesAgregados} paciente(s) exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al cargar el archivo: {ex.Message}");
            }
            
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        static void MostrarPacientesCargados()
        {
            Console.Clear();
            Console.WriteLine("PACIENTES CARGADOS EN MEMORIA\n");

            if (pacientesCargados.EstaVacia())
            {
                Console.WriteLine("No hay pacientes cargados en memoria.");
            }
            else
            {
                Console.WriteLine($"Total de pacientes: {pacientesCargados.Tamaño}\n");
                int indice = 1;

                pacientesCargados.ParaCada(paciente =>
                {
                    Console.WriteLine($"Paciente #{indice}");
                    Console.WriteLine($"  Nombre:    {paciente.Nombre}");
                    Console.WriteLine($"  Edad:      {paciente.Edad} años");
                    Console.WriteLine($"  Períodos:  {paciente.Periodos}");
                    Console.WriteLine($"  Rejilla:   {paciente.M}x{paciente.M}");
                    Console.WriteLine($"  Celdas contagiadas: {paciente.Rejilla.Tamaño}");
                    Console.WriteLine();
                    indice++;
                });
            }
            
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        static void LimpiarMemoriaPacientes()
        {
            Console.Clear();
            Console.WriteLine("LIMPIAR MEMORIA\n");
            
            if (pacientesCargados.EstaVacia())
            {
                Console.WriteLine("No hay pacientes en memoria para limpiar.");
            }
            else
            {
                Console.Write($"¿Está seguro de eliminar {pacientesCargados.Tamaño} paciente(s) de memoria? (S/N): ");
                string confirmacion = Console.ReadLine()?.ToUpper();

                if (confirmacion == "S")
                {
                    pacientesCargados.Limpiar();
                    Console.WriteLine("\nMemoria limpiada exitosamente.");
                }
                else
                {
                    Console.WriteLine("\nOperación cancelada.");
                }
            }
            
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        static void AnalizarPaciente()
        {
            Console.Clear();
            Console.WriteLine("ANALIZAR PACIENTE\n");

            if (pacientesCargados.EstaVacia())
            {
                Console.WriteLine("No hay pacientes cargados. Primero carga el archivo XML.");

                return;
            }

            // mostrar lista de pacientes
            Console.WriteLine("Pacientes disponibles:\n");
            int indice = 1;
            pacientesCargados.ParaCada(paciente =>
            {
                Console.WriteLine($"{indice}. {paciente.Nombre} (Edad: {paciente.Edad}, Rejilla: {paciente.M}x{paciente.M})");
                indice++;
            });

            Console.Write("\nSeleccione el número del paciente: ");
            if (!int.TryParse(Console.ReadLine(), out int seleccion) || seleccion < 1 || seleccion > pacientesCargados.Tamaño)
            {
                Console.WriteLine("\nSelección inválida.");
                return;
            }

            Paciente pacienteSeleccionado = pacientesCargados.ObtenerPorIndice(seleccion - 1);

            Console.WriteLine("\n¿Cómo desea ejecutar la simulación?");
            Console.WriteLine("1. Paso a paso (manual)");
            Console.WriteLine("2. Automático (completo)");
            Console.Write("\nOpción: ");
            string modoSimulacion = Console.ReadLine();

            if (modoSimulacion == "1")
            {
                SimularPasoAPaso(pacienteSeleccionado);
            }
            else if (modoSimulacion == "2")
            {
                SimularAutomatico(pacienteSeleccionado);
            }
            else
            {
                Console.WriteLine("\nOpción inválida.");
                Console.Write("\nPresione Enter para continuar...");
                Console.ReadLine();
            }
        }

        static void SimularPasoAPaso(Paciente paciente)
        {
            Automata automata = new Automata(paciente.M);
            automata.Inicializar(paciente);

            bool continuar = true;
            int periodo = 0;

            while (continuar && periodo < paciente.Periodos)
            {
                Console.Clear();
                Console.WriteLine($"SIMULACIÓN: {paciente.Nombre} - Período {automata.PeriodoActual}\n");

                // mostrar dashboard
                MostrarDashboard(automata);

                // mostrar rejilla
                Console.WriteLine("\nRejilla:");
                MostrarMatriz(automata.EstadoActual, paciente.M);

                // evaluar diagnóstico
                DiagnosticoResultado resultado = automata.EvaluarDiagnostico();
                
                if (resultado.Diagnostico != "Leve")
                {
                    Console.WriteLine($"\nDIAGNÓSTICO DETECTADO: {resultado.Diagnostico}");
                    if (resultado.N > 0)
                        Console.WriteLine($"   Patrón inicial se repite en período N = {resultado.N}");
                    if (resultado.N1 > 0)
                        Console.WriteLine($"   Patrón secundario se repite cada N1 = {resultado.N1} período(s)");

                    break;
                }

                Console.WriteLine("\n[ENTER] Siguiente período | [Q] Salir");
                string tecla = Console.ReadLine();

                if (tecla?.ToUpper() == "Q")
                {
                    continuar = false;
                }
                else
                {
                    // calcular siguiente período
                    automata.SimularPeriodo();
                    periodo++;

                    // generar grafo del período exacto recién calculado
                    string nombreBase = paciente.Nombre.Replace(" ", "_");
                    string nombreDot = $"{nombreBase}_p{automata.PeriodoActual}.dot";
                    string nombrePng = $"{nombreBase}_p{automata.PeriodoActual}.png";
                    string contenidoDot = GenerarContenidoDot(automata.EstadoActual, paciente.M, automata.PeriodoActual);
                    File.WriteAllText(nombreDot, contenidoDot);
                    EjecutarGraphviz(nombreDot, nombrePng);

                    // imprimir contadores del período actual
                    Console.WriteLine($"\nPeríodo actual: {automata.PeriodoActual}");
                    Console.WriteLine($"Celdas sanas: {automata.CeldasSanas}");
                    Console.WriteLine($"Celdas contagiadas: {automata.CeldasContagiadas}");
                    Console.WriteLine($"Gráfica generada: {nombrePng}");
                    Console.Write("\nPresione Enter para continuar...");
                    Console.ReadLine();
                }
            }

            if (periodo >= paciente.Periodos)
            {
                Console.WriteLine("\nSimulación completada. El paciente tiene una enfermedad LEVE (no se detectó patrón repetitivo).");
            }
        }

        static void SimularAutomatico(Paciente paciente)
        {
            Console.Clear();
            Console.WriteLine($"SIMULACIÓN AUTOMÁTICA: {paciente.Nombre}\n");

            Automata automata = new Automata(paciente.M);
            automata.Inicializar(paciente);

            DiagnosticoResultado resultado = automata.EvaluarDiagnostico();

            for (int p = 0; p < paciente.Periodos; p++)
            {
                // calcular siguiente período
                automata.SimularPeriodo();

                // generar grafo del período exacto recién calculado
                string nombreBase = paciente.Nombre.Replace(" ", "_");
                string nombreDot = $"{nombreBase}_p{automata.PeriodoActual}.dot";
                string nombrePng = $"{nombreBase}_p{automata.PeriodoActual}.png";
                string contenidoDot = GenerarContenidoDot(automata.EstadoActual, paciente.M, automata.PeriodoActual);
                File.WriteAllText(nombreDot, contenidoDot);
                EjecutarGraphviz(nombreDot, nombrePng);

                // imprimir contadores del período actual
                Console.WriteLine($"Período actual: {automata.PeriodoActual}");
                Console.WriteLine($"Celdas sanas: {automata.CeldasSanas}");
                Console.WriteLine($"Celdas contagiadas: {automata.CeldasContagiadas}");
                Console.WriteLine($"Gráfica generada: {nombrePng}\n");

                resultado = automata.EvaluarDiagnostico();
                if (resultado.Diagnostico != "Leve")
                {
                    resultado.PeriodoFinal = automata.PeriodoActual;
                    break;
                }
            }

            if (resultado.Diagnostico == "Leve")
            {
                resultado.PeriodoFinal = automata.PeriodoActual;
            }

            Console.WriteLine("RESULTADO");
            Console.WriteLine($"Paciente: {paciente.Nombre}");
            Console.WriteLine($"Edad: {paciente.Edad} años");
            Console.WriteLine($"Rejilla: {paciente.M}x{paciente.M}");
            Console.WriteLine($"Períodos simulados: {resultado.PeriodoFinal}");
            Console.WriteLine();
            Console.WriteLine($"DIAGNÓSTICO: {resultado.Diagnostico.ToUpper()}");
            
            if (resultado.N > 0)
                Console.WriteLine($"   N = {resultado.N} (Patrón inicial se repite)");
            if (resultado.N1 > 0)
                Console.WriteLine($"   N1 = {resultado.N1} (Patrón secundario se repite cada {resultado.N1} período(s))");

            Console.WriteLine($"\nEstado final:");
            Console.WriteLine($"  Celdas sanas: {automata.CeldasSanas}");
            Console.WriteLine($"  Celdas contagiadas: {automata.CeldasContagiadas}");
            
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        static void MostrarDashboard(Automata automata)
        {
            Console.WriteLine($"Período actual: {automata.PeriodoActual}");
            Console.WriteLine($"Celdas sanas: {automata.CeldasSanas}");
            Console.WriteLine($"Celdas contagiadas: {automata.CeldasContagiadas}");
        }

        static void MostrarMatriz(Matriz<int> matriz, int m)
        {
            // para rejillas grandes, mostrar solo una version reducida
            if (m > 20)
            {
                Console.WriteLine($"[Rejilla {m}x{m} - demasiado grande para mostrar en consola]");
                return;
            }

            Console.Write("   ");
            for (int j = 0; j < m; j++)
            {
                Console.Write($"{j % 10} ");
            }
            Console.WriteLine();

            for (int i = 0; i < m; i++)
            {
                Console.Write($"{i % 10:00} ");
                for (int j = 0; j < m; j++)
                {
                    int valor = matriz.Obtener(i, j);
                    Console.Write(valor == 1 ? "█ " : "· ");
                }
                Console.WriteLine();
            }
        }

        static void ExportarResultadosXML()
        {
            Console.Clear();
            Console.WriteLine("EXPORTAR RESULTADOS A XML\n");

            if (pacientesCargados.EstaVacia())
            {
                Console.WriteLine("No hay pacientes para analizar.");
                return;
            }

            Console.Write("Ingrese el nombre del archivo de salida (ej: resultados.xml): ");
            string nombreArchivo = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nombreArchivo))
            {
                nombreArchivo = "resultados.xml";
            }

            try
            {
                XDocument docSalida = new XDocument();
                XElement raizSalida = new XElement("pacientes");

                pacientesCargados.ParaCada(paciente =>
                {
                    // simular el paciente
                    Automata automata = new Automata(paciente.M);
                    DiagnosticoResultado resultado = automata.SimularCompleto(paciente);

                    // crear elemento paciente
                    XElement pacienteXml = new XElement("paciente");

                    // datos personales
                    XElement datosPersonales = new XElement("datospersonales",
                        new XElement("id", paciente.Id),
                        new XElement("nombre", paciente.Nombre),
                        new XElement("edad", paciente.Edad)
                    );
                    pacienteXml.Add(datosPersonales);

                    // agregar resultado
                    pacienteXml.Add(new XElement("resultado", resultado.Diagnostico.ToLower()));

                    // agregar N si aplica
                    if (resultado.N > 0)
                    {
                        pacienteXml.Add(new XElement("n", resultado.N));
                    }

                    // agregar N1 si aplica
                    if (resultado.N1 > 0)
                    {
                        pacienteXml.Add(new XElement("n1", resultado.N1));
                    }

                    raizSalida.Add(pacienteXml);
                });

                docSalida.Add(raizSalida);
                docSalida.Save(nombreArchivo);

                Console.WriteLine($"\nArchivo '{nombreArchivo}' generado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al exportar: {ex.Message}");
            }
            
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        static void GenerarGraphvizTDA()
        {
            Console.Clear();
            Console.WriteLine("GENERAR VISUALIZACIÓN GRAPHVIZ DE TDA\n");

            if (pacientesCargados.EstaVacia())
            {
                Console.WriteLine("No hay pacientes cargados. Primero carga el archivo XML.");

                return;
            }

            // mostrar lista de pacientes
            Console.WriteLine("Pacientes disponibles:\n");
            int indice = 1;
            pacientesCargados.ParaCada(paciente =>
            {
                Console.WriteLine($"{indice}. {paciente.Nombre} (Edad: {paciente.Edad}, Rejilla: {paciente.M}x{paciente.M})");
                indice++;
            });

            Console.Write("\nSeleccione el número del paciente: ");
            if (!int.TryParse(Console.ReadLine(), out int seleccion) || seleccion < 1 || seleccion > pacientesCargados.Tamaño)
            {
                Console.WriteLine("\nSelección inválida.");
                return;
            }

            Paciente pacienteSeleccionado = pacientesCargados.ObtenerPorIndice(seleccion - 1);

            Console.Write("\n¿En qué período desea visualizar la matriz? (0 para estado inicial): ");
            if (!int.TryParse(Console.ReadLine(), out int periodo) || periodo < 0)
            {
                Console.WriteLine("\nPeríodo inválido.");
                return;
            }

            // simular el paciente hasta el período deseado
            Automata automata = new Automata(pacienteSeleccionado.M);
            automata.Inicializar(pacienteSeleccionado);

            for (int p = 0; p < periodo; p++)
            {
                automata.SimularPeriodo();
            }

            // generar el archivo .dot
            string nombreArchivo = $"rejilla_p{periodo}.dot";
            string nombreImagen = $"rejilla_p{periodo}.png";

            try
            {
                // construir el contenido DOT usando nuestra propia lógica sin arrays nativos
                string contenidoDot = GenerarContenidoDot(automata.EstadoActual, pacienteSeleccionado.M, periodo);

                // guardar el archivo .dot
                File.WriteAllText(nombreArchivo, contenidoDot);
                Console.WriteLine($"\nArchivo '{nombreArchivo}' generado exitosamente.");

                // intentar ejecutar el comando dot para generar la imagen
                bool imagenGenerada = EjecutarGraphviz(nombreArchivo, nombreImagen);

                if (imagenGenerada)
                {
                    Console.WriteLine($"Imagen '{nombreImagen}' generada exitosamente.");
                    Console.WriteLine("\n¿Desea abrir la imagen? (S/N): ");
                    string respuesta = Console.ReadLine()?.ToUpper();
                    
                    if (respuesta == "S")
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = nombreImagen,
                                UseShellExecute = true
                            });
                        }
                        catch
                        {
                            Console.WriteLine("No se pudo abrir la imagen automáticamente.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nNo se pudo generar la imagen automáticamente.");
                    Console.WriteLine($"Ejecute manualmente: dot -Tpng {nombreArchivo} -o {nombreImagen}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError al generar Graphviz: {ex.Message}");
            }
            
            Console.Write("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        static string GenerarContenidoDot(Matriz<int> matriz, int m, int periodo)
        {
            // usar nuestra propia ListaEnlazada para construir el contenido
            ListaEnlazada<string> lineas = new ListaEnlazada<string>();

            lineas.Insertar("digraph Matriz {");
            lineas.Insertar("    rankdir=TB;");
            lineas.Insertar("    node [shape=square, width=0.6, height=0.6, fixedsize=true, fontsize=10, style=filled];");
            lineas.Insertar($"    labelloc=\"t\";");
            lineas.Insertar($"    label=\"Estado de la Matriz - Período {periodo}\";");
            lineas.Insertar("    fontsize=18;");
            lineas.Insertar("    fontname=\"Arial Bold\";");
            lineas.Insertar("    splines=false;");
            lineas.Insertar("    nodesep=0.2;");
            lineas.Insertar("    ranksep=0.2;");
            lineas.Insertar("");

            // generar nodos organizados por filas
            for (int i = 0; i < m; i++)
            {
                lineas.Insertar($"    // Fila {i}");
                lineas.Insertar("    {");
                lineas.Insertar("        rank=same;");
                
                for (int j = 0; j < m; j++)
                {
                    int valor = matriz.Obtener(i, j);
                    string color = valor == 1 ? "dodgerblue" : "white";
                    string borde = "black";
                    string linea = $"        n_{i}_{j} [label=\"\", fillcolor=\"{color}\", color=\"{borde}\", penwidth=1.5];";
                    lineas.Insertar(linea);
                }
                
                lineas.Insertar("    }");
            }

            lineas.Insertar("");
            lineas.Insertar("    // Conexiones horizontales (mantener nodos en fila)");

            // conectar horizontalmente para mantener estructura de matriz
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m - 1; j++)
                {
                    string linea = $"    n_{i}_{j} -> n_{i}_{j + 1} [style=invis, weight=10];";
                    lineas.Insertar(linea);
                }
            }

            lineas.Insertar("");
            lineas.Insertar("    // Conexiones verticales (mantener columnas alineadas)");

            // conectar verticalmente
            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < m - 1; i++)
                {
                    string linea = $"    n_{i}_{j} -> n_{i + 1}_{j} [style=invis, weight=10];";
                    lineas.Insertar(linea);
                }
            }

            lineas.Insertar("}");

            // concatenar todas las líneas sin usar string[] o List
            string resultado = "";
            lineas.ParaCada(linea =>
            {
                resultado += linea + "\n";
            });

            return resultado;
        }

        static bool EjecutarGraphviz(string archivoEntrada, string archivoSalida)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dot",
                    Arguments = $"-Tpng \"{archivoEntrada}\" -o \"{archivoSalida}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process processo = Process.Start(startInfo))
                {
                    processo.WaitForExit(10000); // esperar máximo 10 segundos
                    return processo.ExitCode == 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}