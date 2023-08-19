using System;

namespace ChristianSchulz.MultitenancyMonolith.Web;

public sealed class WebServiceStatusCodeResult
{
    public static WebServiceStatusCodeResult Success(Uri baseUri, Uri requestUri)
    {
        var absoluteUri = new Uri(baseUri, requestUri);

        var result = new WebServiceStatusCodeResult
        {
            IsSuccessStatusCode = true,
            AbsoluteUri = absoluteUri.AbsoluteUri
        };

        return result;
    }

    public static WebServiceStatusCodeResult Failed(Uri baseUri, Uri requestUri)
    {
        var absoluteUri = new Uri(baseUri, requestUri);

        var result = new WebServiceStatusCodeResult
        {
            IsSuccessStatusCode = false,
            AbsoluteUri = absoluteUri.AbsoluteUri
        };

        return result;
    }

    public required bool IsSuccessStatusCode { get; init; }

    public required string AbsoluteUri { get; init; }

}