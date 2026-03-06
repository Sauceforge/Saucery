using TUnit.Core.Interfaces;

namespace Saucery.Core.Tests;

public record MyParallelLimit : IParallelLimit
{
    public int Limit => 4;
}
