namespace GameOfTournaments.Services
{
    public interface IEfCoreService<TEntity> : IService<TEntity>
        where TEntity : class
    {
    }
}