using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Saucery.Core.Tests.Util;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class GenericTestCaseAttribute(Type type, params object[] arguments) : TestCaseAttribute(arguments), ITestBuilder
{
    IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test? suite) =>
        BuildFrom(method.IsGenericMethodDefinition 
                ? method.MakeGenericMethod(type) 
                : method, 
            suite);
}