using Domain.Contracts;
using Domain.Entities.IdentitiyModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Presistence.Data.DataSeed
{
    public class IdentityDataInitializer : IDataInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataInitializer> _logger;

        public IdentityDataInitializer(UserManager<ApplicationUser> userManager,
                                        RoleManager<IdentityRole> roleManager,
                                        ILogger<IdentityDataInitializer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // Seed default roles and users into the Identity database
        public async Task InitializeAsync()
        {
            try
            {
                // Check if roles already exist before creating them
                if (!_roleManager.Roles.Any())
                {
                    // Create default application roles
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }

                // Check if users already exist before seeding default users
                if (!_userManager.Users.Any())
                {
                    // Create Admin user
                    var user01 = new ApplicationUser()
                    {
                        DisplayName = "admin",
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        PhoneNumber = "01012131415",
                    };

                    // Create SuperAdmin user
                    var user02 = new ApplicationUser()
                    {
                        DisplayName = "superAdmin",
                        UserName = "superAdmin",
                        Email = "superAdmin@gmail.com",
                        PhoneNumber = "01045131415",
                    };

                    // Create users with passwords
                    await _userManager.CreateAsync(user01,"P@ssw0rd");
                    await _userManager.CreateAsync(user02, "P@ssw0rd");

                    // Assign roles to users
                    await _userManager.AddToRoleAsync(user01, "Admin");
                    await _userManager.AddToRoleAsync(user02, "SuperAdmin");
                }
            }
            catch (Exception ex) {
                _logger.LogError($"Error While Seeding Identity DataBase Message : {ex.Message}");
            }
        }
    }
}
