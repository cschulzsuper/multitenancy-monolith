namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountMemberVerificationManager
{
    bool Has(AccountMemberVerificationKey verificationKey, byte[] verification);

    void Set(AccountMemberVerificationKey verificationKey, byte[] verification);
}