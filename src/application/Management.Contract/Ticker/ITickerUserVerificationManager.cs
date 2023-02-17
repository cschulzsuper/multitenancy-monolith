namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public interface ITickerUserVerificationManager
{
    bool Has(TickerUserVerificationKey verificationKey, byte[] verification);

    void Set(TickerUserVerificationKey verificationKey, byte[] verification);
}