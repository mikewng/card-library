using card_library.Core.Application.Repository;
using card_library.Core.Application.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace card_library.Core.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("Default"))
            );

            services.AddScoped<IUnitOfWork>(sp => 
                sp.GetRequiredService<AppDbContext>()
            );

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
