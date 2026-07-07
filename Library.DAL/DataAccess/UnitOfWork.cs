using Library.Domain.DataAccess;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Library.DAL.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly SqlConnection _connection;
    private SqlTransaction? _transaction;

    public IBooksRepository Books { get; }

    public IAuthorsRepository Authors { get; }

    public IReviewsRepository Reviews { get; }

    public UnitOfWork(string connectionString)
    {
        _connection = new SqlConnection(connectionString);

        Books = new BooksRepository(_connection, TransactionFactory, OpenConnectionAsync);
        Authors = new AuthorsRepository(_connection, TransactionFactory, OpenConnectionAsync);
        Reviews = new ReviewsRepository(_connection, TransactionFactory, OpenConnectionAsync);

    }

    public async Task ExecuteTransactionAsync(Func<Task> transaction)
    {
        await OpenConnectionAsync();

        _transaction = (SqlTransaction)await _connection.BeginTransactionAsync();

        try
        {
            await transaction();

            await CommitAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null) await _transaction.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private async Task CommitAsync()
    {
        await _transaction!.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    private async Task RollbackAsync()
    {
        await _transaction!.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    private SqlTransaction? TransactionFactory() => _transaction;

    private async Task OpenConnectionAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();
    }
}
