using CineBFP.Api.Services.Movie;
using CineBFP.Api.Services.User;
using CineBFP.Application.IoC;
using CineBFP.Infrastructure.IoC;
using Microsoft.OpenApi.Models;

namespace CineBFP.Api.Extentions
{
    public static class ServiceExtentions
    {
        const string corsOrigins = "_AllowPolicy";
        public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
        {

            builder.Services.AddSwaggerGen(options =>
            {
                var title = builder.Configuration["OpenApi:info:title"] + "";
                var description = builder.Configuration["OpenApi:info:description"] + "";
                var termsOfService = new Uri(builder.Configuration["OpenApi:info:termsOfService"] + "" + "");
                var contact = new OpenApiContact
                {
                    Name = builder.Configuration["OpenApi:info:contact:name"] + "",
                    Url = new Uri(builder.Configuration["OpenApi:info:contact:url"] + "" + ""),
                    Email = builder.Configuration["OpenApi:info:contact:email"] + ""
                };
                var license = new OpenApiLicense
                {
                    Name = builder.Configuration["OpenApi:info:License:name"] + "",
                    Url = new Uri(builder.Configuration["OpenApi:info:License:url"] + "" + "")
                };
                options.SwaggerDoc(builder.Configuration["OpenApi:info:version"] + "", new OpenApiInfo
                {
                    Version = builder.Configuration["OpenApi:info:version"] + "",
                    Title = title,
                    Description = description,
                    TermsOfService = termsOfService,
                    Contact = contact,
                    License = license
                }
                );
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Put **_ONLY_** your JWT Bearer **_token_** on textbox below! \r\n\r\n\r\n Example: \"Value: **12345abcdef**\"",

                };
                options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));
            });

            return builder;
        }

        public static WebApplication UseHttpPipeline(this WebApplication builder)
        {

            if (builder.Environment.IsDevelopment())
            {
                builder.UseSwagger();
                builder.UseSwaggerUI(c =>
                {
                    string version = builder.Configuration["OpenApi:info:version"] + "" + "";
                    c.SwaggerEndpoint($"v1/swagger.json", version);
                });
            }

            return builder;
        }


        // Configure the HTTP request pipeline.



        public static WebApplication UseCustomExceptionHandler(this WebApplication app)
        {

            app.UseExceptionHandler();

            return app;

        }


        public static WebApplication UseCustomCors(this WebApplication app)
        {

            app.UseCors(corsOrigins);

            return app;
        }
        public static WebApplicationBuilder SetCors(this WebApplicationBuilder builder)
        {


            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: corsOrigins,
                    policy =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            return builder;

        }
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)




        {

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMovieService, MovieService>();
            return services;

        }
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(); 
                                  
        }

        public static WebApplicationBuilder SetRegisterDependencies(this WebApplicationBuilder builder)
        {


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

            return builder;



        }

    }
}