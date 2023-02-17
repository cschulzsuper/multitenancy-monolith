namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public interface IMemberVerificationManager
{
    bool Has(MemberVerificationKey verificationKey, byte[] verification);

    void Set(MemberVerificationKey verificationKey, byte[] verification);
}