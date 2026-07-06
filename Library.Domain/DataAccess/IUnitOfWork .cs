namespace Library.Domain.DataAccess;

public interface IUnitOfWork : IAsyncDisposable
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    IReviewRepository Reviews { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
