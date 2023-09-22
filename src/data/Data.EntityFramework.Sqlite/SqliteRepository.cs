using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite;

public sealed class SqliteRepository<TEntity> : Repository<TEntity>
    where TEntity : class, ICloneable
{
    private readonly DbContext _context;

    public SqliteRepository(DbContext context) : base(context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    public override IQueryable<TEntity> GetQueryable(FormattableString query)
        => _context.Set<TEntity>().FromSqlInterpolated(query);
}