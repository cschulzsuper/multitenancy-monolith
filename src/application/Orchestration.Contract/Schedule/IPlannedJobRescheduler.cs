using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule
{
    public interface IPlannedJobRescheduler
    {
        Task RescheduleAsync(long plannedJob);
    }
}
