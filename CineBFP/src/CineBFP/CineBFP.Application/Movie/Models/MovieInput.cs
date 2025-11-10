using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineBFP.Application.Movie.Models
{
    public class MovieInput
    {
        [Required, MaxLength(150)]
        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [MaxLength(500)]
        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("director")]
        public string? Director { get; set; }

        [JsonPropertyName("anio")]
        public int? Anio { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("genero")]
        public string? Genero { get; set; }

    }
}
