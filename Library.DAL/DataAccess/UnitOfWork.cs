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

        Books = new BooksRepository(_connection, TransactionFactory);
        Authors = new AuthorsRepository(_connection, TransactionFactory);
        Reviews = new ReviewsRepository(_connection, TransactionFactory);

    }

    public async Task BeginTransactionAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();
        _transaction = (SqlTransaction)await _connection.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _transaction!.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        await _transaction!.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null) await _transaction.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private SqlTransaction? TransactionFactory() => _transaction;
}
