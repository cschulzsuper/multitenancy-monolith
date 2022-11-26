namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityVerficationManager
{
    bool Has(string identity, byte[] verfication);

    void Set(string identity, byte[] verfication);
}