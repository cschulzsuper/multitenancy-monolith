using System;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel;

public interface IModel<TEntity>
{
    static abstract object SetSnowflake(TEntity entity, object snowflake);

    static abstract object GetSnowflake(TEntity entity);

    static abstract bool Multitenancy { get; }

    static abstract void Ensure(IServiceProvider _, IEnumerable<TEntity> data, TEntity entity);
}