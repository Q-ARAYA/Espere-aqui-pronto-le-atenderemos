using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            // 1. Obtener pacientes en espera y consultorios activos
            var pacientesEnEspera = LogicaVistaMain.ObtenerPacientesEnEspera();
            var consultorios = LogicaVistaMain.ObtenerConsultorios();

            // 2. Ejecutar el algoritmo genético para obtener la asignación óptima
            var resultadoOptimizacion = AlgoritmoGenetico.Optimizar(pacientesEnEspera, consultorios);

            // 3. Limpiar las colas actuales de los consultorios
            foreach (var consultorio in consultorios)
                consultorio.ColaPacientes.Clear();

            // 4. Asignar los pacientes optimizados a las colas de los consultorios
            foreach (var grupo in resultadoOptimizacion)
            {
                if (grupo.Count == 0) continue;
                var consultorio = consultorios.FirstOrDefault(c => c.NumeroConsultorio == grupo[0].Consultorio.NumeroConsultorio);
                if (consultorio != null)
                {
                    foreach (var asignacion in grupo)
                        consultorio.ColaPacientes.Add(asignacion.Paciente);
                }
            }

            // 5. Limpiar la lista de espera
            foreach (var paciente in pacientesEnEspera.ToList())
                LogicaVistaMain.EliminarPacienteEnEspera(paciente);

            // 6. Actualizar la UI
            ActualizarListaEspera();
            ActualizarColasConsultorios(consultorios);

            // 7. Iniciar la atención en paralelo
            await AtenderPacientesEnConsultorios();
        }

        private async Task AtenderPacientesEnConsultorios()
        {
            var consultorios = LogicaVistaMain.ObtenerConsultorios();

            // Crear una tarea por consultorio activo
            var tareas = consultorios
                .Where(c => c.Activo)
                .Select(async consultorio =>
                {
                    while (consultorio.ColaPacientes.Count > 0)
                    {
                        var paciente = consultorio.ColaPacientes[0];
                        // Simular la atención (puedes ajustar el tiempo según la especialidad)
                        int duracion = paciente.Paciente.Especialidades.FirstOrDefault(e => consultorio.Especialidades.Any(ce => ce.Nombre == e.Nombre))?.Duracion ?? 1;
                        await Task.Delay(duracion * 700); // 100 ms por minuto de especialidad (ajusta a tu gusto)
                        consultorio.ColaPacientes.RemoveAt(0);

                        // Actualizar la UI en el hilo principal
                        Dispatcher.Invoke(() => ActualizarColasConsultorios(consultorios));
                    }
                }).ToList();

            // Esperar a que todos los consultorios terminen
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
