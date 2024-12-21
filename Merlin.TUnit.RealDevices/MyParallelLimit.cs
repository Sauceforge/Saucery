using TUnit.Core.Interfaces;

namespace Merlin.TUnit.RealDevices;

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 3;
}
