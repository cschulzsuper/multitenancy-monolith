using System;

namespace ChristianSchulz.MultitenancyMonolith.Web;

public interface IWebServiceClient : IDisposable
{
    WebServiceStatusCodeResult TryGet(string path, int attempts = 3);
}