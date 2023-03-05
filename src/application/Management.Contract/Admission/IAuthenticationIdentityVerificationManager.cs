namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationIdentityVerificationManager
{
    bool Has(IdentityVerificationKey verificationKey, byte[] verification);

    void Set(IdentityVerificationKey verificationKey, byte[] verification);
}