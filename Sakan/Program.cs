using Imagekit.Sdk;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sakan.Application.Interfaces;
using Sakan.Application.Interfaces.Admin;
using Sakan.Application.Interfaces.Host;
using Sakan.Application.Interfaces.User;
using Sakan.Application.Mapper;
using Sakan.Application.Services;
using Sakan.Application.Services.Admin;
using Sakan.Controllers;
using Sakan.Domain.Interfaces;
using Sakan.Domain.IUnitOfWork;
using Sakan.Domain.Models;
using Sakan.Hubs;
using Sakan.Infrastructure.Models;
using Sakan.Infrastructure.Repositories;
using Sakan.Infrastructure.Services;
using Sakan.Infrastructure.Services.Admin;
using Sakan.Infrastructure.Services.Host;
using Sakan.Infrastructure.Services.User;
using Sakan.Infrastructure.UnitOfWork;
using Stripe;
using System.Security.Claims;
using System.Text;
using ReviewService = Sakan.Application.Services.ReviewService; // لتجنب التعارض

namespace Sakan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("connection");
            var corsPolicyName = "AllowSpecificOrigins";

            // 1. تسجيل الخدمات (Services & Repositories)
            #region Services and Repositories Registration
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            builder.Services.AddSignalR();


            // إضافة AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // إضافة الخدمات الخاصة بالمشروع
            builder.Services.AddScoped<IListingDetailsService, ListingDetailsService>();
            builder.Services.AddScoped<IRoomDetailsService, RoomDetailsService>();
            builder.Services.AddScoped<IBookingRequestService, BookingRequestService>();
            builder.Services.AddScoped<IImageKitService, ImageKitService>();
            builder.Services.AddScoped<IListingService, ListingService>();
            builder.Services.AddScoped<IHostBookingService, HostBookingService>();
            builder.Services.AddScoped<IHostReviewsService, HostReviewsService>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            builder.Services.AddScoped<IAdminListingService, AdminListingService>();
            builder.Services.AddScoped<IAdminApproveListingService, AdminApproveListingService>();
            builder.Services.AddScoped<IUserReviewService, UserReviewService>();
            builder.Services.AddScoped<IAdminHostsService, AdminHostsApproveService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IHostListingService, HostListingService>();
            builder.Services.AddScoped<IProfileService, Userprofileservice>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IHostDashboardService, HostDashboardService>();
            builder.Services.AddScoped<IAmenityService, AmenityService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IEmailService, SendGridEmailService>();
            builder.Services.AddScoped<ISupportTicketService, SupportTicketService>();
            builder.Services.AddScoped<IAdminUsersService, AdminUsersService>();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            builder.Services.Configure<AiController.OpenAIOptions>(builder.Configuration.GetSection("OpenAI"));

            // إضافة المستودعات (Repositories)
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IListRepository, ListingRepo>();
            builder.Services.AddScoped<IBookingRequestRepository, BookingRequestRepository>();
            builder.Services.AddScoped<IListingRepository, ListingRepository>();
            builder.Services.AddScoped<ITestRepo, TestRepo>();
            builder.Services.AddScoped<IProfile, ProfileRepo>();
            builder.Services.AddScoped<IMessage, MessageRepo>();
            builder.Services.AddScoped<IHostDashboard, HostDashboardRepo>();
            builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();

            // إضافة UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // إعداد ImageKit
            builder.Services.AddSingleton<ImagekitClient>(sp =>
                new ImagekitClient(
                    sp.GetRequiredService<IConfiguration>()["ImageKit:PublicKey"],
                    sp.GetRequiredService<IConfiguration>()["ImageKit:PrivateKey"],
                    sp.GetRequiredService<IConfiguration>()["ImageKit:UrlEndpoint"]
                ));

            #endregion

            // 2. إعداد قاعدة البيانات و Identity
            #region Database and Identity Setup
            builder.Services.AddDbContext<sakanContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<sakanContext>()
            .AddDefaultTokenProviders();
            #endregion

            // 3. إعداد المصادقة (JWT and Google)
            #region Authentication Setup
            builder.Services.AddAuthentication(options =>
            {
                // اجعل JWT هو الافتراضي للـ API
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["jwt:issuer"],
                    ValidAudience = builder.Configuration["jwt:audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])),
                };
                // أضفنا Events للتحقق من الأخطاء في الكونسول
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("❌ JWT Validation failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("✅ Token validated successfully for user: " + context.Principal.Identity.Name);
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

            // إعدادات خاصة بمنع إعادة التوجيه للـ API
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    return Task.CompletedTask;
                };
            });
            #endregion

            // 4. إعداد سياسة CORS
            #region CORS Policy Setup
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicyName, policy =>
                {
                    // اسمح فقط لمصدر الواجهة الأمامية بالوصول
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()  // اسمح بأي هيدر (مهم لـ Authorization)
                          .AllowAnyMethod()  // اسمح بأي نوع طلب (GET, POST, etc.)
                          .AllowCredentials(); // مهم لـ SignalR
                });
                options.AddPolicy(name: corsPolicyName,
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
            #endregion

            // 5. إعداد Swagger
            #region Swagger Setup
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #endregion

            builder.Services.Configure<AiController.OpenAIOptions>(
                builder.Configuration.GetSection("OpenAI"));

            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            var app = builder.Build();

            // 6. إعداد خط أنابيب الطلبات (Request Pipeline)
            // الترتيب هنا مهم جدًا
            #region Request Pipeline Configuration
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // استخدم سياسة CORS قبل المصادقة
            app.UseCors(corsPolicyName);

            // استخدم المصادقة والتفويض
            app.UseAuthentication();
            app.UseAuthorization();

            // ربط الـ Controllers و Hubs
            app.MapControllers();
            app.MapHub<ChatHub>("/ChatHub");
            #endregion

            app.Run();
        }
    }
}