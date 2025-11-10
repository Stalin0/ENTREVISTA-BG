using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.Movie.Entities
{
    public class GetMovieEntities
    {
        public int IdPelicula { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Director { get; set; }
        public int Anio { get; set; }
        public string? Genero { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
