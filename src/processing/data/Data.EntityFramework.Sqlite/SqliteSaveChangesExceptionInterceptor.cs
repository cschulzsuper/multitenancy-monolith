using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite
{
    internal sealed class SqliteSaveChangesExceptionInterceptor : ISaveChangesInterceptor
    {
        public InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, 
            InterceptionResult<int> result)
        {
            var entries = eventData.Context?.ChangeTracker.Entries();
            if (entries == null) return result;

            CustomPropertiesTracking(eventData, entries);

            return result;
        }



        public ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {

            var entries = eventData.Context?.ChangeTracker.Entries();
            if (entries == null) return new(result);

            CustomPropertiesTracking(eventData, entries);

            return new(result);
        }

        public void SaveChangesFailed(
            DbContextErrorEventData eventData)
        {
            ThrowObjectConflictIfNecessary(eventData.Exception);
        }

        public Task SaveChangesFailedAsync(
            DbContextErrorEventData eventData,
            CancellationToken cancellationToken = default)
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

            if (isUniqueConstraintException) DataException.ThrowObjectConflict(exception.InnerException!);
        }

        private static void CustomPropertiesTracking(DbContextEventData eventData, IEnumerable<EntityEntry> entries)
        {
            foreach (var entry in entries)
            {
                var customPropertiesProperty = entry.Metadata.ClrType.GetProperty("CustomProperties");
                if (customPropertiesProperty == null) continue;

                foreach (var customProperty in (IEnumerable)customPropertiesProperty.GetValue(entry.Entity)!)
                {
                    // TODO Need to add properties in owned json https://github.com/dotnet/efcore/issues/31237 https://github.com/dotnet/efcore/issues/28594
                    eventData.Context!.Add(customProperty);
                }
            }
        }
    }
}
