using Microsoft.AspNetCore.Builder;

namespace ChristianSchulz.MultitenancyMonolith.Application
{
    internal static class _Extensions
    {
        public static void WithErrorMessage<TBuilder>(this TBuilder builder, string errorMessage)
            where TBuilder : IEndpointConventionBuilder
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new ErrorMessageAttribute(errorMessage));
            });
        }
    }
}
