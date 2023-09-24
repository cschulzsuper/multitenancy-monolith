using Microsoft.Data.Sqlite;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite;

public sealed class SqliteConnectionAccessor : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteConnectionAccessor()
    {
        var connectionName = $"{Guid.NewGuid}";
        var connectionString = $"DataSource={connectionName};mode=memory;cache=shared";

        _connection = new SqliteConnection(connectionString);
        _connection.Open();
    }
    public SqliteConnection Connection => _connection;

    public void Dispose()
    {
        _connection.Dispose();
    }


}
