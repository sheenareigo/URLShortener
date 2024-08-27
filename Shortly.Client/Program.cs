using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shortly.Client.Data;
using Shortly.Data;
using Shortly.Data.Models;
using Shortly.Data.Services;

namespace Shortly.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // configure dbcontext
            builder.Services.AddDbContext<Shortly.Data.AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
            
            });


            //configure authenication
            //identity service
            builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            //app cookie
            builder.Services.ConfigureApplicationCookie(options => {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Authentication/Login";
                options.SlidingExpiration = true;

                });

            
            //builder.Services.AddTransient();
           builder.Services.AddScoped<IUrlService, UrlService>();
           builder.Services.AddScoped<IUserService, UserService>();
            //builder.Services.AddSingleton<>

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //update default password settings
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;

                //lockour settings
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                //signin settings
                options.SignIn.RequireConfirmedEmail = true;

            });

            //external logins
            builder.Services.AddAuthentication().AddGoogle(options => {

                options.ClientId = builder.Configuration["Auth:Google:ClientID"];
                options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"]; ;
            }
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "specific",
                pattern: "{controller=Home}/{action=Index}/{userid}/{linkid}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //DBSeeder.SeedDefaultData(app);
            DBSeeder.SeedDefaultUsersRoles(app).Wait();

            app.Run();
        }
    }
}
