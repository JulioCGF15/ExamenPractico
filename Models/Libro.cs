using System;
using System.Collections.Generic;
using System.Text;

namespace ExamenPractico.Models
{
    /// <summary>
    /// Representa un libro dentro del catálogo.
    /// Modelo base usado por captura, CollectionView y CRUD.
    /// </summary>
    public class Libro
    {
        // Id único para poder identificar el libro al editar/eliminar
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Titulo { get; set; } = string.Empty;

        public string Autores { get; set; } = string.Empty;

        public string AnioEdicion { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public string UrlPortada { get; set; } = string.Empty;
    }
}