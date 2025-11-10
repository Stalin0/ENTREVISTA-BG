using CineBFP.Domain.Movie.DTOs;
using CineBFP.Domain.Movie.Entities;
using CineBFP.Domain.Movie.Interface.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Infrastructure.Movie
{
    public class MovieRepository : IMovieRepository
    {
        private readonly CineDbContext _context;

        public MovieRepository(CineDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetMovieEntities>> GetMovie()
        {
            var rows = await _context.sp_ListPeliculas
                .FromSqlRaw("EXEC dbo.sp_ListPeliculas")
                .ToListAsync();

            return rows;
        }

        public async Task<bool> InsertMovie(MovieDtoIn request)
        {
            var p = new[]
            {
        new SqlParameter("@Titulo", SqlDbType.VarChar,150){Value=request.Titulo},
        new SqlParameter("@Descripcion", SqlDbType.VarChar,500){Value=request.Descripcion ?? (object)DBNull.Value},
        new SqlParameter("@Director", SqlDbType.VarChar,100){Value=request.Director ?? (object)DBNull.Value},
        new SqlParameter("@Anio", SqlDbType.Int){Value=request.Anio},
        new SqlParameter("@Genero", SqlDbType.VarChar,100){Value=request.Genero ?? (object)DBNull.Value},
        new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar,100){Value=request.UsuarioCreacion},
        };

            var rows = await _context.Database
                .ExecuteSqlRawAsync("EXEC dbo.sp_InsertPelicula @Titulo, @Descripcion, @Director, @Anio, @Genero, @UsuarioCreacion", p);

            return rows > 0; 
        }

        public async Task<bool> DeleteMovie(int idPelicula)
        {
            var p = new[]
            {
                new SqlParameter("@IdPelicula", SqlDbType.Int){ Value = idPelicula }
            };

            var res = await _context.DeleteMovie
                .FromSqlRaw("EXEC dbo.sp_DeletePelicula @IdPelicula", p)
                .AsNoTracking()
                .ToListAsync();

            var rows = res.FirstOrDefault()?.RowsAffected ?? 0;
            return rows > 0;
        }
    }
}
