using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Reflection;

namespace ChristianSchulz.MultitenancyMonolith.Backend.Server.OpenApi
{
    public class OpenApiSchemaProvider
    {
        private readonly object _componentService;
        private readonly MethodInfo _componentServiceGetOrCreateSchema;

        public OpenApiSchemaProvider([ServiceKey] string documentName, IServiceProvider services) 
        {
            var componentServiceType = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetType("Microsoft.AspNetCore.OpenApi.OpenApiComponentService", false))
                .SingleOrDefault(type => type != null)!;

            _componentService = services.GetRequiredKeyedService(componentServiceType, documentName);
            _componentServiceGetOrCreateSchema = componentServiceType.GetMethod("GetOrCreateSchema", BindingFlags.NonPublic | BindingFlags.Instance)!;
        }

        public OpenApiSchema GetOrCreateSchema(Type type)
        {
            var schema = _componentServiceGetOrCreateSchema.Invoke(_componentService, [type, (ApiParameterDescription?)null]) as OpenApiSchema;

            return schema!;
        }
    }
}
