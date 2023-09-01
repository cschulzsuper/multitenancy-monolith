using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Shared.QuerySearch;

public static class SearchTermDictionaryExtensions
{
    public static T[] GetValidSearchTermValues<T>(this SearchTermDictionary dictionary, string key)
    {
        if (!dictionary.TryGetValue(key, out var dictionaryValues))
        {
            return Array.Empty<T>();
        }

        if (dictionaryValues is T[] searchTermValues)
        {
            return searchTermValues;
        }

        var searchTermValueSet = new HashSet<object>();

        foreach (var dictionaryValue in dictionaryValues)
        {
            if (dictionaryValue is T searchTermValue)
            {
                searchTermValueSet.Add(searchTermValue);
                continue;
            }

            if (typeof(T) == typeof(int))
            {
                if (int.TryParse($"{dictionaryValue}", out var parsedValue))
                {
                    searchTermValueSet.Add(parsedValue);
                }
            }

            if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse($"{dictionaryValue}", out var parsedValue))
                {
                    searchTermValueSet.Add(parsedValue);
                }
            }

        }

        return searchTermValueSet
            .Cast<T>()
            .ToArray();
    }
}