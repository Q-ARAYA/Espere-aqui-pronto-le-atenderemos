using System.Windows;
using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System.Linq;
using System.Collections.Specialized;

namespace ProyectoAnalisis.Vistas
{
    public partial class VentanaPaciente : Window
    {
        public VentanaPaciente()
        {
            InitializeComponent();
            CargarEspecialidades();
        }

        private void CargarEspecialidades()
        {
            var lista = LogicaVistaMain.CargarEspecialidades();
            cmbEspecialidad.ItemsSource = lista.Select(esp => $"{esp.Nombre} - {esp.Duracion} min").ToList();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string especialidadSeleccionada = cmbEspecialidad.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre del paciente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(especialidadSeleccionada))
            {
                MessageBox.Show("Seleccione una especialidad.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            var todasEspecialidades = LogicaVistaMain.CargarEspecialidades();
            var especialidad = todasEspecialidades
                .FirstOrDefault(esp => $"{esp.Nombre} - {esp.Duracion} min" == especialidadSeleccionada);

            if (especialidad == null)
            {
                MessageBox.Show("Especialidad no válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Usa el controlador para crear el paciente
            var ultimoPacienteID = LogicaVistaMain.ObtenerPacientes().Count; 
            var resultado = LogicaVistaMain.CrearPaciente(ultimoPacienteID, nombre, especialidad);
            System.Diagnostics.Debug.WriteLine($"David te odioooo");
            System.Diagnostics.Debug.WriteLine($"Paciente creado: {nombre}, Especialidad: {especialidad.Nombre}, Duración: {especialidad.Duracion}");


            if (resultado == null)
            {
                var paciente = LogicaVistaMain.ObtenerPacientes().LastOrDefault();
                if (paciente != null && Owner is VentanaPrincipal ventanaPrincipal)
                {
                    ventanaPrincipal.AgregarPacienteEnEspera(paciente);
                }
            }
            else
            {
                MessageBox.Show(resultado, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}