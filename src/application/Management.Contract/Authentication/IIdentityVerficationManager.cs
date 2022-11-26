namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityVerficationManager
{
    void Set(string identity, byte[] verfication);

    byte[] Get(string identity);
}