using System.Collections.ObjectModel;
using ExamenPractico.Models;
using ExamenPractico.Services;

namespace ExamenPractico
{
    public partial class MainPage : ContentPage
    {
        // Servicio de persistencia
        private readonly LibroService _libroService;

        // Colección observable: al modificarla, el CollectionView se actualiza solo
        private ObservableCollection<Libro> _libros;

        public MainPage()
        {
            InitializeComponent();

            _libroService = new LibroService();

            // Cargar libros guardados al iniciar la app (deserialización JSON)
            var librosGuardados = _libroService.CargarLibros();
            _libros = new ObservableCollection<Libro>(librosGuardados);

            CollectionViewLibros.ItemsSource = _libros;
        }

        /// <summary>
        /// Captura y valida un nuevo libro, lo agrega a la lista
        /// y lo guarda en JSON/Preferences.
        /// </summary>
        private void BtnGuardar_Clicked(object sender, EventArgs e)
        {
            string titulo = EntryTitulo.Text?.Trim() ?? string.Empty;
            string autores = EntryAutores.Text?.Trim() ?? string.Empty;
            string anio = EntryAnio.Text?.Trim() ?? string.Empty;
            string isbn = EntryISBN.Text?.Trim() ?? string.Empty;
            string urlPortada = EntryUrlPortada.Text?.Trim() ?? string.Empty;

            // Validación: campos obligatorios (Título e ISBN)
            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(isbn))
            {
                MostrarError("Título e ISBN son obligatorios.");
                return;
            }

            OcultarError();

            var nuevoLibro = new Libro
            {
                Titulo = titulo,
                Autores = autores,
                AnioEdicion = anio,
                ISBN = isbn,
                UrlPortada = urlPortada
            };

            // Agregar a la colección en memoria (esto refresca el CollectionView automáticamente)
            _libros.Add(nuevoLibro);

            // Guardar la lista completa en JSON + Preferences
            GuardarCambios();

            LimpiarFormulario();
        }

        /// <summary>
        /// Serializa y guarda la lista actual de libros.
        /// </summary>
        private void GuardarCambios()
        {
            _libroService.GuardarLibros(new List<Libro>(_libros));
        }

        private void LimpiarFormulario()
        {
            EntryTitulo.Text = string.Empty;
            EntryAutores.Text = string.Empty;
            EntryAnio.Text = string.Empty;
            EntryISBN.Text = string.Empty;
            EntryUrlPortada.Text = string.Empty;
        }

        private void MostrarError(string mensaje)
        {
            LabelError.Text = mensaje;
            LabelError.IsVisible = true;
        }

        private void OcultarError()
        {
            LabelError.IsVisible = false;
        }
    }
}