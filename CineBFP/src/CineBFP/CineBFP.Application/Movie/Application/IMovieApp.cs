using CineBFP.Domain.Movie.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Application.Movie.Application
{
    public interface IMovieApp
    {
        Task<MovieDto> InsertMovie(MovieDtoIn request);
        Task<GetMovieDto> GetMovie();
        Task<DeleteMovieDto> DeleteMovie(int idPelicula);
    }
}
