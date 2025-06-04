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
        private int numeroConsultorio;

        public VentanaConsultorio()
        {
            InitializeComponent();
            ActualizarBoton();
            CargarEspecialidades();
            lstEspecialidades.SelectionChanged += LstEspecialidades_SelectionChanged;
        }

        public VentanaConsultorio(int numeroConsultorio)
        {
            InitializeComponent();
            this.numeroConsultorio = numeroConsultorio;
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

            if (activo)
            {
                // Obtener especialidades seleccionadas (como string)
                var seleccionadas = lstEspecialidades.SelectedItems.Cast<string>().ToList();

                // Obtener la lista real de especialidades desde la lógica
                var todasEspecialidades = LogicaVistaMain.CargarEspecialidades();

                // Enlazar las especialidades seleccionadas con los objetos reales
                var especialidadesAsignadas = todasEspecialidades
                    .Where(esp => seleccionadas.Contains($"{esp.Nombre} - {esp.Duracion} min"))
                    .ToList();

                // Usar el controlador para crear el consultorio
                var resultado = LogicaVistaMain.CrearConsultorio(
                    numeroConsultorio,
                    true,
                    especialidadesAsignadas
                );

                if (resultado == null)
                {
                    MessageBox.Show($"Consultorio {numeroConsultorio} activado con {especialidadesAsignadas.Count} especialidad(es).");
                }
                else
                {
                    MessageBox.Show(resultado, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Desactivar consultorio
                LogicaVistaMain.DesactivarConsultorio(numeroConsultorio);
                MessageBox.Show($"Consultorio {numeroConsultorio} desactivado correctamente.");
            }
        }

        private void CargarEspecialidades()
        {
            var lista = LogicaVistaMain.CargarEspecialidades();
            lstEspecialidades.ItemsSource = lista.Select(esp => $"{esp.Nombre} - {esp.Duracion} min").ToList();
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

