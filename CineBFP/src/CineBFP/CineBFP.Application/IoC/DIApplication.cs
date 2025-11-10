using CineBFP.Application.Movie.Application;
using CineBFP.Application.Security;
using CineBFP.Application.User.Application;
using CineBFP.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CineBFP.Application.IoC
{
    public static class DIApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserApp, UserApp>();
            services.AddScoped<IMovieApp, MovieApp>();
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddSingleton<JwtTokenGenerator>();
            services.AddDomain(configuration);
            return services;
        }
    }
}
