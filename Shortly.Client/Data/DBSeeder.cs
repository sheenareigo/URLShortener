using Microsoft.AspNetCore.Identity;
using Shortly.Data;
using Shortly.Data.Models;
using Shortly.Client.Helpers.Roles;


namespace Shortly.Client.Data
{
    public static class DBSeeder
    {
        //public static void SeedDefaultData(IApplicationBuilder applicatiobuilder)
        //{
        //    using (var servicescope = applicatiobuilder.ApplicationServices.CreateScope())
        //    {
        //        var dbContext = servicescope.ServiceProvider.GetService<AppDbContext>();
        //        if (!dbContext.Users.Any())
        //        {
        //            dbContext.Users.Add(new User()
        //            {
        //                Name = "Sheena Reigo",
        //                Email = "sheenareigo@gmail.com",
        //                //Address = "4569 Metcalf Avenue, Mississauga, L5M 4L8"

        //            });
        //            dbContext.SaveChanges();
        //        }

        //        if (!dbContext.Urls.Any())
        //        {
        //            dbContext.Urls.Add(new Url()
        //            {
        //                OriginalLink = "xyz.com",
        //                ShortLink = "xxx",
        //                NoOfClicks = 10,
        //                DateCreated = DateTime.Now,
        //                UserId = dbContext.Users.First().Id
        //            }
        //                );
        //            dbContext.SaveChanges();
        //        }

        //    }

        //}

        public static async Task SeedDefaultUsersRoles(IApplicationBuilder applicatiobuilder)
        {
            using (var servicescope = applicatiobuilder.ApplicationServices.CreateScope())
            {
                var roleManager = servicescope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var userManager = servicescope.ServiceProvider.GetRequiredService<UserManager<User>>();


                //create user and role
                var userRole = Role.User;
                var userEmail = "user@shortly.com";

               // var userEmail = "user@shortly.com";
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = "user",
                        Email = userEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, "Test@123");
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                // Now add the role
                if (!(await userManager.IsInRoleAsync(user, userRole)))
                {
                    await userManager.AddToRoleAsync(user, userRole);
                }



                //create admin and role
                var adminRole = Role.Admin;
                var adminEmail = "admin@shortly.com";

                if (!await roleManager.RoleExistsAsync(adminRole))
                {

                    await roleManager.CreateAsync(new IdentityRole()
                    {

                        Name = adminRole
                    }
                    );
                }
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {

                    var admin = new User()
                    {
                        //Name = "Admin",
                        UserName = "admin",
                        Email = adminEmail,
                        EmailConfirmed = true

                    };

                    await userManager.CreateAsync(admin, "Admin@123");
                    await userManager.AddToRoleAsync(admin, adminRole);
                }


            }
        }
    }
}
