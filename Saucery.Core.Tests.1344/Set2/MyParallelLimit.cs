using TUnit.Core.Interfaces;

namespace Set2;

public record MyParallelLimit2 : IParallelLimit
{
    public int Limit => 4;
}
