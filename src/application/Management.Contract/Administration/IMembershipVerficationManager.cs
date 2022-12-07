namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMembershipVerficationManager
{
    bool Has(string group, string member, byte[] verfication);

    void Set(string group, string member, byte[] verfication);
}