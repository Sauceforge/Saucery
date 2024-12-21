using System.Linq;

namespace Sandbox.TUnit;

public class MethodDataSourceTests
{
    [Test]
    [MethodDataSource(typeof(MyTestDataSources), nameof(MyTestDataSources.AdditionTestData))]
    public async Task MyTest(AdditionTestData additionTestData)
    {
        var result = Add(additionTestData.value1, additionTestData.value2);

        await Assert.That(result).IsEqualTo(additionTestData.expectedResult);
    }

    private int Add(int x, int y) 
        => x + y;

    [Test]
    [MethodDataSource(nameof(AllCombinations))]
    public async Task DataDrivenTest(int first, int second)
    {
        Console.WriteLine($"First Second:{first} {second}");

        await Assert.That(first).IsNotNull();
    }

    public static IEnumerable<(int, int)> AllCombinations() => from firstInt in new[] { 1, 2 }
                                                               from secondInt in new[] { 4, 5 }
                                                               select (firstInt, secondInt);
}

public record AdditionTestData(int value1, int value2, int expectedResult);

public static class MyTestDataSources
{
    public static List<Func<AdditionTestData>> AdditionTestData() => [
            () => new AdditionTestData(1, 2, 3),
            () => new AdditionTestData(2, 3, 5)
        ];
}
