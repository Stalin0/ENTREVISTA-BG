// CineBFP.Infrastructure/CineDbContext.cs
using Microsoft.EntityFrameworkCore;
using CineBFP.Domain.User.Entities;
using CineBFP.Domain.Movie.Entities;

namespace CineBFP.Infrastructure
{
    public class CineDbContext : DbContext
    {
        public CineDbContext(DbContextOptions<CineDbContext> options) : base(options) { }
        public virtual DbSet<UserEntities> sp_InsertUsuario { get; set; }
        public virtual DbSet<LoginEntities> Login { get; set; } = null!;
        public virtual DbSet<GetUserEntities> GetUser { get; set; }
        public virtual DbSet<UpdateUserEntities> UpdateUser { get; set; }
        public virtual DbSet<DeleteUserEntities> DeleteUser { get; set; }
        public virtual DbSet<MovieEntities> sp_InsertPelicula { get; set; }
        public virtual DbSet<GetMovieEntities> sp_ListPeliculas { get; set; } = null!;
        public virtual DbSet<DeleteMovieEntities> DeleteMovie { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntities>().HasNoKey();
            modelBuilder.Entity<LoginEntities>().HasNoKey().ToView(null);
            modelBuilder.Entity<GetUserEntities>().HasNoKey();
            modelBuilder.Entity<MovieEntities>().HasNoKey();
            modelBuilder.Entity<GetMovieEntities>().HasNoKey();
            modelBuilder.Entity<UpdateUserEntities>().HasNoKey();
            modelBuilder.Entity<DeleteUserEntities>().HasNoKey();
            modelBuilder.Entity<DeleteMovieEntities>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
