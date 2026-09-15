using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ExamenPractico.Models;
using Microsoft.Maui.Storage;

namespace ExamenPractico.Services
{
    /// <summary>
    /// Servicio compartido para guardar y leer los libros
    /// usando JSON + Preferences.
    /// </summary>
    public class LibroService
    {
        private const string PreferencesKey = "libros_json";

        /// <summary>
        /// Carga la lista de libros guardada en Preferences (deserializa el JSON).
        /// Si no hay datos guardados, regresa una lista vacía.
        /// </summary>
        public List<Libro> CargarLibros()
        {
            string json = Preferences.Get(PreferencesKey, string.Empty);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Libro>();

            try
            {
                var libros = JsonSerializer.Deserialize<List<Libro>>(json);
                return libros ?? new List<Libro>();
            }
            catch
            {
                // Si el JSON está corrupto o cambió de formato, evitamos que la app truene
                return new List<Libro>();
            }
        }

        /// <summary>
        /// Serializa la lista completa de libros a JSON y la guarda en Preferences.
        /// Debe llamarse después de: agregar, editar o eliminar un libro.
        /// </summary>
        public void GuardarLibros(List<Libro> libros)
        {
            string json = JsonSerializer.Serialize(libros);
            Preferences.Set(PreferencesKey, json);
        }

        /// <summary>
        /// Borra toda la información guardada (útil para pruebas).
        /// </summary>
        public void LimpiarDatos()
        {
            Preferences.Remove(PreferencesKey);
        }
    }
}