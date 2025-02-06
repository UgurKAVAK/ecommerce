using API.Entity;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public static class SeedDatabase
    {
        public static async void Initialize(IApplicationBuilder app)
        {
            var userManager = app.ApplicationServices
                                .CreateScope()
                                .ServiceProvider
                                .GetRequiredService<UserManager<AppUser>>();

            var roleManager = app.ApplicationServices
                                .CreateScope()
                                .ServiceProvider
                                .GetRequiredService<RoleManager<AppRole>>();

            if (!roleManager.Roles.Any())
            {
                var customer = new AppRole { Name = "Customer" };
                var admin = new AppRole { Name = "Admin" };

                await roleManager.CreateAsync(customer);
                await roleManager.CreateAsync(admin);
            }

            if (!userManager.Users.Any())
            {
                var customer = new AppUser { Name = "Uğur KAVAK", UserName = "ugur", Email = "ugurkavak@gmail.com" };
                var admin = new AppUser { Name = "Admin", UserName = "admin", Email = "admin@gmail.com" };

                await userManager.CreateAsync(customer, "Ugur_123");
                // Birden çok role sahip olacak ise AddToRolesAsync
                await userManager.AddToRolesAsync(customer, ["Admin", "Customer"]);

                await userManager.CreateAsync(admin, "Admin_123");
                // Tek role sahip olacak ise AddToRoleAsync
                await userManager.AddToRoleAsync(admin, "Admin");
            }

        }
    }
}
