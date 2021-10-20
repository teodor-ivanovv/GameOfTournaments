namespace GameOfTournaments.Web.Controllers
{
    using GameOfTournaments.Services;
    using GameOfTournaments.Web.Cache.ApplicationUsers;
    using Microsoft.AspNetCore.Http;

    public class TournamentsController : Controller
    {
        public TournamentsController(IHttpContextAccessor httpContextAccessor, IAuthenticationService AuthenticationService, IApplicationUserCache applicationUserCache)
            : base(httpContextAccessor, AuthenticationService, applicationUserCache)
        {
        }
    }
}