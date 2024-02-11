using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

public interface IPlannedJobRequestHandler
{
    Task<PlannedJobResponse> GetAsync(string job);

    IAsyncEnumerable<PlannedJobResponse> GetAll();

    Task UpdateAsync(string job, PlannedJobRequest request);
}