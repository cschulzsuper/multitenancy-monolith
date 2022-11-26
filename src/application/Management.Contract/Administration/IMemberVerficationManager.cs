namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberVerficationManager
{
    bool Has(string group, string member, byte[] verfication);

    void Set(string group, string member, byte[] verfication);
}