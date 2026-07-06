namespace Library.Domain.DataAccess;

public interface IUnitOfWork : IAsyncDisposable
{
    IBooksRepository Books { get; }
    IAuthorsRepository Authors { get; }
    IReviewsRepository Reviews { get; }

    Task ExecuteTransactionAsync(Func<Task> transaction);
}
