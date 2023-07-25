using ChristianSchulz.MultitenancyMonolith.Jobs;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule
{
    public class PlannedJobRescheduler : IPlannedJobRescheduler
    {
        private readonly IPlannedJobManager _plannedJobManager;
        private readonly IPlannedJobScheduler _plannedJobScheduler;

        public PlannedJobRescheduler(
            IPlannedJobManager plannedJobManager,
            IPlannedJobScheduler plannedJobScheduler)
        {
            _plannedJobManager = plannedJobManager;
            _plannedJobScheduler = plannedJobScheduler;
        }
        public async Task RescheduleAsync(long plannedJob)
        {
            var @object = await _plannedJobManager.GetAsync(plannedJob);

            _plannedJobScheduler.Reschedule(@object.UniqueName);
        }
    }
}
