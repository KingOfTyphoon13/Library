using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal abstract class BaseRepository
{
    protected readonly SqlConnection _connection;
    protected readonly Func<SqlTransaction?> _transaction;

    public BaseRepository(SqlConnection connection, Func<SqlTransaction?> transaction)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }
}
