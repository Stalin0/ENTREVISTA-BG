using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.Movie.DTOs
{
    public class DeleteMovieDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
