using Microsoft.AspNetCore.Builder;

namespace ChristianSchulz.MultitenancyMonolith.Application
{
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
    }
}
