namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IUserManager
{
    User Get(string username);
}