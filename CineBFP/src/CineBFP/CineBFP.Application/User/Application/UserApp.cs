using CineBFP.Application.Security;
using CineBFP.Domain.User.DTOs;
using CineBFP.Domain.User.Interface.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CineBFP.Application.User.Application
{
    public class UserApp : IUserApp
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwt;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<UserApp> _logger;

        public UserApp(IUserRepository userRepository, JwtTokenGenerator jwt, IHttpContextAccessor http, ILogger<UserApp> logger)
        {
            _userRepository = userRepository;
            _jwt = jwt;
            _http = http;
            _logger = logger;
        }

        public async Task<UserDto> User(UserDtoIn request)
        {
            if (!string.IsNullOrWhiteSpace(request.Contrasenia) && !request.Contrasenia.StartsWith("$2"))
            {
                request.Contrasenia = PasswordUtils.HashBcrypt(request.Contrasenia, workFactor: 12);
            }

            var ok = await _userRepository.User(request);
            return new UserDto
            {
                Success = ok,
                Message = ok ? "Usuario registrado correctamente." : "No se pudo registrar el Usuario."
            };
        }

        public async Task<LoginDto> Login(LoginDtoIn request)
        {
            var entity = await _userRepository.Login(request);
            if (entity is null)
            {
                _logger.LogWarning("Login fallido: correo no encontrado. Correo={Correo}", request.Correo);
                return new LoginDto { Success = false, Message = "Credenciales no son correctas.", Token = null };
            }
            if (entity is null)
                return new LoginDto { Token = null };

            if (!PasswordUtils.Verify(request.Contrasenia, entity.Contrasenia))
                return new LoginDto { Token = null };

            var roleText = entity.IdRol switch
            {
                1 => "Admin",
                2 => "Invitado",
                _ => entity.NombreRol ?? "Invitado"
            };

            var token = _jwt.Create(
                name: entity.FullName,
                email: entity.Correo,
                role: roleText,
                usuario: entity.UsuarioCreacion
            );

            return new LoginDto { Token = token };
        }

        public async Task<List<GetUserDto>> GetUser()
        {
            var principal = _http.HttpContext?.User
                ?? throw new InvalidOperationException("No hay usuario en el contexto.");

            var usuarioAdmin = principal.FindFirst("usuario")?.Value
                ?? principal.FindFirst(ClaimTypes.Name)?.Value
                ?? throw new InvalidOperationException("No se encontró el claim 'usuario'.");

            var rows = await _userRepository.GetUsers(usuarioAdmin);

            var list = rows.Select(r => new GetUserDto
            {
                IdUsuario = r.IdUsuario,
                FullName = r.FullName,
                Correo = r.Correo,
                Activo = (bool)r.Activo ? "Activo" : "No Activo",
            }).ToList();

            return list;
        }

        public async Task<UpdateUserDto> UpdateUser(int id, UpdateUserDtoIn request)
        {
            var user = _http.HttpContext?.User;
            var usuarioAdmin = user?.FindFirst("usuario")?.Value
                               ?? user?.FindFirst(ClaimTypes.Name)?.Value
                               ?? string.Empty;

            var dtoIn = new UpdateUserDtoIn
            {
                IdUsuario = id,
                FullName = request.FullName,
                Correo = request.Correo,
                Activo = request.Activo
            };

            var ok = await _userRepository.UpdateUser(usuarioAdmin, dtoIn);
            return new UpdateUserDto
            {
                Success = ok,
                Message = ok ? "Invitado actualizado." : "No se actualizó el invitado (verifique Id, rol=2 o pertenencia al admin)."
            };
        }

        public async Task<DeleteUserDto> DeleteUser(int idUsuario)
        {
            var user = _http.HttpContext?.User
                       ?? throw new InvalidOperationException("No hay usuario en el contexto.");

            var usuarioAdmin =
                user.FindFirst("usuario")?.Value ??
                user.FindFirst(ClaimTypes.Name)?.Value ??
                throw new InvalidOperationException("El token no contiene claim 'usuario'.");

            var ok = await _userRepository.DeleteUser(idUsuario, usuarioAdmin);

            return new DeleteUserDto
            {
                Success = ok,
                Message = ok
                    ? "Invitado eliminado."
                    : "No se pudo eliminar (no existe, no es INVITADO o no pertenece a este admin)."
            };
        }
    }
}

