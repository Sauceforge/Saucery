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
}

public record AdditionTestData(int value1, int value2, int expectedResult);

public static class MyTestDataSources
{
    public static List<Func<AdditionTestData>> AdditionTestData()
    {
        return new List<Func<AdditionTestData>>
        {
            () => new AdditionTestData(1, 2, 3),
            () => new AdditionTestData(2, 3, 5)
        };
    }
}
