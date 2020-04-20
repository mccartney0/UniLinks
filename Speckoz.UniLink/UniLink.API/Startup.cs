using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using System.Text;

using UniLink.API.Business;
using UniLink.API.Business.Interfaces;
using UniLink.API.Data;
using UniLink.API.Repository;
using UniLink.API.Repository.Interfaces;
using UniLink.API.Services;
using UniLink.API.Services.Email;
using UniLink.API.Services.Email.Interfaces;
using UniLink.API.Utils;

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
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
				});

			// MySQL Database
			services.AddDbContext<DataContext>
			(
				options => options.UseMySql(Configuration["ConnectionString"],
				builder => builder.MigrationsAssembly("UniLink.API"))
			);

			// Injecoes do smtp/email
			services.Configure<ConfigEmailModel>(Configuration.GetSection("ConfigEmailModel"));
			services.AddTransient<ISendEmailService, SendEmailService>();

			// Seed
			services.AddScoped<DataSeeder>();

			services.AddControllers(x => x.InputFormatters.Insert(x.InputFormatters.Count, new TextPlainInputFormatter()));

			// Services
			services.AddScoped<GenerateTokenService>();
			services.AddScoped<SecurityService>();

			// Business
			services.AddScoped<IAccountBusiness, AccountBusiness>();
			services.AddScoped<ILessonBusiness, LessonBusiness>();
			services.AddScoped<IStudentBusiness, StudentBusiness>();
			services.AddScoped<ICourseBusiness, CourseBusiness>();

			// Repositories
			services.AddScoped<IAccountRepository, AccountRepository>();
			services.AddScoped<ILessonRepository, LessonRepository>();
			services.AddScoped<IStudentRepository, StudentRepository>();
			services.AddScoped<ICourseRepository, CourseRepository>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataSeeder dataSeeder)
		{
			using (IServiceScope scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
				dbContext.Database.Migrate();
			}

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				dataSeeder.Init();
			}

			//app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}