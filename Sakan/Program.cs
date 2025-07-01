using Microsoft.EntityFrameworkCore;
using Sakan.Infrastructure.Services;
using Sakan.Application.Interfaces;
using Sakan.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Sakan.Application.Services;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Hubs;
using Sakan.Infrastructure.Repositories;
using System.Text;
using System.Security.Claims;
using Sakan.Domain.IUnitOfWork;
using Sakan.Infrastructure.UnitOfWork;
using Sakan.Application.Mapper;


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

            //builder.Services.AddControllers(option =>
            //{
            //    //  option.Filters.Add();
            //});

            builder.Services.AddControllers();
            builder.Services.AddScoped<IListingDetailsService, ListingDetailsService>();
            builder.Services.AddScoped<IRoomDetailsService, RoomDetailsService>();
            builder.Services.AddScoped<IBookingRequestService, BookingRequestService>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();


            builder.Services.AddScoped<IImageKitService, ImageKitService>();
            builder.Services.AddScoped<IListRepository, ListingRepo>();
            builder.Services.AddScoped<IListingService, ListingService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();


            builder.Services.AddScoped<IHostListingService, HostListingService>();



            //builder.Services.AddDbContext<sakanContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["jwt:issuer"],
                    ValidAudience = builder.Configuration["jwt:audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])
                    ),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };


            //}).AddGoogle(googleoption =>
            //{
            //    googleoption.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            //    googleoption.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            //    googleoption.CallbackPath = "/signin-google";
            //    // 🔥 THIS disables redirect to Account/Login for APIs
            //    googleoption.Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            Console.WriteLine("❌ JWT validation failed: " + context.Exception.Message);
            //            return Task.CompletedTask;
            //        },
            //        OnChallenge = context =>
            //        {
            //            context.HandleResponse(); // suppress default redirect
            //            context.Response.StatusCode = 401;
            //            context.Response.ContentType = "application/json";
            //            return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
            //        }
            //    };

            });




            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.SaveToken = true;
            //    options.RequireHttpsMetadata = false;

            //    options.Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            Console.WriteLine("❌ Token validation failed: " + context.Exception.Message);
            //            return Task.CompletedTask;
            //        },
            //        OnTokenValidated = context =>
            //        {
            //            Console.WriteLine("✅ Token validated for: " +
            //                context.Principal.Identity.Name);
            //            return Task.CompletedTask;
            //        }
            //    };

            //    options.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        //ValidIssuer = builder.Configuration["jwt:issuer"],
            //        //ValidAudience = builder.Configuration["jwt:audience"],
            //        //IssuerSigningKey = new SymmetricSecurityKey(
            //        //    Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"]))

            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,

            //        ValidIssuer = builder.Configuration["jwt:issuer"],
            //        ValidAudience = builder.Configuration["jwt:audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(
            //            Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])),

            //        // 👇 This tells ASP.NET to map `ClaimTypes.NameIdentifier` correctly
            //        NameClaimType = ClaimTypes.NameIdentifier,
            //        RoleClaimType = ClaimTypes.Role

            //    };

            //});


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
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                              //.AllowCredentials();
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


            builder.Services.AddScoped<IImageKitService, ImageKitService>();

            builder.Services.AddScoped<ITestRepo, TestRepo>();
            builder.Services.AddScoped<IProfile, ProfileRepo>();
            builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<IProfileService, Userprofileservice>();
            builder.Services.AddScoped<IMessage, MessageRepo>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            //builder.Services.AddScoped<IHostDashboard, HostDashboardRepo>();
            //builder.Services.AddScoped<IHostDashboardService, HostDashboardService>();
            builder.Services.AddScoped<IListingRepository, ListingRepository>();
            builder.Services.AddScoped<IListingService, ListingService>();
            builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
            builder.Services.AddScoped<IAmenityService, AmenityService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


            builder.Services.AddScoped(provider =>
            {
                return new Imagekit.Sdk.ImagekitClient(
                    publicKey: builder.Configuration["ImageKit:PublicKey"],
                    privateKey: builder.Configuration["ImageKit:PrivateKey"],
                    urlEndPoint: builder.Configuration["ImageKit:UrlEndpoint"]
                );
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

            //app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseCors("AllowFrontend");
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chat");
            app.MapControllers();

            app.Run();
        }
    }
}