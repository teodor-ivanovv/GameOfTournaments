namespace GameOfTournaments.Web.Controllers
{
    using System;
    using System.Net;
    using System.Security.Claims;
    using GameOfTournaments.Data.Infrastructure;
    using GameOfTournaments.Services;
    using GameOfTournaments.Services.Infrastructure;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public abstract class Controller : ControllerBase
    {
        private const int NegativeId = -1;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [NotNull]
        private readonly IApplicationUserCache _applicationUserCache;

        /// <summary>
        /// Gets an instance of <see cref="IAuthenticationService"/> used for current user authentication operations.
        /// </summary>
        protected IAuthenticationService AuthenticationService { get; }

        protected Controller(
            IHttpContextAccessor httpContextAccessor,
            IAuthenticationService authenticationService,
            IApplicationUserCache applicationUserCache)
        {
            this._applicationUserCache = applicationUserCache ?? throw new ArgumentNullException(nameof(applicationUserCache));
            this.AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            var httpContext = ExtractContextData(httpContextAccessor, out var ipAddress, out var integerId);

            var applicationUserCacheModel = this._applicationUserCache.Get(integerId);
            if (applicationUserCacheModel == null)
            {
                this.AuthenticationService.Set(new AuthenticationContext(false, integerId));
                return;
            }

            this.SetAuthenticated(integerId, httpContext, ipAddress, applicationUserCacheModel);
        }

        /// <summary>
        /// Returns either an <see cref="OkObjectResult"/> or <see cref="BadRequestObjectResult"/> based on the result of the given <paramref name="operationResult"/>.
        /// </summary>
        /// <param name="operationResult">The <see cref="IOperationResult{T}"/> providing necessary information according to a specified operation.</param>
        /// <typeparam name="T">The type used in the <paramref name="operationResult"/>.</typeparam>
        /// <returns>An <see cref="ActionResult{TValue}"/> representing either an <see cref="OkObjectResult"/> or <see cref="BadRequestObjectResult"/> based on the result of the given <paramref name="operationResult"/>.</returns>
        protected ActionResult<T> FromOperationResult<T>([NotNull] IOperationResult<T> operationResult)
        {
            if (operationResult == null)
                return this.BadRequest();

            if (operationResult.Success)
                return this.Ok(operationResult);

            return this.BadRequest(operationResult);
        }

        private static HttpContext ExtractContextData(IHttpContextAccessor httpContextAccessor, out IPAddress ipAddress, out int integerId)
        {
            var httpContext = httpContextAccessor?.HttpContext;
            var userId = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ipAddress = httpContext?.Connection.RemoteIpAddress;
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