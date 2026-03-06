namespace Proyecto1
{
    public class Automata
    {
        public Matriz<int> EstadoActual { get; set; } // estado actual de la rejilla
        public ListaEnlazada<Matriz<int>> Historico { get; set; } // guardar cada período para detectar patrones
        public int PeriodoActual { get; set; }
        public int M { get; set; } // tamaño de la rejilla
        public int CeldasContagiadas { get; set; } // contar celdas contagiadas en el período actual
        public int CeldasSanas { get; set; } // contar celdas sanas en el período actual

        public Automata(int m)
        {
            M = m;
            EstadoActual = new Matriz<int>(m);
            Historico = new ListaEnlazada<Matriz<int>>();
            PeriodoActual = 0;
            CeldasContagiadas = 0;
            CeldasSanas = 0;
        }

        // inicializar el autómata con un estado inicial (desde un paciente)
        public void Inicializar(Paciente paciente)
        {
            EstadoActual.Construir(0); // todas sanas al inicio

            // marcar las celdas contagiadas desde el paciente
            paciente.Rejilla.ParaCada(celda =>
            {
                EstadoActual.Establecer(celda.Fila, celda.Columna, 1);
            });

            // guardar el estado inicial en el histórico
            Matriz<int> copia = EstadoActual.Copiar();
            Historico.Insertar(copia);
            PeriodoActual = 0;
            ActualizarContadores();
        }

        // contar vecinos contagiados de una celda (Moore: 8 vecinos)
        private int ContarVecinosContagiados(int fila, int columna)
        {
            ListaEnlazada<int> vecinos = EstadoActual.ObtenerVecinos(fila, columna);
            int contador = 0;

            vecinos.ParaCada(vecino =>
            {
                if (vecino == 1)
                    contador++;
            });

            return contador;
        }

        // aplicar la regla 1: célula contagiada sobrevive con 2 o 3 vecinos contagiados
        private int AplicarRegla1(int estadoActual, int vecinosContagiados)
        {
            if (estadoActual == 1) // si está contagiada
            {
                if (vecinosContagiados == 2 || vecinosContagiados == 3)
                    return 1; // sobrevive
                else
                    return 0; // sana
            }
            return estadoActual;
        }

        // aplicar la regla 2: célula sana se contagia si tiene exactamente 3 vecinos contagiados
        private int AplicarRegla2(int estadoActual, int vecinosContagiados)
        {
            if (estadoActual == 0) // si está sana
            {
                if (vecinosContagiados == 3)
                    return 1; // se contagia
            }
            return estadoActual;
        }

        // simular un período: evaluar reglas y generar nuevo estado
        public void SimularPeriodo()
        {
            Matriz<int> nuevoEstado = new Matriz<int>(M);
            nuevoEstado.Construir(0);

            // para cada celda, evaluar las reglas
            EstadoActual.ParaCada((estado, fila, columna) =>
            {
                int vecinosContagiados = ContarVecinosContagiados(fila, columna);

                // aplicar reglas
                int nuevoValor = estado;
                if (estado == 1)
                    nuevoValor = AplicarRegla1(estado, vecinosContagiados);
                else
                    nuevoValor = AplicarRegla2(estado, vecinosContagiados);

                nuevoEstado.Establecer(fila, columna, nuevoValor);
            });

            // actualizar el estado actual
            EstadoActual = nuevoEstado;
            PeriodoActual++;

            // guardar en histórico
            Matriz<int> copia = EstadoActual.Copiar();
            Historico.Insertar(copia);

            // actualizar contadores
            ActualizarContadores();
        }

        // actualizar los contadores de celdas sanas y contagiadas
        private void ActualizarContadores()
        {
            CeldasContagiadas = 0;
            CeldasSanas = 0;

            EstadoActual.ParaCada((estado, fila, columna) =>
            {
                if (estado == 1)
                    CeldasContagiadas++;
                else
                    CeldasSanas++;
            });
        }

        // comparar dos matrices para ver si son idénticas
        public bool CompararMatrices(Matriz<int> m1, Matriz<int> m2)
        {
            bool sonIguales = true;

            m1.ParaCada((dato, fila, columna) =>
            {
                int dato2 = m2.Obtener(fila, columna);
                if (dato != dato2)
                    sonIguales = false;
            });

            return sonIguales;
        }

        // evaluar diagnóstico: detectar patrones de repetición
        public DiagnosticoResultado EvaluarDiagnostico()
        {
            DiagnosticoResultado resultado = new DiagnosticoResultado();

            // Patrón MORTAL: retorna al estado inicial (período 0)
            if (PeriodoActual > 0)
            {
                Matriz<int> estadoInicial = Historico.ObtenerPorIndice(0);
                if (CompararMatrices(EstadoActual, estadoInicial))
                {
                    resultado.Diagnostico = "Mortal";
                    resultado.N = PeriodoActual;  // El período donde vuelve al estado inicial
                    resultado.N1 = 0;  // El estado inicial es el período 0
                    return resultado;
                }
            }

            // Patrón GRAVE: detectar oscilación periódica (patrón de período anterior que se repite)
            for (int i = 1; i < PeriodoActual; i++)
            {
                Matriz<int> estadoAnterior = Historico.ObtenerPorIndice(i);
                if (CompararMatrices(EstadoActual, estadoAnterior))
                {
                    // Se encontró que estado en período P coincide con período i
                    // Esto significa hay un patrón de período = P - i
                    int periodoOscilacion = PeriodoActual - i;
                    
                    resultado.Diagnostico = "Grave";
                    resultado.N = i;  // Período donde inició el patrón
                    resultado.N1 = periodoOscilacion;  // Cada cuántos períodos se repite
                    return resultado;
                }
            }

            // Patrón LEVE: sin repeticiones detectadas
            resultado.Diagnostico = "Leve";
            resultado.N = PeriodoActual;  // Registrar hasta qué período llegó
            resultado.N1 = 0;
            return resultado;
        }

        // simular todos los períodos de un paciente
        public DiagnosticoResultado SimularCompleto(Paciente paciente)
        {
            Inicializar(paciente);

            for (int periodo = 0; periodo < paciente.Periodos; periodo++)
            {
                SimularPeriodo();

                // verificar si hay diagnóstico en cada período
                DiagnosticoResultado resultado = EvaluarDiagnostico();
                if (resultado.Diagnostico != "Leve")
                {
                    resultado.PeriodoFinal = PeriodoActual;
                    return resultado;
                }
            }

            // si llegamos aquí, es leve
            DiagnosticoResultado resultadoFinal = new DiagnosticoResultado();
            resultadoFinal.Diagnostico = "Leve";
            resultadoFinal.PeriodoFinal = PeriodoActual;
            return resultadoFinal;
        }
    }

    // clase para guardar el resultado del diagnóstico
    public class DiagnosticoResultado
    {
        public string Diagnostico { get; set; } // "Leve", "Grave" o "Mortal"
        public int N { get; set; } // período en que se repite el patrón inicial (si aplica)
        public int N1 { get; set; } // período en que se repite el patrón secundario (si aplica)
        public int PeriodoFinal { get; set; } // período en que terminó la simulación
    }
}
