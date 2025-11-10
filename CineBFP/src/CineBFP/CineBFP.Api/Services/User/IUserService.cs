using CineBFP.Application.User.Models;
using CineBFP.Domain.User.DTOs;

namespace CineBFP.Api.Services.User
{
    public interface IUserService
    {
        Task<UserDto> User(UserInput request);
        Task<LoginDto> Login(LoginInput request);
        Task<List<GetUserDto>> GetUser();
        Task<UpdateUserDto> UpdateUser(int id, UpdateUserInput request);
        Task<DeleteUserDto> DeleteUser(int idUsuario);
    }
}
