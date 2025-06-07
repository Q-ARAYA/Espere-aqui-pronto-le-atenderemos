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
                var ventana = new VentanaConsultorio(numeroConsultorio);
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
    }
}
