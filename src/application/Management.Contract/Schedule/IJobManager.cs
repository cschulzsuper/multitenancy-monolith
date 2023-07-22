using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

public interface IJobManager
{
    Task<Job> GetAsync(string job);

    IAsyncEnumerable<Job> GetAsyncEnumerable(Func<IQueryable<Job>, IQueryable<Job>> query);

    Task InsertAsync(Job @object);

    Task UpdateAsync(string job, Action<Job> action);
}