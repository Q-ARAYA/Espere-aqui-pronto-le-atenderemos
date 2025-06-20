﻿using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ProyectoAnalisis.Vistas
{
    public partial class VentanaPrincipal : Window
    {
        private bool optimizacionActiva = false;
        private DispatcherTimer timerAtencion;
        private List<Consultorios> consultoriosOptimizados;
        public bool optimizacionEnCurso = false;
        private CancellationTokenSource optimizacionCts;
        private DispatcherTimer timerPrioridades;
        public VentanaPrincipal()
        {
            InitializeComponent();
            IniciarTimerPrioridades();
        }

        private void btnCargarDatos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ruta completa al archivo datos.xml ubicado en la raiz del proyecto
                string rutaArchivo = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "datos.xml")
                );


                // Cargar especialidades
                List<Especialidades> especialidades = CargaDatos.CargarEspecialidadesDesdeXML(rutaArchivo);

                // Cargar pacientes usando las especialidades cargadas
                List<Pacientes> pacientes = CargaDatos.CargarPacientesDesdeXML(rutaArchivo, especialidades);

                ActualizarListaEspera();

                // Mostrar resumen en MessageBox
                string mensaje = $"Especialidades cargadas: {especialidades.Count}\n" +
                                 $"Pacientes cargados: {pacientes.Count}";

                MessageBox.Show(mensaje, "Carga de datos", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el archivo:\n" + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btnEspecialidades_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaEspecialidades();
            ventana.ShowDialog();
        }

        private void Consultorio_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string tag && int.TryParse(tag, out int numeroConsultorio))
            {
                var ventana = new VentanaConsultorio(numeroConsultorio, this);
                ventana.Owner = this;
                ventana.ShowDialog();
            }
        }

        private void btnPacientes_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaPaciente();
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        public void CambiarColorConsultorio(int numeroConsultorio, bool activo)
        {

            var uniformGrid = LogicalTreeHelper.FindLogicalNode(this, "CabeceraConsultorios") as UniformGrid;
            if (uniformGrid == null)
                return;

            // Busca el Border correspondiente usando el Tag
            foreach (var child in uniformGrid.Children)
            {
                if (child is Border border && border.Tag != null && border.Tag.ToString() == numeroConsultorio.ToString())
                {
                    border.Background = activo ? Brushes.LightGreen : Brushes.IndianRed;
                    break;
                }
            }
        }


        public void AgregarPacienteEnEspera(Pacientes paciente)
        {
            // Selecciona una imagen aleatoria

            string rutaBase = AppDomain.CurrentDomain.BaseDirectory;
            string rutaImagenes = System.IO.Path.GetFullPath(System.IO.Path.Combine(rutaBase, @"..\..\Imagenes"));
            var imagenes = Directory.GetFiles(rutaImagenes, "*.png").ToList();


            string imagenSeleccionada = imagenes.Count > 0
                ? imagenes[new System.Random().Next(imagenes.Count)]
                : null;

            var resultado = LogicaVistaMain.CrearPacienteEnEspera(paciente, imagenSeleccionada);
            if (resultado != null)
            {
                MessageBox.Show(resultado, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ActualizarListaEspera();
        }

        private void ActualizarListaEspera()
        {
            lstEspera.ItemsSource = null;
            var pacientesEnEspera = LogicaVistaMain.ObtenerPacientesEnEspera();
            lstEspera.ItemsSource = pacientesEnEspera;
        }

        private void lstEspera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void ActualizarColasConsultorios(List<Consultorios> consultorios)
        {

            List<ListBox> listBoxes = new List<ListBox>
            {
                lstConsultorio1, lstConsultorio2, lstConsultorio3, lstConsultorio4, lstConsultorio5,
                lstConsultorio6, lstConsultorio7, lstConsultorio8, lstConsultorio9, lstConsultorio10,
                lstConsultorio11, lstConsultorio12, lstConsultorio13, lstConsultorio14, lstConsultorio15
            };

            // Lista correspondiente de TextBlocks para mostrar la duración
            List<TextBlock> tiempoTextBlocks = new List<TextBlock>
            {
                txtTiempoConsultorio1, txtTiempoConsultorio2, txtTiempoConsultorio3, txtTiempoConsultorio4, txtTiempoConsultorio5,
                txtTiempoConsultorio6, txtTiempoConsultorio7, txtTiempoConsultorio8, txtTiempoConsultorio9, txtTiempoConsultorio10,
                txtTiempoConsultorio11, txtTiempoConsultorio12, txtTiempoConsultorio13, txtTiempoConsultorio14, txtTiempoConsultorio15
            };

            // Limpia todos los listbox y oculta los tiempos primero
            for (int i = 0; i < listBoxes.Count; i++)
            {
                listBoxes[i].ItemsSource = null;
                if (i < tiempoTextBlocks.Count && tiempoTextBlocks[i] != null)
                {
                    tiempoTextBlocks[i].Text = "";
                    tiempoTextBlocks[i].Visibility = Visibility.Collapsed;
                }
            }

            // Asigna cada consultorio a su listbox según el identificador
            foreach (var consultorio in consultorios)
            {
                int idx = consultorio.NumeroConsultorio - 1;
                if (idx >= 0 && idx < listBoxes.Count)
                {
                    // Actualiza la cola de pacientes
                    listBoxes[idx].ItemsSource = consultorio.ColaPacientes;

                    // Actualiza el TextBlock de tiempo si existe y el consultorio está activo
                    if (idx < tiempoTextBlocks.Count && tiempoTextBlocks[idx] != null && consultorio.Activo)
                    {
                        tiempoTextBlocks[idx].Text = consultorio.ObtenerTiempoEsperaFormateado();
                        tiempoTextBlocks[idx].Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void IniciarTimerPrioridades()
        {
            timerPrioridades = new DispatcherTimer();
            timerPrioridades.Interval = TimeSpan.FromSeconds(10); // Actualizar cada 10 segundos
            timerPrioridades.Tick += ActualizarPrioridadesPacientes;
            timerPrioridades.Start();
        }

        private void ActualizarPrioridadesPacientes(object sender, EventArgs e)
        {
            var pacientesEnEspera = LogicaVistaMain.ObtenerPacientesEnEspera();
            if (pacientesEnEspera.Count == 0) return;

            var consultorios = LogicaVistaMain.ObtenerConsultorios();

            // Incrementar prioridad para cada paciente
            foreach (var paciente in pacientesEnEspera)
            {
                // Verificar si hay algún consultorio activo para su especialidad
                bool especialidadDisponible = false;
                foreach (var especialidad in paciente.Paciente.Especialidades)
                {
                    if (consultorios.Any(c => c.Activo && c.Especialidades.Any(ec => ec.Nombre == especialidad.Nombre)))
                    {
                        especialidadDisponible = true;
                        break;
                    }
                }

                // Aumentar prioridad según disponibilidad
                paciente.IncrementarPrioridad(especialidadDisponible);
            }

            // Actualizar interfaz
            ActualizarListaEspera();

        }
        public async Task ReoptimizarYAtender()
        {
            // Cancelar optimización anterior si existe
            optimizacionCts?.Cancel();
            optimizacionCts = new CancellationTokenSource();
            var token = optimizacionCts.Token;

            try
            {
                var pacientesEnEspera = LogicaVistaMain.ObtenerPacientesEnEspera();
                var consultorios = LogicaVistaMain.ObtenerConsultorios();

                // Reunir todos los pacientes: en espera + en colas de consultorios
                var pacientesEnColas = consultorios
                    .SelectMany(c => c.ColaPacientes ?? new List<PacientesEnEspera>())
                    .ToList();

                var todosLosPacientes = pacientesEnEspera
                    .Concat(pacientesEnColas)
                    .Distinct()
                    .ToList();

                // Limpiar todas las colas antes de optimizar
                foreach (var consultorio in consultorios)
                    consultorio.ColaPacientes.Clear();

                // Ejecutar el algoritmo genético con todos los pacientes
                var resultadoOptimizacion = AlgoritmoGenetico.Optimizar(todosLosPacientes, consultorios);

                var pacientesAsignados = new HashSet<PacientesEnEspera>();
                foreach (var grupo in resultadoOptimizacion)
                {
                    if (grupo.Count == 0) continue;
                    var consultorio = consultorios.FirstOrDefault(c => c.NumeroConsultorio == grupo[0].Consultorio.NumeroConsultorio);
                    if (consultorio != null)
                    {
                        foreach (var asignacion in grupo)
                        {
                            consultorio.ColaPacientes.Add(asignacion.Paciente);
                            pacientesAsignados.Add(asignacion.Paciente);
                        }
                    }
                }

                
                // Solo elimina de la lista de espera a los pacientes que realmente fueron asignados
                foreach (var paciente in pacientesEnEspera.ToList())
                {
                    if (pacientesAsignados.Contains(paciente))
                        LogicaVistaMain.EliminarPacienteEnEspera(paciente);
                    // Si no fue asignado, permanece en espera
                }

                // Además, los pacientes que estaban en colas y no fueron reasignados deben volver a espera
                foreach (var paciente in pacientesEnColas)
                {
                    if (!pacientesAsignados.Contains(paciente))
                    {
                        // Evita duplicados en espera
                        LogicaVistaMain.CrearPacienteEnEspera(paciente.Paciente, paciente.Imagen);

                    }
                }

                ActualizarListaEspera();
                ActualizarColasConsultorios(consultorios);

                await AtenderPacientesEnConsultorios(token);
            }
            catch (OperationCanceledException)
            {
                // La optimización fue cancelada, no hacer nada
            }
        }

        private async Task AtenderPacientesEnConsultorios(CancellationToken token)
        {
            var consultorios = LogicaVistaMain.ObtenerConsultorios();

            var tareas = consultorios
                .Select(async consultorio =>
                {
                    while (consultorio.ColaPacientes.Count > 0)
                    {
                        token.ThrowIfCancellationRequested();

                        // Si el consultorio ya no está activo, mueve los pacientes restantes a espera y termina
                        if (!consultorio.Activo)
                        {
                            
                            while (consultorio.ColaPacientes.Count > 0)
                            {
                                var pacienteRestante = consultorio.ColaPacientes[0];
                                LogicaVistaMain.CrearPacienteEnEspera(pacienteRestante.Paciente, pacienteRestante.Imagen);
                                consultorio.ColaPacientes.RemoveAt(0);
                            }
                            Dispatcher.Invoke(() => ActualizarColasConsultorios(consultorios));
                            Dispatcher.Invoke(() => ActualizarListaEspera());
                            return;
                        }

                        var pacienteEnEspera = consultorio.ColaPacientes[0];
                        var especialidadAtendida = pacienteEnEspera.EspecialidadPendiente;
                        int duracion = especialidadAtendida?.Duracion ?? 1;

                        await Task.Delay(duracion * 500, token);

                        consultorio.ColaPacientes.RemoveAt(0);

                        // Elimina la especialidad atendida de la lista del paciente
                        pacienteEnEspera.Paciente.Especialidades.Remove(especialidadAtendida);

                        // Si quedan más especialidades

                        /*
                        if (pacienteEnEspera.Paciente.Especialidades.Count > 0)
                        {
                            var siguiente = pacienteEnEspera.Paciente.Especialidades.First();
                            pacienteEnEspera.EspecialidadPendiente = siguiente;
                            Dispatcher.Invoke(() => ActualizarColasConsultorios(consultorios));
                            var consultoriosDisponibles = consultorios
                                .Where(c => c.Activo && c.Especialidades.Any(e => e.Nombre == siguiente.Nombre))
                                .ToList();

                            LogicaVistaMain.CrearPacienteEnEspera(pacienteEnEspera.Paciente, pacienteEnEspera.Imagen);
                            Dispatcher.Invoke(() => ActualizarListaEspera());

                            if (consultoriosDisponibles.Count > 0)
                            {
                                await ReoptimizarYAtender();
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                        */

                        Dispatcher.Invoke(() => ActualizarColasConsultorios(consultorios));
                    }
                }).ToList();

            await Task.WhenAll(tareas);
            optimizacionEnCurso = false;
            btnOptimizar.IsEnabled = true;
            btnOptimizar.Content = "Optimizar";
            btnOptimizar.Background = Brushes.Green;
        }
        private async void btnIniciarOptimizacion_Click(object sender, RoutedEventArgs e)
        {
            optimizacionEnCurso = true;
            btnOptimizar.IsEnabled = false;
            btnOptimizar.Content = "Optimizando...";
            btnOptimizar.Background = Brushes.LightGray;
            await ReoptimizarYAtender();
            

        }
    }
}
