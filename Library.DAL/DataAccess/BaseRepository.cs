using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal abstract class BaseRepository
{
    protected readonly SqlConnection _connection;
    protected readonly Func<SqlTransaction?> _transaction;
    protected readonly Func<Task?> _openDbConnectionAsync;

    public BaseRepository(SqlConnection connection, Func<SqlTransaction?> transaction, Func<Task?> openDbConnection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        _openDbConnectionAsync = openDbConnection ?? throw new ArgumentNullException(nameof(_openDbConnectionAsync));
    }
}
