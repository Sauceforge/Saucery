using TUnit.Core.Interfaces;

namespace Set1;

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 4;
}
