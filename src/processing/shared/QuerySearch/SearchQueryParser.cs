using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChristianSchulz.MultitenancyMonolith.Shared.QuerySearch;

public static partial class SearchQueryParser
{
    [GeneratedRegex(@"\ (?=(?:[^\""]*\""[^\""]*\"")*[^\""]*$)")]
    private static partial Regex SearchQueryRegex();

    public static SearchTermDictionary Parse(string searchQuery)
        => new(ParseInternal(searchQuery));

    private static IEnumerable<KeyValuePair<string, object>> ParseInternal(string searchQuery)
    {
        var matches = SearchQueryRegex().Matches(searchQuery);

        var indices = matches
            .Select(x => x.Index)
            .ToArray();

        var searchTerms = SplitAt(searchQuery, indices);

        foreach (var searchTerm in searchTerms)
        {
            var parts = searchTerm
                .Trim(' ')
                .Split(':', 2);

            if (parts.Length == 1)
            {
                var value = parts[0].Trim('\"');
                yield return new KeyValuePair<string, object>(string.Empty, value);
            }

            if (parts.Length == 2)
            {
                var value = parts[1].Trim('\"');
                yield return new KeyValuePair<string, object>(parts[0], value);
            }
        }
    }

    public static string[] SplitAt(string source, params int[] index)
    {
        index = index.Distinct().OrderBy(x => x).ToArray();
        string[] output = new string[index.Length + 1];
        int pos = 0;

        for (int i = 0; i < index.Length; pos = index[i++])
            output[i] = source[pos..index[i]];

        output[index.Length] = source[pos..];
        return output;
    }
}