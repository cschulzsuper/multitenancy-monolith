namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationIdentityVerificationManager
{
    bool Has(AuthenticationIdentityVerificationKey verificationKey, byte[] verification);

    void Set(AuthenticationIdentityVerificationKey verificationKey, byte[] verification);
}