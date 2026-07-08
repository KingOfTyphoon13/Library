namespace Library.Domain.DataAccess;

public interface IRepository
{
    Task<int> GetTotalEntries();
}
