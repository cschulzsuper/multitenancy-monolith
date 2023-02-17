namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityVerificationManager
{
    bool Has(IdentityVerificationKey verificationKey, byte[] verification);

    void Set(IdentityVerificationKey verificationKey, byte[] verification);
}