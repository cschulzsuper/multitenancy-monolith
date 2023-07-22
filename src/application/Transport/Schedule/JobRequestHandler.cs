using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Responses;
using ChristianSchulz.MultitenancyMonolith.Jobs;
using System;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

public class JobRequestHandler : IJobRequestHandler
{
    private readonly IJobQueue _jobQueue;

    public JobRequestHandler(IJobQueue jobQueue)
    {
        _jobQueue = jobQueue;
    }

    public JobResponse Get(string job)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<JobResponse> GetAll()
    {
        throw new NotImplementedException();
    }

    public void Update(string job, JobRequest request)
    {
        _jobQueue.Requeue(job, new CronExpressionSchedule(request.Expression));
    }
}
