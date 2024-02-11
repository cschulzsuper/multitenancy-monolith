using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Web;

public interface IWebServiceClientFactory
{
    IWebServiceClient Create(string uniqueName);

    IWebServiceClient Create(string service, string token);

    IWebServiceClient Create(string service, Func<Task<string?>> tokenProvider);
}