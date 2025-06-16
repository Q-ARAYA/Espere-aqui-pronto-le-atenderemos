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
        private VentanaPrincipal _ventanaPrincipal;

        public VentanaConsultorio(int numeroConsultorio, VentanaPrincipal ventanaPrincipal)
        {
            InitializeComponent();
            this.numeroConsultorio = numeroConsultorio;
            _ventanaPrincipal = ventanaPrincipal;

            // Buscar si ya existe el consultorio
            var consultorio = LogicaVistaMain.ObtenerConsultorios()
                .FirstOrDefault(c => c.NumeroConsultorio == numeroConsultorio);

            if (consultorio != null)
            {
                activo = consultorio.Activo;
                // Si quieres también mostrar las especialidades ya asignadas:
                var lista = LogicaVistaMain.CargarEspecialidades();
                lstEspecialidades.ItemsSource = lista.Select(esp => $"{esp.Nombre} - {esp.Duracion} min").ToList();

                // Selecciona las especialidades ya asignadas
                foreach (var esp in consultorio.Especialidades)
                {
                    var item = $"{esp.Nombre} - {esp.Duracion} min";
                    lstEspecialidades.SelectedItems.Add(item);
                }
            }
            else
            {
                activo = false;
                CargarEspecialidades();
            }

            ActualizarBoton();
            lstEspecialidades.SelectionChanged += LstEspecialidades_SelectionChanged;
        }

        private void ActualizarBoton()
        {
            btnActivar.Content = activo ? "Desactivar" : "Activar";
            btnActivar.Background = activo ? System.Windows.Media.Brushes.IndianRed : System.Windows.Media.Brushes.ForestGreen;
        }

        private async void btnActivar_Click(object sender, RoutedEventArgs e)
        {
            if (!activo)
            {
                if (lstEspecialidades.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos una especialidad para activar el consultorio.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            activo = !activo;
            ActualizarBoton();

            if (Owner is VentanaPrincipal ventanaPrincipal)
            {
                ventanaPrincipal.CambiarColorConsultorio(numeroConsultorio, activo);
            }

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
                    //if (_ventanaPrincipal.optimizacionEnCurso)
                        //await _ventanaPrincipal.ReoptimizarYAtender();
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
                //if (_ventanaPrincipal.optimizacionEnCurso)
                    //await _ventanaPrincipal.ReoptimizarYAtender();
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

