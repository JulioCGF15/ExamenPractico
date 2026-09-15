using CommunityToolkit.Maui.Views;
using ExamenPractico.Models;

namespace ExamenPractico.Views
{
	public partial class LibroPopup : Popup<Libro>
	{
		private readonly Libro _original;

		public LibroPopup(Libro libro)
		{
			InitializeComponent();
			_original = libro;

			EntryTitulo.Text = libro.Titulo;
			EntryAutores.Text = libro.Autores;
			EntryAnio.Text = libro.AnioEdicion;
			EntryISBN.Text = libro.ISBN;
			EntryUrlPortada.Text = libro.UrlPortada;
		}

        private async void BtnGuardarCambios_Clicked(object? sender, EventArgs e)
        {
			string titulo = EntryTitulo.Text?.Trim() ?? string.Empty;
			string isbn = EntryISBN.Text?.Trim() ?? string.Empty;

			if(string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(isbn))
			{
				LabelError.Text = "Título e ISBN son obligatorios";
				LabelError.IsVisible = true;
				return;
			}
			var editado = new Libro
			{
				Id = _original.Id,
				Titulo = titulo,
				Autores = EntryAutores.Text?.Trim() ?? string.Empty,
				AnioEdicion = EntryAnio.Text?.Trim() ?? string.Empty,
				ISBN = isbn,
				UrlPortada = EntryUrlPortada.Text?.Trim() ?? string.Empty,
			};
			await CloseAsync(editado);
        }

        private async void BtnCancelar_Clicked(object? sender, EventArgs e)
        {
			await CloseAsync();
        }
    }
}