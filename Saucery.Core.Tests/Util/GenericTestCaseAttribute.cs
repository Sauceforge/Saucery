﻿using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Saucery.Core.Tests.Util;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class GenericTestCaseAttribute(Type type, params object[] arguments) : TestCaseAttribute(arguments), ITestBuilder
{
    private readonly Type _type = type;

    IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test? suite)
    {
        if (method.IsGenericMethodDefinition && _type != null)
        {
            var gm = method.MakeGenericMethod(_type);
            return BuildFrom(gm, suite);
        }

        return BuildFrom(method, suite);
    }
}