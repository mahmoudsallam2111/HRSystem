using HRSystem.Domain.Entities;
using HRSystem.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : 
        IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, IdentityUserRole<string>,
            IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions)
            : base(contextOptions) { }

        public DbSet<Employee> Employees => Set<Employee>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            HandleDecimalTypeProperty(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);    // apply all configuration that implement IEntityTypeConfiguration interface
        }

        private static void HandleDecimalTypeProperty(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
        }
    }
}
