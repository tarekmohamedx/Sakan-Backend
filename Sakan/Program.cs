using Microsoft.EntityFrameworkCore;
using Sakan.Application.Mapper;
using Sakan.Application.Services;
using Sakan.Application.Services_Interfaces;
using Sakan.Domain.Interfaces;
using Sakan.Infrastructure.MyContext;
using Sakan.Infrastructure.Repositories;
namespace Sakan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularLocalhost",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<sakanContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IListingRepository, ListingRepository>();
            builder.Services.AddScoped<IListingService, ListingService>();
            builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
            builder.Services.AddScoped<IAmenityService, AmenityService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowAngularLocalhost");


            app.MapControllers();

            app.Run();
        }
    }
}
