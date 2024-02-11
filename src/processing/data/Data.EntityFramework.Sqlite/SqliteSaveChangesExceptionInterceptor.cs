using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite
{
    internal sealed class SqliteSaveChangesExceptionInterceptor : ISaveChangesInterceptor
    {
        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            ThrowObjectConflictIfNecessary(eventData.Exception);
        }

        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            ThrowObjectConflictIfNecessary(eventData.Exception);
            return Task.CompletedTask;
        }

        private static void ThrowObjectConflictIfNecessary(Exception exception)
        {
            var isUniqueConstraintException = exception is DbUpdateException
            {
                InnerException: SqliteException
                {
                    SqliteErrorCode: 19,
                    SqliteExtendedErrorCode: 2067
                }
            };

            if (isUniqueConstraintException) RepositoryException.ThrowObjectConflict(exception.InnerException!);
        }
    }
}
