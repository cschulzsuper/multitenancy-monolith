using IdGen;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

public sealed class SnowflakeGenerator
{
    private readonly IdGenerator _idGenerator;

    public SnowflakeGenerator()
    {
        _idGenerator = new IdGenerator(0);
    }

    public long Next()
    {
        return _idGenerator.CreateId();
    }
}