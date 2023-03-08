using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Ticker;

public class TickerUserModel : IModel<TickerUser>
{
    public static object SetSnowflake(TickerUser entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(TickerUser entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider services, IEnumerable<TickerUser> data, TickerUser entity)
    {
        var mailAddressConflict = data.Any(x => string.Equals(x.MailAddress, entity.MailAddress, StringComparison.InvariantCultureIgnoreCase));
        
        if (mailAddressConflict)
        {
            ModelException.ThrowPropertyValueConflict<TickerUser>(nameof(entity.MailAddress), entity.MailAddress);
        }
    }
}