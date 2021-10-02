namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Net;
    using System.Security.Claims;
    using GameOfTournaments.Data.Infrastructure;
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
        private readonly IApplicationUserCache _applicationUserCache;

        protected IAuthenticationService AuthenticationService { get; }

        protected Controller(IHttpContextAccessor httpContextAccessor, IAuthenticationService AuthenticationService, IApplicationUserCache applicationUserCache)
        {
            this._applicationUserCache = applicationUserCache ?? throw new ArgumentNullException(nameof(applicationUserCache));
            this.AuthenticationService = AuthenticationService ?? throw new ArgumentNullException(nameof(AuthenticationService));
            
            var httpContext = ExtractContextData(httpContextAccessor, out var ipAddress, out var integerId);

            var cachedUser = this._applicationUserCache.Get(integerId);
            if (cachedUser == null)
            {
                this.AuthenticationService.Set(new AuthenticationContext(false, integerId));
                return;
            }
            
            this.SetAuthenticated(integerId, httpContext, ipAddress, cachedUser);
        }

        protected ActionResult<T> FromOperationResult<T>(IOperationResult<T> operationResult)
        {
            if (operationResult.Success)
                return this.Ok(operationResult);

            return this.BadRequest(operationResult);
        }

        private static HttpContext ExtractContextData(IHttpContextAccessor httpContextAccessor, out IPAddress ipAddress, out int integerId)
        {
            var httpContext = httpContextAccessor?.HttpContext;
            var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            ipAddress = httpContext?.Connection?.RemoteIpAddress;
            var parsed = int.TryParse(userId, out integerId);
            if (!parsed)
                integerId = NegativeId;

            return httpContext;
        }

        private void SetAuthenticated(int integerId, HttpContext httpContext, IPAddress ipAddress, ApplicationUserCacheModel cachedUser)
            => this.AuthenticationService.Set(
                new AuthenticationContext
                {
                    Id = integerId,
                    Authenticated = true, 
                    ApplicationUser = httpContext?.User,
                    IpAddress = ipAddress?.ToString(),
                    Permissions = cachedUser.Permissions,
                });
    }
}