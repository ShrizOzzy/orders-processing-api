using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace OrderProcess.Infrastructure.Configurations
{
    namespace OrderProcessApi.Extensions
    {
        public static class SwaggerConfigExtensions
        {
            public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Order Processing API",
                        Version = "v1",
                        Description = "API for processing customer orders"
                    });

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Name = "JWT Authentication",
                        Description = "Enter JWT Bearer token",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    };

                    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        { securityScheme, Array.Empty<string>() }
                    });
                });

                return services;
            }
        }
    }
}
