using CineBFP.Domain.User.DTOs;
using CineBFP.Domain.User.Entities;
using CineBFP.Domain.User.Interface.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Infrastructure.User
{
    public class UserRepository : IUserRepository
    {
        private readonly CineDbContext _context;

        public UserRepository(CineDbContext context)
        {
            _context = context;
        }

        public async Task<bool> User(UserDtoIn request)
        {
            var idRol = request.IdRol ?? 1; 

            var parameters = new[]
            {
                new SqlParameter("@FullName", SqlDbType.VarChar, 100) { Value = request.FullName },
                new SqlParameter("@Correo", SqlDbType.VarChar, 100) { Value = request.Correo },
                new SqlParameter("@Contrasenia", SqlDbType.VarChar, 255) { Value = request.Contrasenia },
                new SqlParameter("@IdRol", SqlDbType.Int) { Value = idRol },
                new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar, 100) { Value = request.UsuarioCreacion }
            };

            var inserted = await _context
                .sp_InsertUsuario
                .FromSqlRaw(
                    "EXEC dbo.sp_InsertUsuario @FullName, @Correo, @Contrasenia, @IdRol, @UsuarioCreacion",
                    parameters)
                .ToListAsync();

            return inserted.Any();
        }

        public async Task<LoginEntities?> Login(LoginDtoIn request)
        {
            var rows = await _context.Login
                .FromSqlInterpolated($"EXEC dbo.sp_LoginUsuario @Correo={request.Correo}")
                .AsNoTracking()
                .ToListAsync();

            return rows.FirstOrDefault();
        }

        public async Task<List<GetUserEntities>> GetUsers(string usuarioCreacion)
        {
            var rows = await _context.GetUser
                .FromSqlInterpolated($"EXEC dbo.sp_ListUsuarios @UsuarioCreacion={usuarioCreacion}")
                .AsNoTracking()
                .ToListAsync();

            return rows; 
        }


        public async Task<bool> UpdateUser(string usuarioAdmin, UpdateUserDtoIn request)
        {
            var p = new[]
            {
                new SqlParameter("@IdUsuario", SqlDbType.Int){ Value = request.IdUsuario },
                new SqlParameter("@UsuarioAdmin", SqlDbType.VarChar,100){ Value = usuarioAdmin },

                new SqlParameter("@FullName", SqlDbType.VarChar,100)
                {
                    Value = (object?)request.FullName ?? DBNull.Value, IsNullable = true
                },
                new SqlParameter("@Correo", SqlDbType.VarChar,100)
                {
                    Value = (object?)request.Correo ?? DBNull.Value, IsNullable = true
                },
                new SqlParameter("@Activo", SqlDbType.Bit)
                {
                    Value = (object?)request.Activo ?? DBNull.Value, IsNullable = true
                }
            };

            var rows = await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_UpdateUsuarioInvitado @IdUsuario, @UsuarioAdmin, @FullName, @Correo, @Activo", p);

            return rows > 0;
        }
        public async Task<bool> DeleteUser(int idUsuario, string usuarioAdmin)
        {
            var p = new[]
            {
            new SqlParameter("@IdUsuario", SqlDbType.Int){ Value = idUsuario },
            new SqlParameter("@UsuarioAdmin", SqlDbType.VarChar, 100){ Value = usuarioAdmin }
        };

            var res = await _context.DeleteUser
                .FromSqlRaw("EXEC dbo.sp_DeleteUsuarioInvitado @IdUsuario, @UsuarioAdmin", p)
                .AsNoTracking()
                .ToListAsync();

            var rows = res.FirstOrDefault()?.RowsAffected ?? 0;
            return rows > 0;
        }

    }
}

