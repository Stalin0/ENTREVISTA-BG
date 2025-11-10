using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.Movie.DTOs
{
    public class MovieDtoIn
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Director { get; set; }
        public int? Anio { get; set; }
        public string? Genero { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
    }
}
