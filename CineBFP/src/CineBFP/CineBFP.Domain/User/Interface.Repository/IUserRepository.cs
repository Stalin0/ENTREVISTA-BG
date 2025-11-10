using CineBFP.Domain.User.DTOs;
using CineBFP.Domain.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.User.Interface.Repository
{
    public interface IUserRepository
    {
        Task<bool> User(UserDtoIn request);
        Task<LoginEntities> Login(LoginDtoIn request);
        Task<List<GetUserEntities>> GetUsers(string usuarioCreacion);
        Task<bool> UpdateUser(string usuarioAdmin, UpdateUserDtoIn request);
        Task<bool> DeleteUser(int idUsuario, string usuarioAdmin);
    }
}
