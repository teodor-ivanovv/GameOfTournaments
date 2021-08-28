namespace GameOfTournaments.Web
{
    using GameOfTournaments.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using ServiceCollectionExtensions;

    public class Startup
    {
        public Startup(IConfiguration configuration) { this.Configuration = configuration; }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var applicationSettings = services.GetApplicationSettings(this.Configuration);
            
            services.AddControllers();

            services.AddIdentity()
                .RegisterDbContextFactory(applicationSettings)
                .AddJwtAuthentication(applicationSettings)
                .AddApplicationServices();
            
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(applicationSettings.ConnectionString));
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameOfTournaments.Web", Version = "v1" }); });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameOfTournaments.Web v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.ApplyMigrations();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}