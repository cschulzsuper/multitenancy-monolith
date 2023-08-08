namespace ChristianSchulz.MultitenancyMonolith.Web;

public interface IWebServiceClientFactory
{
    IWebServiceClient Create(string uniqueName);
}