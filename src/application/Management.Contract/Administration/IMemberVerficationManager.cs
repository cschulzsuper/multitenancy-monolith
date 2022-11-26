namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberVerficationManager
{
    void Set(string member, byte[] verfication);

    byte[] Get(string member);

}