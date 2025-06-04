using System.Windows;
using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoAnalisis.Vistas
{
    public partial class VentanaConsultorio : Window
    {
        private bool activo = false;

        public VentanaConsultorio()
        {
            InitializeComponent();
            ActualizarBoton();
            CargarEspecialidades();
            lstEspecialidades.SelectionChanged += LstEspecialidades_SelectionChanged;
        }

        private void ActualizarBoton()
        {
            btnActivar.Content = activo ? "Desactivar" : "Activar";
            btnActivar.Background = activo ? System.Windows.Media.Brushes.IndianRed : System.Windows.Media.Brushes.ForestGreen;
        }

        private void btnActivar_Click(object sender, RoutedEventArgs e)
        {
            activo = !activo;
            ActualizarBoton();
        }

        private void CargarEspecialidades()
        {
            var lista = LogicaVistaMain.CargarEspecialidades();
            lstEspecialidades.ItemsSource = lista.Select(e => $"{e.Nombre} - {e.Duracion} min").ToList();
        }

        private void LstEspecialidades_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Limita la selección a 5 elementos
            if (lstEspecialidades.SelectedItems.Count > 5)
            {
                // Quita la última selección si supera el límite
                lstEspecialidades.SelectedItems.RemoveAt(lstEspecialidades.SelectedItems.Count - 1);
                MessageBox.Show("Solo puedes seleccionar hasta 5 especialidades.", "Límite alcanzado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}