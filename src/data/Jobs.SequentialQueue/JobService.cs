using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal sealed class JobService : BackgroundService
{
    private readonly IJobQueue _queue;
    private readonly IServiceProvider _services;
    private readonly JobsOptions _options;

    public JobService(
        IJobQueue queue, 
        IServiceProvider services,
        JobsOptions options)
    {
        _queue = queue;
        _services = services;
        _options = options;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = _queue.Dequeue();

            var wait = job.Timestamp - DateTime.UtcNow;

            if (wait > TimeSpan.Zero)
            {
                await Task.Delay(wait, stoppingToken);
            }

            await InvokeAsync(job);
        }
    }

    private async Task InvokeAsync(JobCallback job)
    {
        await using var scope = _services.CreateAsyncScope();

        await _options.BeforeJobInvocation(scope.ServiceProvider, job.UniqueName);

        await job.Job.Invoke(scope.ServiceProvider);

        await _options.AfterJobInvocation(scope.ServiceProvider, job.UniqueName);
    }
}