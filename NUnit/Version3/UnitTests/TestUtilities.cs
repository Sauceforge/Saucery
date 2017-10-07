using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Execution;

namespace UnitTests {
    /// <summary>
    /// Utility Class used to build and run NUnit tests used as test data
    /// </summary>
    public class TestBuilder {
        #region Build Tests

        public static TestSuite MakeFixture(Type type) {
            return new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(type));
        }

        public static TestSuite MakeFixture(object fixture) {
            var suite = MakeFixture(fixture.GetType());
            suite.Fixture = fixture;
            return suite;
        }

        public static TestSuite MakeParameterizedMethodSuite(Type type, string methodName) {
            var suite = MakeTestFromMethod(type, methodName) as TestSuite;
            Assert.NotNull(suite, "Unable to create parameterized suite - most likely there is no data provided");
            return suite;
        }

        public static TestSuite MakeParameterizedMethodSuite(object fixture, string methodName) {
            var test = MakeTestFromMethod(fixture.GetType(), methodName) as ParameterizedMethodSuite;
            Assert.That(test, Is.TypeOf<ParameterizedMethodSuite>());

            TestSuite suite = test;
            suite.Fixture = fixture;
            return suite;
        }

        public static TestMethod MakeTestCase(Type type, string methodName) {
            var test = MakeTestFromMethod(type, methodName);
            Assert.That(test, Is.TypeOf<TestMethod>());

            return (TestMethod) test;
        }

        // Will return either a ParameterizedMethodSuite or an NUnitTestMethod
        // depending on whether the method takes arguments or not
        internal static Test MakeTestFromMethod(Type type, string methodName) {
            var method = type.GetMethod(methodName,
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
                Assert.Fail("Method not found: " + methodName);
            return new DefaultTestCaseBuilder().BuildFrom(new MethodWrapper(type, method));
        }

        #endregion

        #region Run Tests

        public static ITestResult RunTestFixture(Type type) {
            return RunTestSuite(MakeFixture(type), null);
        }

        public static ITestResult RunTestFixture(object fixture) {
            return RunTestSuite(MakeFixture(fixture), fixture);
        }

        public static ITestResult RunParameterizedMethodSuite(Type type, string methodName) {
            var suite = MakeParameterizedMethodSuite(type, methodName);

            object testObject = null;
            if (!IsStaticClass(type))
                testObject = Reflect.Construct(type);

            return RunTestSuite(suite, testObject);
        }

        public static ITestResult RunTestSuite(TestSuite suite, object testObject) {
            var context = new TestExecutionContext {TestObject = testObject};

            var work = WorkItemBuilder.CreateWorkItem(suite, TestFilter.Empty);
            work.InitializeContext(context);
            work.Execute();

            // TODO: Replace with an event - but not while method is static
            while (work.State != WorkItemState.Complete) {
#if PORTABLE
                System.Threading.Tasks.Task.Delay(1);
#else
                Thread.Sleep(1);
#endif
            }

            return work.Result;
        }

        public static ITestResult RunTestCase(Type type, string methodName) {
            var testMethod = MakeTestCase(type, methodName);

            object testObject = null;
            if (!IsStaticClass(type))
                testObject = Reflect.Construct(type);

            return RunTest(testMethod, testObject);
        }

        public static ITestResult RunTestCase(object fixture, string methodName) {
            var testMethod = MakeTestCase(fixture.GetType(), methodName);

            return RunTest(testMethod, fixture);
        }

        // This method can't currently be used. It would be more efficient
        // to run test cases using the command directly, but that would
        // cause errors in tests that have a timeout or that require a
        // separate thread or a specific apartment. Those features are
        // handled at the level of the WorkItem in the current build.
        // Therefore, we run all tests, both test cases and fixtures,
        // by creating a WorkItem and executing it. See the RunTest
        // method below.

        //public static ITestResult RunTestMethod(TestMethod testMethod, object fixture)
        //{
        //    TestExecutionContext context = new TestExecutionContext();
        //    context.CurrentTest = testMethod;
        //    context.CurrentResult = testMethod.MakeTestResult();
        //    context.TestObject = fixture;

        //    TestCommand command = testMethod.MakeTestCommand();

        //    return command.Execute(context);
        //}

        //public static ITestResult RunTest(Test test)
        //{
        //    return RunTest(test, null);
        //}

        public static ITestResult RunTest(Test test, object testObject) {
            var context = new TestExecutionContext {TestObject = testObject};

            var work = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty);
            work.InitializeContext(context);
            work.Execute();

            // TODO: Replace with an event - but not while method is static
            while (work.State != WorkItemState.Complete) {
#if PORTABLE
                System.Threading.Tasks.Task.Delay(1);
#else
                Thread.Sleep(1);
#endif
            }

            return work.Result;
        }

        #endregion

        private static bool IsStaticClass(Type type) {
            return type.GetTypeInfo().IsAbstract && type.GetTypeInfo().IsSealed;
        }

        private TestBuilder() {
        }
    }
}