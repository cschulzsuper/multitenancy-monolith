using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Ticker;

public class TickerMessageModel : IModel<TickerMessage>
{
    public static object SetSnowflake(TickerMessage entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(TickerMessage entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider services, IEnumerable<TickerMessage> data, TickerMessage entity)
    {

    }
}