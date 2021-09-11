namespace GameOfTournaments.Web.Cache.ApplicationUsers
{
    public interface IApplicationUserCache
    {
        ApplicationUserCacheModel Get(int id);

        void Cache(ApplicationUserCacheModel applicationUserCacheModel);
    }
}