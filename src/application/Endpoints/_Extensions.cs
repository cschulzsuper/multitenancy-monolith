using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
internal static class _Extensions
{
    public static TBuilder WithErrorMessage<TBuilder>(this TBuilder builder, string errorMessage)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(endpointBuilder =>
        {
            endpointBuilder.Metadata.Add(new ErrorMessageAttribute(errorMessage));
        });

        return builder;
    }

    public static TBuilder Authenticates<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(endpointBuilder =>
        {
            endpointBuilder.Metadata.Add(new AuthenticationAttribute());
        });

        return builder;
    }
}
