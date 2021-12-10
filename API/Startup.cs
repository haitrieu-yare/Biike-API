using System;
using System.IO;
using Application;
using Application.Core;
using Application.Notifications;
using Application.PointHistory;
using Application.Trips;
using Application.TripTransactions;
using Application.Users;
using Application.Wallets;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Quartz;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _currentEnvironment = env;
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "API", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: '12345ABC')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    policy => { policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin(); });
            });

            string pathToKey = string.Empty;

            if (_currentEnvironment.IsDevelopment())
                pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "Keys",
                    "firebase_admin_sdk_development.json");
            else if (_currentEnvironment.IsProduction())
                pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "firebase_admin_sdk.json");

            var options = new AppOptions {Credential = GoogleCredential.FromFile(pathToKey)};
            FirebaseApp.Create(options);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.Authority = _config["Jwt:Firebase:ValidIssuer"];
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _config["Jwt:Firebase:ValidIssuer"],
                        ValidAudience = _config["Jwt:Firebase:ValidAudience"]
                    };
                });

            // ReSharper disable CommentTypo
            // Middleware này là để custom message trả về nếu như request không có thông tin authentication (lỗi 401)
            // ReSharper restore CommentTypo
            services.AddAuthorization()
                .AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultTransformer>();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                q.ScheduleJob<WalletJob>(trigger => trigger.WithIdentity("WalletJob", Constant.ReoccurredJob)
                    .StartNow()
                    .WithCronSchedule("0 0 0 1 1/4 ? *",
                        x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))
                    .WithDescription("Managing creating new wallet and expire old wallet"));

                q.ScheduleJob<AutoTripCancellationStartup>(trigger =>
                    trigger.WithIdentity("StartupTripAutoCancellation", Constant.OneTimeJob)
                        .StartNow()
                        .WithDescription("Managing creating auto trip cancellation on startup"));
                
                q.ScheduleJob<AutoMaxPointReset>(trigger => trigger.WithIdentity("AutoMaxPointResetJob", Constant.ReoccurredJob)
                    .StartNow()
                    .WithCronSchedule("0 0 0 1 1/1 ? *",
                        x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))
                    .WithDescription("Managing resetting biker max total point"));
            });

            // ASP.NET Core hosting
            services.AddQuartzServer(quartzHostedServiceOptions =>
            {
                // when shutting down we want jobs to complete gracefully
                quartzHostedServiceOptions.WaitForJobsToComplete = true;
            });

            services.AddMediatR(typeof(HistoryList.Handler).Assembly);
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddScoped(typeof(Hashing));
            services.AddScoped(typeof(AutoTripTransactionCreation));
            services.AddScoped(typeof(AutoPointHistoryCreation));
            services.AddScoped(typeof(NotificationSending));
            services.AddScoped(typeof(TripCancellationCheck));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            logger.LogInformation("App start at {Time}", DateTime.UtcNow.AddHours(7));
        }
    }
}