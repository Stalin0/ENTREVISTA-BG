using CineBFP.Application.User.Application;
using CineBFP.Domain.Movie.DTOs;
using CineBFP.Domain.Movie.Interface.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Application.Movie.Application
{
    public class MovieApp : IMovieApp
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IUserApp _userApp;
        private readonly IHttpContextAccessor _http;

        public MovieApp(IMovieRepository movieRepository, IUserApp userApp, IHttpContextAccessor http)
        {
            _movieRepository = movieRepository;
            _userApp = userApp;
            _http = http;
        }

        public async Task<MovieDto> InsertMovie(MovieDtoIn request)
        {
            var user = _http.HttpContext?.User;
            var role = user?.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return new MovieDto
                {
                    Success = false,
                    Message = "Solo el rol Admin puede crear películas.",
                    IdPelicula = null
                };
            }
            request.UsuarioCreacion = string.IsNullOrWhiteSpace(request.UsuarioCreacion)
                ? (user?.FindFirst("usuario")?.Value
                   ?? user?.Identity?.Name
                   ?? "system")
                : request.UsuarioCreacion;
            var ok = await _movieRepository.InsertMovie(request);
            return new MovieDto
            {
                Success = ok,
                Message = ok ? "Película creada." : "No se pudo crear la película.",
                IdPelicula = null
            };
        }

        public async Task<GetMovieDto> GetMovie()
        {
            var rows = await _movieRepository.GetMovie();
            var dto = new GetMovieDto
            {
                Success = true,
                Count = rows.Count,
                Items = rows.Select(x => new MovieItemDto
                {
                    IdPelicula = x.IdPelicula,
                    Titulo = x.Titulo,
                    Descripcion = x.Descripcion,
                    Director = x.Director,
                    Anio = x.Anio,
                    Genero = x.Genero,
                    FechaCreacion = x.FechaCreacion
                }).ToList()
            };

            return dto;
        }

        public async Task<DeleteMovieDto> DeleteMovie(int idPelicula)
        {
            var role = _http.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.Equals(role, "Admin", System.StringComparison.OrdinalIgnoreCase))
            {
                return new DeleteMovieDto { Success = false, Message = "No autorizado." };
            }

            var ok = await _movieRepository.DeleteMovie(idPelicula);
            return new DeleteMovieDto
            {
                Success = ok,
                Message = ok ? "Película eliminada." : "No se encontró la película."
            };
        }
    }
}
