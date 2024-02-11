using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Schedule.Responses;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

internal sealed class PlannedJobRequestHandler : IPlannedJobRequestHandler
{
    private readonly IPlannedJobManager _plannedJobManager;

    public PlannedJobRequestHandler(IPlannedJobManager plannedJobManager)
    {
        _plannedJobManager = plannedJobManager;
    }

    public async Task<PlannedJobResponse> GetAsync(string job)
    {
        var @object = await _plannedJobManager.GetAsync(job);

        var response = new PlannedJobResponse
        {
            UniqueName = @object.UniqueName,
            Expression = @object.Expression
        };

        return response;
    }

    public async IAsyncEnumerable<PlannedJobResponse> GetAll()
    {
        var objects = _plannedJobManager.GetAsyncEnumerable();

        await foreach (var @object in objects)
        {
            var response = new PlannedJobResponse
            {
                UniqueName = @object.UniqueName,
                Expression = @object.Expression,
            };

            yield return response;
        }
    }

    public async Task UpdateAsync(string job, PlannedJobRequest request)
    {
        await _plannedJobManager.UpdateAsync(job,
            @object =>
            {
                @object.Expression = request.Expression;
                @object.ExpressionType = ScheduleExpressionTypes.CronExpression;
            });
    }
}
