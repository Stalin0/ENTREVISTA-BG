using CineBFP.Api.Filters;
using CineBFP.Api.Services.Movie;
using CineBFP.Application.Movie.Models;
using CineBFP.Application.User.Models;
using CineBFP.Domain.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CineBFP.Api.Controllers.Movie
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieServices;

        public MovieController(IMovieService movieServices)
        {
            _movieServices = movieServices;
        }

        [JwtAuthorize("Admin")]
        [HttpPost("api/v1/movie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertMovie([FromBody] MovieInput request)
        {
            var response = await _movieServices.InsertMovie(request);
            return Ok(response);
        }

        [JwtAuthorize("Admin, Invitado")]
        [HttpGet("api/v1/movies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovie()
        {
            var response = await _movieServices.GetMovie();
            return Ok(response);
        }

        [JwtAuthorize("Admin")]
        [HttpDelete("api/v1/movie/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMovie([FromRoute] int id)
        {
            var resp = await _movieServices.DeleteMovie(id);
            return Ok(resp);
        }
    }
}
