using HRSystem.Common.Authorization;
using HRSystem.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Persistence.Context
{
    public class ApplicationDbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public ApplicationDbSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = applicationDbContext;
        }

        public async Task SeedDatabaseAsync()
        {
            // check pending db migration
            await CheckAndApplyPendingMigrationAsync();
            // seed roles with permessions
            await SeedRolesAsync();
            // seed users
            await SeedBasicUserAsync();
            await SeedAdminUserAsync();
        }


        private async Task CheckAndApplyPendingMigrationAsync()
        {
            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                await _dbContext.Database.MigrateAsync();
            }
        }

        private async Task SeedRolesAsync()
        {
            foreach (var roleName in AppRoles.DeafultRoles)
            {
                if (await _roleManager.Roles.FirstOrDefaultAsync(r=>r.Name == roleName) is not ApplicationRole applicationRole)
                {
                    applicationRole = new ApplicationRole()
                    {
                        Name = roleName,
                        Description = $"{roleName} role"
                    };

                    await _roleManager.CreateAsync(applicationRole);
                }

                // assign permession to role
                if (roleName == AppRoles.Admin)
                    await AssignPermessionToRoleAsync(applicationRole,AppPermessions.AdminPermessions);
                else
                    await AssignPermessionToRoleAsync(applicationRole, AppPermessions.BasicPermessions);
            }
        }

        private async Task AssignPermessionToRoleAsync(ApplicationRole role , IReadOnlyList<AppPermession> permessions)
        {
            // get current claims for this role and update the claims
            var currentClaims = await _roleManager.GetClaimsAsync(role);    

            // then assign the permession(claim) to the role
            foreach (var permession in permessions)   // note permession is a claim type 
            {
                if (!currentClaims.Any(claim=>claim.Type == AppClaim.Permession && claim.Value == permession.Name))
                {
                    await _dbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = AppClaim.Permession,
                        ClaimValue = permession.Name,
                        Description = permession.Description,
                        Group = permession.Group
                    });

                    await _dbContext.SaveChangesAsync();
                }
            }

        }

        private async Task SeedAdminUserAsync()
        {
            var adminUserName = AppCredentials.Email[..AppCredentials.Email.IndexOf("@")].ToLowerInvariant();
            var adminUser = new ApplicationUser
            {
                FirstName = "Mahmoud",
                LastName = "Sallam",
                Email = AppCredentials.Email,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = AppCredentials.Email.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true,
            };

            // create the user
            if (!await _userManager.Users.AnyAsync(u=>u.Email == AppCredentials.Email))
            {
                var password = new PasswordHasher<ApplicationUser>();
                adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.Password);
                await _userManager.CreateAsync(adminUser);
            }
            // assign role to user

            if (!await _userManager.IsInRoleAsync(adminUser , AppRoles.Guest) || !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
            {
                await _userManager.AddToRolesAsync(adminUser, AppRoles.DeafultRoles);
            }

        }

        private async Task SeedBasicUserAsync()
        {
            var basicUser = new ApplicationUser
            {
                FirstName = "Hager",
                LastName = "Sallam",
                Email = "Hager@gamil.com",
                UserName = "HagerSallam",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = "HAGERSALLAM@GMAIL.COM",
                NormalizedUserName = "HAGER",
                IsActive = true
            };

            if (!await _userManager.Users.AnyAsync(u => u.Email == "Hager@gamil.com"))
            {
                var password = new PasswordHasher<ApplicationUser>();
                basicUser.PasswordHash = password.HashPassword(basicUser, AppCredentials.Password);
                await _userManager.CreateAsync(basicUser);
            }

            // Assign role to user
            if (!await _userManager.IsInRoleAsync(basicUser, AppRoles.Guest))
            {
                await _userManager.AddToRoleAsync(basicUser, AppRoles.Guest);
            }
        }
    }
}
