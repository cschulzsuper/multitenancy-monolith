using IdGen;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework;

public sealed class SnowflakeGenerator : ValueGenerator<long>
{
    private readonly IdGenerator _idGenerator;

    public SnowflakeGenerator()
    {
        _idGenerator = new IdGenerator(0);
    }

    public override bool GeneratesTemporaryValues => false;

    public override long Next(EntityEntry entry)
    {
        return _idGenerator.CreateId();
    }
}