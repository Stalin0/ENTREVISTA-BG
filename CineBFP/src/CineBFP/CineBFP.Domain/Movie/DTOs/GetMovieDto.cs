using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Domain.Movie.DTOs
{
    public class GetMovieDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("items")]
        public List<MovieItemDto> Items { get; set; } = new();
    }

    public class MovieItemDto
    {
        [JsonPropertyName("idPelicula")]
        public int IdPelicula { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("director")]
        public string? Director { get; set; }

        [JsonPropertyName("anio")]
        public int Anio { get; set; }

        [JsonPropertyName("genero")]
        public string? Genero { get; set; }

        [JsonPropertyName("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }
    }
}
