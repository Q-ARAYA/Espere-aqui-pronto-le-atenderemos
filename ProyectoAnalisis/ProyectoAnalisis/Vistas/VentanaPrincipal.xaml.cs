using ProyectoAnalisis.Logica;
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
        public VentanaPrincipal()
        {
            InitializeComponent();
        }

        private void btnEspecialidades_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"David te odioooo");
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

            var imagenes = Directory.GetFiles("C:\\Users\\david\\OneDrive\\Escritorio\\Espere-aqui-pronto-le-atenderemos#\\ProyectoAnalisis\\ProyectoAnalisis\\Imagenes\\", "*.png").ToList();

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
            // Asume que tienes 15 ListBox: lstConsultorio1 ... lstConsultorio15
            List<ListBox> listBoxes = new List<ListBox>
    {
            lstConsultorio1, lstConsultorio2, lstConsultorio3, lstConsultorio4, lstConsultorio5,
            lstConsultorio6, lstConsultorio7, lstConsultorio8, lstConsultorio9, lstConsultorio10,
            lstConsultorio11, lstConsultorio12, lstConsultorio13, lstConsultorio14, lstConsultorio15
    };

            for (int i = 0; i < listBoxes.Count; i++)
            {
                if (i < consultorios.Count && consultorios[i].ColaPacientes != null)
                {
                    listBoxes[i].ItemsSource = null;
                    listBoxes[i].ItemsSource = consultorios[i].ColaPacientes;
                }
                else
                {
                    listBoxes[i].ItemsSource = null;
                }
            }
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
                    .Distinct() // Evita duplicados si algún paciente está en ambos lados
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

                foreach (var paciente in pacientesEnEspera.ToList())
                {
                    if (pacientesAsignados.Contains(paciente))
                        LogicaVistaMain.EliminarPacienteEnEspera(paciente);
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
                .Where(c => c.Activo)
                .Select(async consultorio =>
                {
                    while (consultorio.ColaPacientes.Count > 0)
                    {
                        token.ThrowIfCancellationRequested();

                        var pacienteEnEspera = consultorio.ColaPacientes[0];
                        var especialidadAtendida = pacienteEnEspera.EspecialidadPendiente;
                        int duracion = especialidadAtendida?.Duracion ?? 1;

                        await Task.Delay(duracion * 1000, token);

                        consultorio.ColaPacientes.RemoveAt(0);

                        // Elimina la especialidad atendida de la lista del paciente
                        pacienteEnEspera.Paciente.Especialidades.Remove(especialidadAtendida);

                        // Si quedan más especialidades, vuelve a ponerlo en espera con la siguiente
                        if (pacienteEnEspera.Paciente.Especialidades.Count > 0)
                        {
                            pacienteEnEspera.EspecialidadPendiente = pacienteEnEspera.Paciente.Especialidades.First();
                            LogicaVistaMain.CrearPacienteEnEspera(pacienteEnEspera.Paciente, pacienteEnEspera.Imagen);
                            ActualizarListaEspera();
                            await ReoptimizarYAtender();
                            return; // Importante: salir del ciclo para evitar conflictos de 

                        }

                        Dispatcher.Invoke(() => ActualizarColasConsultorios(consultorios));
                    }
                }).ToList();

            await Task.WhenAll(tareas);
        }

        private async void btnIniciarOptimizacion_Click(object sender, RoutedEventArgs e)
        {
            optimizacionEnCurso = true;
            await ReoptimizarYAtender();
            optimizacionEnCurso = false;
        }
    }
}
