using TUnit.Core.Interfaces;

namespace Set8;

public record MyParallelLimit8 : IParallelLimit
{
    public int Limit => 4;
}
