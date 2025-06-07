using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
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
    }
}
