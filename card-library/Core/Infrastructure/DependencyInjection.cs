using card_library.Core.Infrastructure.Repository;
using card_library.Core.Infrastructure.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace card_library.Core.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
