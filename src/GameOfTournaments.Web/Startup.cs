namespace GameOfTournaments.Web
{
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Services;
    using GameOfTournaments.Web.Hubs;
    using GameOfTournaments.Web.ServiceCollectionExtensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var applicationSettings = services.GetApplicationSettings(this.Configuration);

            services.AddControllers();

            services.AddIdentity()
                .RegisterDbContextFactory(applicationSettings)
                .RegisterDbContext(applicationSettings)
                .AddJwtAuthentication(applicationSettings)
                .AddApplicationServices();

            services.AddSignalR();

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameOfTournaments.Web", Version = "v1" }); });
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app
                    .UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameOfTournaments.Web v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication()
                .UseAuthorization();

            app.ApplyMigrations();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapHub<TournamentsHub>("/TournamentsHub");
                });
        }
    }
}