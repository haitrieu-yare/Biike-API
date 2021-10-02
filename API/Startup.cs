using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Persistence;
using Application.Core;
using Application.Trips;
using Application.TripTransactions;
using Application;

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
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
			});
			services.AddDbContext<DataContext>(opt =>
			{
				opt.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
			});
			services.AddCors(opt =>
			{
				opt.AddPolicy("CorsPolicy", policy =>
				{
					policy
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowAnyOrigin();
				});
			});

			string pathToKey = string.Empty;

			if (_currentEnvironment.IsDevelopment())
			{
				pathToKey = Path.Combine(Directory.GetCurrentDirectory(),
					"keys", "firebase_admin_sdk_development.json");
			}
			else if (_currentEnvironment.IsProduction())
			{
				pathToKey = Path.Combine(Directory.GetCurrentDirectory(),
				"keys", "firebase_admin_sdk.json");
			}

			FirebaseApp.Create(new AppOptions()
			{
				Credential = GoogleCredential.FromFile(pathToKey)
			});

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

			services.AddMediatR(typeof(HistoryList.Handler).Assembly);
			services.AddAutoMapper(typeof(MappingProfiles).Assembly);
			services.AddScoped(typeof(Hashing));
			services.AddScoped(typeof(AutoCreateTripTransaction));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors("CorsPolicy");

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
