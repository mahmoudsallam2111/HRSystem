using HRSystem.Application.Services.Identity;
using HRSystem.Infrastructure.Persistence.Context;
using HRSystem.Infrastructure.Persistence.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRSystem.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services , IConfiguration configuration)
        {
             services.AddDbContext<ApplicationDbContext>(options =>
             {
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
             })
                .AddTransient<ApplicationDbSeeder>();   // when application start the seeder class is registered

            return services;
        }
    }
}
