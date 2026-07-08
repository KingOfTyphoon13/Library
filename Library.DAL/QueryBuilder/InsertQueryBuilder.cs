using System.Text;

namespace Library.DAL.QueryBuilder;

public sealed class InsertQueryBuilder
{
    private string _table = "";
    private readonly List<string> _columns = [];
    private string? _outputColumn;
    private readonly Dictionary<string, object> _parameters = [];

    public static InsertQueryBuilder Create() => new();

    public InsertQueryBuilder Into(string table)
    {
        _table = table;
        return this;
    }

    public InsertQueryBuilder Value(string column, string paramName, object? value)
    {
        _columns.Add(column);
        _parameters[paramName] = value ?? DBNull.Value;
        return this;
    }

    public InsertQueryBuilder OutputInserted(string column)
    {
        _outputColumn = column;
        return this;
    }

    public BuiltQuery Build()
    {
        var sb = new StringBuilder();
        sb.Append($"INSERT INTO {_table} ({string.Join(", ", _columns)})\n");

        if (_outputColumn is not null)
        {
            sb.Append($"OUTPUT Inserted.{_outputColumn}\n");
        }

        var paramNames = _parameters.Keys;
        sb.Append($"VALUES ({string.Join(", ", paramNames)});");

        return new BuiltQuery(sb.ToString(), _parameters);
    }
}
