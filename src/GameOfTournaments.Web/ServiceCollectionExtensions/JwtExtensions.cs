namespace GameOfTournaments.Web.ServiceCollectionExtensions
{
    using System.Text;
    using Ardalis.GuardClauses;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection serviceCollection,
            ApplicationSettings applicationSettings)
        {
            Guard.Against.Null(applicationSettings, nameof(applicationSettings));

            var key = Encoding.ASCII.GetBytes(applicationSettings.JwtSecret);

            serviceCollection.AddAuthentication(
                    x =>
                    {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddJwtBearer(
                    x =>
                    {
                        x.RequireHttpsMetadata = false;
                        x.SaveToken = true;
                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(key), ValidateIssuer = false, ValidateAudience = false,
                        };
                    });

            return serviceCollection;
        }
    }
}