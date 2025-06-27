using Microsoft.EntityFrameworkCore;
using Sakan.Infrastructure.Services;
using Sakan.Application.Interfaces;
using Sakan.Infrastructure.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sakan.Application.Services;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Hubs;
using Sakan.Infrastructure.Models;
using Sakan.Infrastructure.Repositories;
using System.Text;

namespace Sakan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "AllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);

            var connection = builder.Configuration.GetConnectionString("connection");
            // Add services to the container.

            builder.Services.AddControllers(option =>
            {
              //  option.Filters.Add();
            });
            builder.Services.AddControllers();
            builder.Services.AddScoped<IListingDetailsService, ListingDetailsService>();
            builder.Services.AddScoped<IRoomDetailsService, RoomDetailsService>();
            builder.Services.AddScoped<IBookingRequestService, BookingRequestService>();


            builder.Services.AddDbContext<sakanContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSignalR();



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

            });


            // conect with dbcontext 
            builder.Services.AddDbContext<sakanContext>(option =>
            {
                option.UseSqlServer(connection);
            });

            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();


            //swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<sakanContext>().AddDefaultTokenProviders();

            //builder.Services.AddCors(option =>
            //{
            //    option.AddPolicy("s", o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            //});

           // Add CORS policy
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(name: MyAllowSpecificOrigins,
                        policy =>
                        {
                            policy.WithOrigins("http://localhost:4200")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials();
                        });
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
            builder.Services.AddScoped<IMessage, MessageRepo>();
            builder.Services.AddScoped<IMessageService, MessageService>();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy
                        .WithOrigins("http://localhost:4200") // Angular dev server
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
                options.RoutePrefix = "";
            });

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);
            app.MapHub<ChatHub>("/chat");
            app.MapControllers();

            app.Run();
        }
    }
}
