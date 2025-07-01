using TUnit.Core.Interfaces;

namespace MyTestProject;

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 4;
}
