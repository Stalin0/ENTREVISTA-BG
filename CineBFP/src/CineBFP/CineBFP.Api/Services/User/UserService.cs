using CineBFP.Application.User.Application;
using CineBFP.Application.User.Models;   
using CineBFP.Domain.User.DTOs;
using System.Threading.Tasks;

namespace CineBFP.Api.Services.User
{ 

    public class UserService : IUserService
    {
        private readonly IUserApp _userApp;

        public UserService(IUserApp userApp)
        {
            _userApp = userApp;
        }

        public async Task<UserDto> User(UserInput request)
        {
            var dtoIn = new UserDtoIn
            {
                FullName = request.FullName,
                Correo = request.Correo,
                Contrasenia = request.Contrasenia,
                IdRol = request.IdRol,
                UsuarioCreacion = request.UsuarioCreacion
            };
            return await _userApp.User(dtoIn);
        }

        public async Task<LoginDto> Login(LoginInput request)
        {
            var dtoIn = new LoginDtoIn
            {
                Correo = request.Correo,
                Contrasenia = request.Contrasenia,
            };
            return await _userApp.Login(dtoIn);

        }

        public Task<List<GetUserDto>> GetUser()
                => _userApp.GetUser();

        public Task<UpdateUserDto> UpdateUser(int id, UpdateUserInput request)
            => _userApp.UpdateUser(id, new UpdateUserDtoIn
            {
                FullName = request.FullName,
                Correo = request.Correo,
                Activo = request.Activo
            });

        public Task<DeleteUserDto> DeleteUser(int idUsuario)
        => _userApp.DeleteUser(idUsuario);
    }
}
