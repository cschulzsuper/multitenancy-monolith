using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Shared.QuerySearch;

public class SearchTermDictionary : IDictionary<string, object[]>
{
    public IDictionary<string, object[]> _searchTerms;

    public SearchTermDictionary(IEnumerable<KeyValuePair<string, object>> searchTerms)
    {
        _searchTerms = searchTerms
            .Distinct()
            .GroupBy(x => x.Key, x => x.Value)
            .ToDictionary(
                x => x.Key,
                x => x.ToArray());
    }

    public object[] this[string key]
    {
        get => _searchTerms[key];
        set => _searchTerms[key] = value;
    }

    public ICollection<string> Keys
        => _searchTerms.Keys;

    public ICollection<object[]> Values
        => _searchTerms.Values;

    public int Count
        => _searchTerms.Count;

    public bool IsReadOnly
        => _searchTerms.IsReadOnly;

    public void Add(string key, object[] value)
        => _searchTerms.Add(key, value);

    public void Add(KeyValuePair<string, object[]> item)
        => _searchTerms.Add(item);

    public void Clear()
        => _searchTerms.Clear();

    public bool Contains(KeyValuePair<string, object[]> item)
        => _searchTerms.Contains(item);

    public bool ContainsKey(string key)
        => _searchTerms.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, object[]>[] array, int arrayIndex)
        => _searchTerms.CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, object[]>> GetEnumerator()
        => _searchTerms.GetEnumerator();

    public bool Remove(string key)
        => _searchTerms.Remove(key);

    public bool Remove(KeyValuePair<string, object[]> item)
        => _searchTerms.Remove(item);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object[] value)
        => _searchTerms.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => _searchTerms.GetEnumerator();
}