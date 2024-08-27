
using Microsoft.EntityFrameworkCore;
using Shortly.Data;
using Shortly.Data.Services;

namespace Shortly.Redirect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));

            });

            builder.Services.AddScoped<IUrlService, UrlService>();

            var app = builder.Build();

           
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //add redirection

            app.MapGet("/{path}", async (string path, IUrlService urlservice) =>
            {
                var urlobj = await urlservice.GetOriginalUrl(path);
                if (urlobj != null)
                {
                    await urlservice.Incrementclicks(urlobj.Id);
                    return Results.Redirect(urlobj.OriginalLink);
                }
                return Results.Redirect("/");
            });


            

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
