﻿using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal sealed class ObjectTypeDefinitionProvider : IObjectTypeDefinitionProvider
{
    public static readonly IDictionary<string, ObjectTypeDefinition> _objectTypes;

    static ObjectTypeDefinitionProvider()
    {
        var businessObjectAnnotation = typeof(BusinessObject).GetCustomAttribute<ObjectAnnotationAttribute>()
            ?? throw new UnreachableException("The object 'BusinessObject' is not annotated with an 'ObjectAnnotationAttribute'.");

        _objectTypes = new Dictionary<string, ObjectTypeDefinition>
        {
            [businessObjectAnnotation.UniqueName] = new()
            {
                UniqueName = businessObjectAnnotation.UniqueName,
                DisplayName = businessObjectAnnotation.DisplayName,
                Area = businessObjectAnnotation.Area,
                Collection = businessObjectAnnotation.Collection
            }
        };
    }

    public ObjectTypeDefinition Get(string uniqueName)
    {
        ObjectTypeValidation.EnsureUniqueName(uniqueName);

        return _objectTypes[uniqueName];
    }

    public IEnumerable<ObjectTypeDefinition> GetEnumerable()
        => _objectTypes.Values;
}