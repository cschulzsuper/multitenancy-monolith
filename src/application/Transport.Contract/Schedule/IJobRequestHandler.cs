using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Responses;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

public interface IJobRequestHandler
{
    JobResponse Get(string job);

    IEnumerable<JobResponse> GetAll();

    void Update(string job, JobRequest request);
}