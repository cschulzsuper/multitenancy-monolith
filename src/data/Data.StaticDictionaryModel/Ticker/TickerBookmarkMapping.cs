using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Ticker;

public sealed class TickerBookmarkMapping : IMapping<TickerBookmark>
{
    public static object SetSnowflake(TickerBookmark entity, object snowflake)
        => entity.Snowflake == default ? entity.Snowflake = (long)snowflake : entity.Snowflake;

    public static object GetSnowflake(TickerBookmark entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider services, IEnumerable<TickerBookmark> data, TickerBookmark entity)
    {
        var conflict = data.Any(x =>
            string.Equals(x.TickerUser, entity.TickerUser, StringComparison.InvariantCultureIgnoreCase) &&
            x.TickerMessage == entity.TickerMessage);

        if (conflict)
        {
            ModelException.ThrowObjectConflict<TickerUser>($"{entity.TickerUser}/{entity.TickerMessage}");
        }
    }
}