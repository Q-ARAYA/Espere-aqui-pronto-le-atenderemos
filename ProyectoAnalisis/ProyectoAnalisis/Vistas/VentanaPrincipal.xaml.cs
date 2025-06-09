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

namespace ProyectoAnalisis.Vistas
{
    public partial class VentanaPrincipal : Window
    {
        private bool optimizacionActiva = false;
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
            var pacientesEnEspera = LogicaVistaMain.ObtenerPacientesEnEspera();
            lstEspera.ItemsSource = null;
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
            if (!optimizacionActiva)
                return; // No hacer nada si la optimización no está activa

            var consultorios = LogicaVistaMain.ObtenerConsultorios();
            await MoverPacientesGraficamente(consultorios);
            await AtenderPacientesEnConsultorios(consultorios);
        }
        // Ejemplo: después de cerrar un consultorio

        private async Task AtenderPacientesEnConsultorios(List<Consultorios> consultorios)
        {
            bool hayPacientes = true;
            while (hayPacientes)
            {
                hayPacientes = false;
                foreach (var consultorio in consultorios.Where(c => c.Activo))
                {
                    if (consultorio.ColaPacientes.Count > 0)
                    {
                        hayPacientes = true;
                        var pacienteEnEspera = consultorio.ColaPacientes[0];
                        consultorio.ColaPacientes.RemoveAt(0);

                        // Busca la primera especialidad que el consultorio puede atender
                        var especialidadAtendible = pacienteEnEspera.Paciente.Especialidades
                            .FirstOrDefault(e => consultorio.Especialidades.Any(c => c.Nombre == e.Nombre));

                        if (especialidadAtendible != null)
                        {
                            int duracion = especialidadAtendible.Duracion;
                            await Task.Delay(duracion * 100);

                            // Elimina la especialidad atendida
                            pacienteEnEspera.Paciente.Especialidades.Remove(especialidadAtendible);

                            // Si quedan especialidades, intenta asignar a otro consultorio disponible
                            if (pacienteEnEspera.Paciente.Especialidades.Count > 0)
                            {
                                var siguienteEspecialidad = pacienteEnEspera.Paciente.Especialidades.FirstOrDefault();
                                var consultorioDisponible = consultorios
                                    .FirstOrDefault(c => c.Activo && c.Especialidades.Any(e => e.Nombre == siguienteEspecialidad.Nombre));
                                if (consultorioDisponible != null)
                                {
                                    consultorioDisponible.ColaPacientes.Add(pacienteEnEspera);
                                }
                                else
                                {
                                    LogicaVistaMain.CrearPacienteEnEspera(pacienteEnEspera.Paciente, pacienteEnEspera.Imagen);
                                }
                            }
                        }
                        else
                        {
                            // No se puede atender ninguna especialidad, vuelve a lista de espera
                            LogicaVistaMain.CrearPacienteEnEspera(pacienteEnEspera.Paciente, pacienteEnEspera.Imagen);
                        }

                        ActualizarColasConsultorios(consultorios);
                        ActualizarListaEspera();
                    }
                }
            }
        }

        private Task MoverPacientesGraficamente(List<Consultorios> consultorios)
        {
            // Limpiar colas
            foreach (var consultorio in consultorios)
                consultorio.ColaPacientes.Clear();

            // Obtener la mejor asignación del algoritmo genético
            var pacientesEnEspera = LogicaVistaMain.ObtenerPacientesEnEspera().ToList();
            var resultado = AlgoritmoGenetico.Optimizar(pacientesEnEspera, consultorios);

            foreach (var grupo in resultado)
            {
                if (grupo.Count == 0) continue;
                var consultorio = grupo[0].Consultorio;
                foreach (var asignacion in grupo)
                {
                    var pacienteEnEspera = pacientesEnEspera.FirstOrDefault(p => p.Paciente == asignacion.Paciente.Paciente);
                    if (pacienteEnEspera != null)
                    {
                        LogicaVistaMain.EliminarPacienteEnEspera(pacienteEnEspera);
                        consultorio.ColaPacientes.Add(pacienteEnEspera);
                    }
                }
            }

            // Refresca la UI una sola vez al final
            ActualizarListaEspera();
            ActualizarColasConsultorios(consultorios);

            return Task.CompletedTask;
        }
        private async void btnIniciarOptimizacion_Click(object sender, RoutedEventArgs e)
        {
            optimizacionActiva = true;
            var consultorios = LogicaVistaMain.ObtenerConsultorios();

            await MoverPacientesGraficamente(consultorios);
            await AtenderPacientesEnConsultorios(consultorios);

            // Verifica si realmente no quedan pacientes en espera ni en colas
            bool colasVacias = consultorios.All(c => c.ColaPacientes == null || c.ColaPacientes.Count == 0);
            bool esperaVacia = LogicaVistaMain.ObtenerPacientesEnEspera().Count == 0;

            if (colasVacias && esperaVacia)
                MessageBox.Show("Todos los pacientes han sido atendidos.");
            else
                MessageBox.Show("Aún quedan pacientes por atender.");
        }
    }
}
