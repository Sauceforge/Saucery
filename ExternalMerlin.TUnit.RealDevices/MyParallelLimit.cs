using TUnit.Core.Interfaces;

namespace ExternalMerlin.TUnit.RealDevices;

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 3;
}
