namespace GameOfTournaments.Web.Cache.ApplicationUsers
{
    public interface IApplicationUserCache
    {
        long Count { get; }
        
        ApplicationUserCacheModel Get(int id);

        void Cache(ApplicationUserCacheModel applicationUserCacheModel);
    }
}