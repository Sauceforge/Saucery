using TUnit.Core.Interfaces;

namespace ExternalMerlin.TUnit;

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 3;
}
