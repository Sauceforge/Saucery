﻿using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Execution;

namespace Saucery.Core.Tests;

/// <summary>
/// Utility Class used to build and run NUnit tests used as test data
/// </summary>
public class TestBuilder {
    #region Build Tests

    public static TestSuite MakeSuite(string name) => new(name);

    public static TestSuite MakeFixture(Type type) => new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(type));

    private static TestSuite MakeFixture(object fixture) {
        var suite = MakeFixture(fixture.GetType());
        suite.Fixture = fixture;
        return suite;
    }

    private static TestSuite MakeParameterizedMethodSuite(Type type, string methodName) {
        var suite = MakeTestFromMethod(type, methodName) as TestSuite;
        Assert.That(suite != null, "Unable to create parameterized suite - most likely there is no data provided");
        return suite!;
    }

    private static TestMethod MakeTestCase(Type type, string methodName) {
        var test = MakeTestFromMethod(type, methodName) as TestMethod;
        Assert.That(test != null, "Unable to create TestMethod from {0}", methodName);
        return test!;
    }

    // Will return either a ParameterizedMethodSuite or an NUnitTestMethod
    // depending on whether the method takes arguments or not
    private static Test MakeTestFromMethod(Type type, string methodName) {
        var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null) {
            Assert.Fail("Method not found: " + methodName);
        }

        return new DefaultTestCaseBuilder().BuildFrom(new MethodWrapper(type, method!));
    }

    #endregion

    #region Create WorkItems

    public static WorkItem CreateWorkItem(Type type) => CreateWorkItem(MakeFixture(type));

    public static WorkItem CreateWorkItem(Type type, string methodName) => CreateWorkItem(MakeTestFromMethod(type, methodName));

    private static WorkItem CreateWorkItem(Test test) {
        var context = new TestExecutionContext
        {
            Dispatcher = new SuperSimpleDispatcher()
        };

        return CreateWorkItem(test, context);
    }

    private static WorkItem CreateWorkItem(Test test, object testObject) {
        var context = new TestExecutionContext
        {
            TestObject = testObject,
            Dispatcher = new SuperSimpleDispatcher()
        };

        return CreateWorkItem(test, context);
    }

    private static WorkItem CreateWorkItem(Test test, TestExecutionContext context) {
        var work = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty, true);
        work?.InitializeContext(context);

        return work!;
    }

    #endregion

    #region Run Tests

    public static ITestResult RunTestFixture(Type type) => RunTest(MakeFixture(type), null);

    public static ITestResult RunTestFixture(object fixture) => RunTest(MakeFixture(fixture), fixture);

    public static ITestResult RunParameterizedMethodSuite(Type type, string methodName)
    {
        var suite = MakeParameterizedMethodSuite(type, methodName);

        object? testObject = null;
        if (!type.IsStatic())
        {
            testObject = Reflect.Construct(type);
        }

        return RunTest(suite, testObject);
    }

    public static ITestResult RunTestCase(Type type, string methodName)
    {
        var testMethod = MakeTestCase(type, methodName);

        object? testObject = null;
        if (!type.IsStatic())
        {
            testObject = Reflect.Construct(type);
        }

        return RunTest(testMethod, testObject!);
    }

    public static ITestResult RunTestCase(object fixture, string methodName)
    {
        var testMethod = MakeTestCase(fixture.GetType(), methodName);

        return RunTest(testMethod, fixture);
    }

    public static ITestResult RunAsTestCase(Action action)
    {
        var method = action.GetMethodInfo();
        var testMethod = MakeTestCase(method.DeclaringType!, method.Name);
        return RunTest(testMethod);
    }

    private static ITestResult RunTest(Test test) => RunTest(test, null);

    private static ITestResult RunTest(Test test, object? testObject) => ExecuteWorkItem(CreateWorkItem(test, testObject!));

    private static ITestResult ExecuteWorkItem(WorkItem work)
    {
        work.Execute();

        // TODO: Replace with an event - but not while method is static
        while (work.State != WorkItemState.Complete)
        {
            Thread.Sleep(1);
        }

        return work.Result;
    }

    #endregion

    #region Nested TestDispatcher Class

    /// <summary>
    /// SuperSimpleDispatcher merely executes the work item.
    /// It is needed particularly when running suites, since
    /// the child items require a dispatcher in the context.
    /// </summary>
    class SuperSimpleDispatcher : IWorkItemDispatcher
    {
        public int LevelOfParallelism => 0;

        public void Start(WorkItem topLevelWorkItem) => topLevelWorkItem.Execute();

        public void Dispatch(WorkItem work) => work.Execute();

        public void CancelRun(bool force) => throw new NotImplementedException();
    }
    #endregion
}