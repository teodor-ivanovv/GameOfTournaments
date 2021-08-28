namespace GameOfTournaments.Data.Infrastructure
{
    public interface IIdentifiable<T>
    {
        T Id { get; set; }
    }
}