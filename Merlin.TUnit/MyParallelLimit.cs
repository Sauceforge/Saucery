using TUnit.Core.Interfaces;

namespace Merlin.TUnit;

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 4;
}
