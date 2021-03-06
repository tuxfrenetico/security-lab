using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Site.Config;
using Site.Extentions;
using Site.Repository;
using Site.Services;

namespace Site
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      var jwtKey = Configuration["JwtKey"];
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey))
      };

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(
          options =>
          {
            options.TokenValidationParameters = tokenValidationParameters;
          }
        );

      services.AddControllers();

      services.AddSingleton<AppConfig>(new AppConfig(jwtKey));

      services.AddScoped<SimulationService>();
      services.AddScoped<UserService>();
      services.AddScoped<MailService>();

      services.AddScoped<SimulationRepository>();
      services.AddScoped<UserRepository>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseMiddleware(typeof(ExceptionHandler));

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}