using CineBFP.Application.Movie.Models;
using CineBFP.Domain.Movie.DTOs;

namespace CineBFP.Api.Services.Movie
{
    public interface IMovieService
    {
        Task<MovieDto> InsertMovie(MovieInput request);
        Task<GetMovieDto> GetMovie();
        Task<DeleteMovieDto> DeleteMovie(int idPelicula);
    }
}
