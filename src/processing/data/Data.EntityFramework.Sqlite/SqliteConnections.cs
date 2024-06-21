using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite;

public sealed class SqliteConnections : IDisposable
{
    private readonly ConcurrentDictionary<string,SqliteConnection> _connections;

    private readonly string _connectionDiscriminator;

    public SqliteConnections()
    {
        _connections = new ConcurrentDictionary<string,SqliteConnection>();
        _connectionDiscriminator = $"{Guid.NewGuid()}";
    }

    public SqliteConnection Get(string name)
        => _connections.GetOrAdd(name, Create);

    private SqliteConnection Create(string name)
    {
        var connectionName = $"{_connectionDiscriminator}-{name}";
        var connectionString = $"DataSource={connectionName};mode=memory;cache=shared";

        var connection = new SqliteConnection(connectionString);
        
        connection.Open();

        return connection;
    }

    public void Dispose()
    {
        foreach (var connection in _connections.Values)
        {
            connection.Dispose();
        }

        _connections.Clear();
    }
}
