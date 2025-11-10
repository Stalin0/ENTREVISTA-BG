using CineBFP.Domain.Movie.DTOs;
using CineBFP.Domain.Movie.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain.Movie.Interface.Repository
{
    public interface IMovieRepository
    {
        Task<bool> InsertMovie(MovieDtoIn request);
        Task<List<GetMovieEntities>> GetMovie();
        Task<bool> DeleteMovie(int idPelicula);
    }
}
