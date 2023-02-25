using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class TaskCollection
{
    private readonly ILogger<TaskCollection> _logger;
    private readonly List<Task> _tasks;

    public TaskCollection(ILogger<TaskCollection> logger)
    {
        _logger = logger;
        _tasks = new List<Task>();
    }

    public void Add(Task task)
        => _tasks.Add(task);

    public async Task WaitAsync(CancellationToken cancellationToken)
    {
        foreach (var task in _tasks)
        {
            try
            {
                if (!task.IsCompleted)
                {
                    await task.WaitAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                continue;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while cancelling event service task");
                continue;
            }
        }
    }
}