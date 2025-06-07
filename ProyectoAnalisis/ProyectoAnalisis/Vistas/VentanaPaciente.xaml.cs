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
            lstEspecialidades.ItemsSource = lista;
            lstEspecialidades.DisplayMemberPath = "Nombre";
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            var especialidadesSeleccionadas = lstEspecialidades.SelectedItems.Cast<Especialidades>().ToList();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre del paciente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (especialidadesSeleccionadas == null || especialidadesSeleccionadas.Count == 0)
            {
                MessageBox.Show("Seleccione una especialidad.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            

            // Usa el controlador para crear el paciente
            var ultimoPacienteID = LogicaVistaMain.ObtenerPacientes().Count;
            var resultado = LogicaVistaMain.CrearPaciente(ultimoPacienteID, nombre, especialidadesSeleccionadas);
            System.Diagnostics.Debug.WriteLine($"Paciente creado: {nombre}, Especialidades: {string.Join(", ", especialidadesSeleccionadas.Select(x => x.Nombre))}");


            if (resultado == null)
            {
                MessageBox.Show("Paciente creado exitosamente con: " + especialidadesSeleccionadas.Count() + " especialidades.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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