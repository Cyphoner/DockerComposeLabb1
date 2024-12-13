
using DockerComposeLabb1.Data;
using DockerComposeLabb1.Models;
using Microsoft.EntityFrameworkCore;

namespace DockerComposeLabb1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ProductContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProductContext>();
                context.Database.Migrate();

                if(!context.Products.Any())
                {
                    context.Products.AddRange(
                        new Product { Name = "Coca-Cola", Price = 10 },
                        new Product { Name = "Redbull", Price = 20 },
                        new Product { Name = "Monster Energy", Price = 25 }
                        );

                    context.SaveChanges();
                }

            }


            app.MapGet("/products", async (ProductContext db) => await db.Products.ToListAsync());

            app.MapPost("/products", async (Product product, ProductContext db) =>
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();

                return Results.Ok();
            });

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.Run();
        }
    }
}
