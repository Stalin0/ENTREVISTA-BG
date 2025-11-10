using CineBFP.Domain.User.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Application.User.Application
{
    public interface IUserApp
    {
        Task<UserDto> User(UserDtoIn request);
        Task<LoginDto> Login(LoginDtoIn request);
        Task<List<GetUserDto>> GetUser();
        Task<UpdateUserDto> UpdateUser(int id, UpdateUserDtoIn request);
        Task<DeleteUserDto> DeleteUser(int idUsuario);

    }
}
