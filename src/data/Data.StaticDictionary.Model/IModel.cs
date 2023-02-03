namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model
{
    public interface IModel<T>
    {
        static abstract object SetSnowflake(T entity, object snowflake);

        static abstract object GetSnowflake(T entity);

        static abstract bool Multitenancy { get; }

        static abstract void Ensure(IServiceProvider _, IEnumerable<T> data, T entity);
    }
}