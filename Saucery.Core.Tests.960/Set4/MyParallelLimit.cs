using TUnit.Core.Interfaces;

namespace Set4;

public record MyParallelLimit4 : IParallelLimit
{
    public int Limit => 4;
}
