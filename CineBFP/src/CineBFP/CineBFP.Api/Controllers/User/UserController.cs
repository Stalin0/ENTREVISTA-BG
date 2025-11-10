using CineBFP.Api.Filters;
using CineBFP.Api.Services.User;
using CineBFP.Application.User.Models;
using CineBFP.Domain.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CineBFP.Api.Controllers.User
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UserController : Controller
    {
        private readonly IUserService _userServices;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userServices, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _logger = logger;
        }

        [HttpPost("api/v1/user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsetUser([FromBody] UserInput request)
        {
            _logger.LogInformation($"UserController::Iniciando con insertar el usaurio.-->{request}");
            var response = await _userServices.User(request);
            _logger.LogInformation($"UserController::Fin con insertar el usaurio.");
            return Ok(response);
        }

        [HttpPost("api/v1/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginInput request)
        {
            _logger.LogInformation($"UserController::Iniciando con el logueo del usuario.-->{request}");
            var response = await _userServices.Login(request);
            _logger.LogInformation($"UserController::Fin con el logueo del usuario.-->{request}");
            return Ok(response);
        }

        [JwtAuthorize("Admin")]
        [HttpGet("api/v1/user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser()
        {
            var response = await _userServices.GetUser();
            return Ok(response);
        }

        [JwtAuthorize("Admin")]
        [HttpPut("api/v1/user/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserInput body)
        {
            var response = await _userServices.UpdateUser(id, body);
            return Ok(response);
        }

        [JwtAuthorize("Admin")]
        [HttpDelete("api/v1/user/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErroresDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var resp = await _userServices.DeleteUser(id);
            return Ok(resp);
        }
    }
}

