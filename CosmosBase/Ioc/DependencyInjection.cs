using CosmosBase.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

namespace CosmosBase
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCosmosBase(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            services.AddHttpContextAccessor();


            //Token MW
            var tokenOptions = configuration.GetSection("TokenOptions").Get<JwtOptions>();
            if (tokenOptions != null)
            {
                var key = Encoding.UTF8.GetBytes(tokenOptions.SecurityKey);
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudiences = tokenOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero

                    };

                    //options.Events = new JwtBearerEvents
                    //{
                    //    OnAuthenticationFailed = context =>
                    //    {
                    //        Console.WriteLine($"JWT HATA = {context.Exception.Message}");
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });
            }

            //SwaggerGen
            var title = Directory.GetParent(Directory.GetCurrentDirectory())?.Name;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{title} API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "To use JWT Token, enter the '{token}' format. Bearer prefix will add automatically"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });


            //Redis
            var redis = configuration.GetSection("Redis").Get<RedisConnection>();
            if (redis != null)
            {
                StringBuilder connString = new();
                connString.Append($"{redis.Host}:{redis.Port},defaultDatabase={redis.Database}");
                connString = string.IsNullOrEmpty(redis.Password) ? connString : connString.Append($",password={redis.Password}");

                services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connString.ToString()));
                services.AddScoped<ICacheRepository, RedisRepository>();
            }

            //Localization

            services.AddMemoryCache();
            services.AddSingleton<IStringLocalizer>(provider =>
            new LocalizationBuilder(
                    provider.GetRequiredService<IMemoryCache>(),
                    new System.Resources.ResourceManager(typeof(System.Resources.Resources)) 
                ));
            return services;
        }

        public static IApplicationBuilder UseCosmosBase(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();

            return app;

        }
    }
}
