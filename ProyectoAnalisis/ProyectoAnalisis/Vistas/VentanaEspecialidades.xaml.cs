// Archivo: Vistas/VentanaEspecialidades.xaml.cs
using System.Windows;
using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System.Collections.ObjectModel;

namespace ProyectoAnalisis.Vistas
{
    public partial class VentanaEspecialidades : Window
    {
        private ObservableCollection<string> especialidadesEnVista = new ObservableCollection<string>();

        public VentanaEspecialidades()
        {
            InitializeComponent();
            lstEspecialidades.ItemsSource = especialidadesEnVista;
            RefrescarLista();
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (!int.TryParse(txtDuracion.Text.Trim(), out int duracion))
            {
                duracion = 0;
            }

            string error = LogicaVistaMain.CrearEspecialidad(nombre, duracion);

            if (error != null)
            {
                MessageBox.Show(error, "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Especialidad creada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNombre.Clear();
                txtDuracion.Clear();
                RefrescarLista();
            }
        }

        private void RefrescarLista()
        {
            especialidadesEnVista.Clear();
            var listaDesdeLogica = LogicaVistaMain.CargarEspecialidades();
            int i = 1;
            foreach (var esp in listaDesdeLogica)
            {
                especialidadesEnVista.Add($"{i}. {esp.Nombre} - {esp.Duracion} min");
                i++;
            }
        }
    }
}