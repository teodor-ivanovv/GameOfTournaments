namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public abstract class Controller : ControllerBase
    {
        protected IAuthenticationService AuthenticationService { get; }

        protected Controller(IHttpContextAccessor httpContextAccessor, IAuthenticationService AuthenticationService)
        {
            var httpContext = httpContextAccessor?.HttpContext;
            this.AuthenticationService = AuthenticationService ?? throw new ArgumentNullException(nameof(AuthenticationService));
            var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = httpContext?.Connection?.RemoteIpAddress;
            var parsed = int.TryParse(userId, out var integerId);
            if (!parsed)
                integerId = -1;
            
            this.AuthenticationService.Set(
                new AuthenticationContext
                {
                    Id = integerId,
                    ApplicationUser = httpContext?.User,
                    IpAddress = ipAddress?.ToString(),
                });
        }
    }
}