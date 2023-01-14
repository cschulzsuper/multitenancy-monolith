namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityVerficationManager
{
    bool Has(IdentityVerficationKey verficationKey, byte[] verfication);

    void Set(IdentityVerficationKey verficationKey, byte[] verfication);
}