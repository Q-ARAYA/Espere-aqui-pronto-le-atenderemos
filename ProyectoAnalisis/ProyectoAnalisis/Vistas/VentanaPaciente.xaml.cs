using System.Windows;
using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System.Linq;

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
            var resultado = LogicaVistaMain.CrearPaciente(nombre, especialidad);

            System.Diagnostics.Debug.WriteLine($"Paciente creado: {nombre}, Especialidad: {especialidad.Nombre}, Duración: {especialidad.Duracion}");


            if (resultado == null)
            {
                MessageBox.Show("Paciente agregado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNombre.Clear();
                cmbEspecialidad.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show(resultado, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}