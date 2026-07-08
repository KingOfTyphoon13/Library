using Library.Domain.Common.Pagination;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Library.DAL.QueryBuilder;

public sealed class QueryBuilder
{
    private readonly List<string> _selectColumns = [];
    private string _from = "";
    private readonly List<string> _joins = [];
    private readonly List<string> _whereConditions = [];
    private readonly List<string> _groupByColumns = [];
    private string _orderBy = "";
    private string _paginationClause = "";
    private readonly Dictionary<string, object> _parameters = [];

    public static QueryBuilder Create() => new();

    public QueryBuilder Select(params string[] columns)
    {
        _selectColumns.AddRange(columns);
        return this;
    }

    public QueryBuilder From(string table)
    {
        _from = table;
        return this;
    }
    public QueryBuilder From(BuiltQuery subquery, string alias)
    {
        _from = $"({subquery.Sql}) AS {alias}";
        foreach (var (key, value) in subquery.Parameters)
        {
            _parameters[key] = value;
        }
        return this;
    }

    public QueryBuilder LeftJoin(string table, string onCondition)
    {
        _joins.Add($"LEFT JOIN {table} ON {onCondition}");
        return this;
    }

    public QueryBuilder InnerJoin(string table, string onCondition)
    {
        _joins.Add($"INNER JOIN {table} ON {onCondition}");
        return this;
    }

    public QueryBuilder Where(string condition, string? paramName = null, object? paramValue = null)
    {
        _whereConditions.Add(condition);
        if (paramName is not null)
        {
            _parameters[paramName] = paramValue ?? DBNull.Value;
        }
        return this;
    }

    public QueryBuilder GroupBy(params string[] columns)
    {
        _groupByColumns.AddRange(columns);
        return this;
    }

    public QueryBuilder OrderBy(string column)
    {
        _orderBy = column;
        return this;
    }

    public QueryBuilder Paginate(PagedRequest request)
    {
        _paginationClause = "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        _parameters["@Offset"] = (request.PageNumber - 1) * request.PageSize;
        _parameters["@PageSize"] = request.PageSize;
        return this;
    }

    public QueryBuilder PaginateKeyset(KeysetRequest request, string keysetColumn)
    {
        _whereConditions.Add($"(@LastItemId IS NULL OR {keysetColumn} > @LastItemId)");
        _parameters["@LastItemId"] = (object?)request.LastItemIndex ?? DBNull.Value;
        _parameters["@FetchSize"] = request.PageSize;
        _selectColumns.Insert(0, "TOP (@FetchSize)");
        return this;
    }

    public BuiltQuery Build()
    {
        var sb = new StringBuilder();

        var topClause = "";
        var columns = _selectColumns;
        if (columns.Count > 0 && columns[0] == "TOP (@FetchSize)")
        {
            topClause = "TOP (@FetchSize) ";
            columns = columns.Skip(1).ToList();
        }

        sb.Append($"SELECT {topClause}{string.Join(", ", columns)}\n");
        sb.Append($"FROM {_from}\n");

        foreach (var join in _joins)
        {
            sb.Append($"{join}\n");
        }

        if (_whereConditions.Count > 0)
        {
            sb.Append($"WHERE {string.Join(" AND ", _whereConditions)}\n");
        }

        if (_groupByColumns.Count > 0)
        {
            sb.Append($"GROUP BY {string.Join(", ", _groupByColumns)}\n");
        }

        if (!string.IsNullOrEmpty(_orderBy))
        {
            sb.Append($"ORDER BY {_orderBy}\n");
        }

        if (!string.IsNullOrEmpty(_paginationClause))
        {
            sb.Append(_paginationClause);
        }

        return new BuiltQuery(sb.ToString().TrimEnd(), _parameters);
    }
}

public sealed record BuiltQuery(string Sql, IReadOnlyDictionary<string, object> Parameters)
{
    public SqlCommand ToCommand(SqlConnection connection, SqlTransaction? transaction)
    {
        var command = new SqlCommand(Sql, connection, transaction);
        foreach (var (name, value) in Parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }
        return command;
    }
}