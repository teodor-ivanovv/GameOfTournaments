namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public abstract class Controller : ControllerBase
    {
        private const int NegativeId = -1;
        
        private readonly IApplicationUserCache applicationUserCache;

        protected IAuthenticationService AuthenticationService { get; }

        protected Controller(IHttpContextAccessor httpContextAccessor, IAuthenticationService AuthenticationService, IApplicationUserCache applicationUserCache)
        {
            this.applicationUserCache = applicationUserCache ?? throw new ArgumentNullException(nameof(applicationUserCache));
            this.AuthenticationService = AuthenticationService ?? throw new ArgumentNullException(nameof(AuthenticationService));
            
            var httpContext = httpContextAccessor?.HttpContext;
            var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = httpContext?.Connection?.RemoteIpAddress;
            var parsed = int.TryParse(userId, out var integerId);
            if (!parsed)
                integerId = NegativeId;

            var cachedUser = this.applicationUserCache.Get(integerId);
            if (cachedUser == null)
            {
                this.AuthenticationService.Set(new AuthenticationContext(false, integerId));
                return;
            }
            
            this.AuthenticationService.Set(
                new AuthenticationContext
                {
                    Id = integerId,
                    Authenticated = true,
                    ApplicationUser = httpContext?.User,
                    IpAddress = ipAddress?.ToString(),
                    Roles = cachedUser.Roles,
                });
        }
    }
}