using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBFP.Domain
{
    public static class DIIDomain
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
        {
            //importar el core del negocio 
            //services.AddScoped(typeof(IBussinesCommand<>), typeof(SampleBussinesComand<>));

            


            return services;
        }
    }
}
