using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CineBFP.Domain.User.Interface.Repository;
using CineBFP.Infrastructure.User;
using CineBFP.Domain.Movie.Interface.Repository;
using CineBFP.Infrastructure.Movie;

namespace CineBFP.Infrastructure.IoC
{
    public static class DIIInfraestructure
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CineDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("CineDb")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();
            return services;
        }
    }
}
