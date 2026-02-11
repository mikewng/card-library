using Amazon.S3;
using card_library.Core.Application.Models;
using card_library.Core.Application.Services;
using card_library.Core.Application.Services.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using tasking_api.Main.Service;

namespace card_library.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGamesService, GamesService>();
        services.AddScoped<IDecksService, DecksService>();
        services.AddScoped<ICardsService, CardsService>();
        services.AddScoped<ITextIconService, TextIconService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Configure AWS S3 settings
        services.Configure<AwsS3Settings>(configuration.GetSection(AwsS3Settings.SectionName));

        // Register AWS S3 client
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var settings = configuration.GetSection(AwsS3Settings.SectionName).Get<AwsS3Settings>()
                ?? throw new InvalidOperationException("AwsS3 configuration is missing");

            var config = new Amazon.S3.AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(settings.Region)
            };

            return new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
        });

        // Register file storage service
        services.AddScoped<IFileStorageService, S3FileStorageService>();

        // Register JWT configuration
        var jwtSection = configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        return services;
    }
}
