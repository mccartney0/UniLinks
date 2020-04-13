using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using System.Text;

using UniLink.API.Data;
using UniLink.API.Services;

namespace UniLink.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			// JWT Authentication
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = Configuration["JWT:Issuer"],

					ValidateAudience = true,
					ValidAudience = Configuration["JWT:Audience"],

					ValidateLifetime = true,

					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecurityKey"]))
				});

			// MySQL Database
			services.AddDbContext<DataContext>
			(
				options => options.UseMySql(Configuration["ConnectionString"],
				builder => builder.MigrationsAssembly("UniLink.API"))
			);

			services.AddControllers();

			// Services
			services.AddScoped<GenerateTokenService>();

			// Business
			// Code

			// Repositories
			// Code
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			using (IServiceScope scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
				dbContext.Database.Migrate();
			}

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			//app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}