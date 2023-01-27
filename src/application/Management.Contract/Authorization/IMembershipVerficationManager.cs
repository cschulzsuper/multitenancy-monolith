namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMembershipVerficationManager
{
    bool Has(MembershipVerficationKey verficationKey, byte[] verfication);

    void Set(MembershipVerficationKey verficationKey, byte[] verfication);
}