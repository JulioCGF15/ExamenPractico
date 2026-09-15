using CommunityToolkit.Maui;              
using CommunityToolkit.Maui.Extensions;   
using CommunityToolkit.Maui.Views;
using ExamenPractico.Views;
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
        private Libro? _libroSeleccionado;
        private bool _ocupado;
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
        private void CollectionViewLibros_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            _libroSeleccionado = e.CurrentSelection.FirstOrDefault() as Libro;
            MostrarDetalle(_libroSeleccionado);
        }

        private void MostrarDetalle(Libro? libro)
        {
            if(libro is null)
            {
                PanelDetalle.IsVisible = false;
                return;
            }
            LblDetalleTitulo.Text = libro.Titulo;
            LblDetalleAutores.Text = string.IsNullOrWhiteSpace(libro.Autores) ? "(sin autores)" : libro.Autores;
            LblDetalleAnio.Text = $"Año: {libro.AnioEdicion}";
            LblDetalleIsbn.Text = $"ISBN: {libro.ISBN}";

            ImgDetallePortada.Source = Uri.TryCreate(libro.UrlPortada, UriKind.Absolute, out var uri) 
                ? ImageSource.FromUri(uri)
                : null;

            PanelDetalle.IsVisible = true;
        }

        private async void BtnEditar_Clicked(object sender, EventArgs e)
        {
            if (_ocupado) return;
            _ocupado = true;
            try 
            {
                var original = _libroSeleccionado;
                if(original is null)
            {
                await DisplayAlertAsync("Aviso", "Selecciona un libro primero.", "OK");
                return;
            }

            var copia = new Libro
            {
                Id = original.Id,
                Titulo = original.Titulo,
                Autores = original.Autores,
                AnioEdicion = original.AnioEdicion,
                ISBN = original.ISBN,
                UrlPortada = original.UrlPortada
            };

            var popup = new LibroPopup(copia);
                var resultado = await this.ShowPopupAsync<Libro>(popup, new PopupOptions { Shape = null }, CancellationToken.None);
                
                if (resultado.WasDismissedByTappingOutsideOfPopup)
                return;

                if (resultado.Result is Libro editado)
                    ActualizarLibro(original, editado); 
            }
            finally { _ocupado = false; }
        }

        private void ActualizarLibro(Libro original, Libro editado)
        {
            //var actual =_libros.FirstOrDefault(l => l.Id == editado.Id);
            //if (actual is null)
            //    return;

            int indice = _libros.IndexOf(original);
            if (indice < 0) return;

            _libros[indice] = editado;

            _libroSeleccionado = editado;
            CollectionViewLibros.SelectedItem = editado;
            MostrarDetalle(editado);

            GuardarCambios();
        }

        private async void BtnEliminar_Clicked(object sender, EventArgs e)
        {
            if (_ocupado) return;
            _ocupado = true;
            try
            {
                var aEliminar = _libroSeleccionado;
                if (aEliminar is null)
                {
                    await DisplayAlertAsync("Aviso", "Selecciona un libro primero.", "OK");
                    return;
                }

                bool confirmar = await DisplayAlertAsync(
                    "Confirmar",
                    $"¿Está seguro de eliminar este registro?\n\n{aEliminar.Titulo}",
                    "Sí", "No");

                if (!confirmar)
                    return;

                _libros.Remove(aEliminar);
                _libroSeleccionado = null;
                CollectionViewLibros.SelectedItem = null;
                MostrarDetalle(null);

                GuardarCambios();
            }
            finally { _ocupado = false; }
        }
    }
}
