
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sakan.Application.Services;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using Sakan.Infrastructure.Repositories;
using System.Text;

namespace Sakan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connection = builder.Configuration.GetConnectionString("connection");
            // Add services to the container.

            builder.Services.AddControllers(option =>
            {
              //  option.Filters.Add();
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = builder.Configuration["jwt:issuer"],
                    ValidAudience = builder.Configuration["jwt:audiance"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"]))

                };

            }).AddGoogle(googleoption =>
            {
                googleoption.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleoption.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                googleoption.CallbackPath = "/sakanak";
            });


            // conect with dbcontext 
            builder.Services.AddDbContext<sakanContext>(option =>
            {
                option.UseSqlServer(connection);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<sakanContext>().AddDefaultTokenProviders();

            builder.Services.AddCors(option =>
            {
                option.AddPolicy("s", o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.AllowedForNewUsers = false;
                
            });



            builder.Services.AddScoped<ITestRepo, TestRepo>();
            builder.Services.AddScoped<IProfile, ProfileRepo>();
            builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<IProfileService, Userprofileservice>(); 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            
            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseCors("s");

            app.MapControllers();

            app.Run();
        }
    }
}
