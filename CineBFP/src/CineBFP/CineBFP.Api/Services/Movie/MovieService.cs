using CineBFP.Application.Movie.Application;
using CineBFP.Application.Movie.Models;
using CineBFP.Domain.Movie.DTOs;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace CineBFP.Api.Services.Movie
{
    public class MovieService : IMovieService
    {
        private readonly IMovieApp _movieApp;

        public MovieService(IMovieApp movieApp)
        {
            _movieApp = movieApp;
        }


        public async Task<MovieDto> InsertMovie(MovieInput req)
        {
           

            var dtoIn = new MovieDtoIn
            {
                Titulo = req.Titulo,
                Descripcion = req.Descripcion,
                Director = req.Director,
                Anio = req.Anio,
                Genero = req.Genero,
                
            };

            return await _movieApp.InsertMovie(dtoIn);
        }

        public Task<GetMovieDto> GetMovie()
            => _movieApp.GetMovie();

        public Task<DeleteMovieDto> DeleteMovie(int idPelicula)
            => _movieApp.DeleteMovie(idPelicula);
    }
}
