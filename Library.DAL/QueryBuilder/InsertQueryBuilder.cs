using System.Text;

namespace Library.DAL.QueryBuilder;

public sealed class InsertQueryBuilder
{
    private string _table = "";
    private readonly List<string> _columns = [];
    private string? _outputColumn;
    private readonly Dictionary<string, object> _parameters = [];
    private readonly List<List<string>> _rows = [];

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

    public InsertQueryBuilder AddRow(params (string Column, object? Value)[] values)
    {
        if (_columns.Count == 0)
        {
            _columns.AddRange(values.Select(v => v.Column));
        }

        var rowParamNames = new List<string>();
        var rowIndex = _rows.Count;

        foreach (var (column, value) in values)
        {
            var paramName = $"@{column}_{rowIndex}";
            _parameters[paramName] = value ?? DBNull.Value;
            rowParamNames.Add(paramName);
        }

        _rows.Add(rowParamNames);
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

        if (_rows.Count > 0)
        {
            var valueRows = _rows.Select(row => $"({string.Join(", ", row)})");
            sb.Append($"VALUES {string.Join(", ", valueRows)};");
        }
        else
        {
            sb.Append($"VALUES ({string.Join(", ", _parameters.Keys)});");
        }

        return new BuiltQuery(sb.ToString(), _parameters);
    }
}
