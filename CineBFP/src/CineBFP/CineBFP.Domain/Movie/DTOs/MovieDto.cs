using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.Movie.DTOs
{
    public class MovieDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? IdPelicula { get; set; }
    }
}
