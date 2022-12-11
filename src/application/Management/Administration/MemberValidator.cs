using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class MemberValidator
{
    internal static void Ensure(Member member)
    {
        EnsureUniqueName(member.UniqueName);
    }

    internal static void EnsureUniqueName(string uniqueName)
    {
        if(string.IsNullOrWhiteSpace(uniqueName))
        {
            throw new ManagementException("Unique name cannot be empty");
        }
    }
       
}
