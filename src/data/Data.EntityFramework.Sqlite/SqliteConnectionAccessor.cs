using Microsoft.Data.Sqlite;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite;

public sealed class SqliteConnectionAccessor : IDisposable
{
    private const string InMemeoryConnectionString = "DataSource=multitenancy-monolith;mode=memory;cache=shared";

    private readonly SqliteConnection _connection;

    public SqliteConnectionAccessor()
    {
        _connection = new SqliteConnection(InMemeoryConnectionString);
        _connection.Open();
    }
    public SqliteConnection Connection => _connection;

    public void Dispose()
    {
        _connection.Dispose();
    }


}
