namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityManager
{
    Identity Get(string uniqueName);
}