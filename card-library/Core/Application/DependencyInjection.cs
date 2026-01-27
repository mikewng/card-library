using card_library.Core.Application.Services;
using card_library.Core.Application.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tasking_api.Main.Service;

namespace card_library.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGamesService, GamesService>();
        services.AddScoped<IDecksService, DecksService>();
        services.AddScoped<ICardsService, CardsService>();

        return services;
    }
}
