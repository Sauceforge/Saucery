using System.Reflection;

namespace Saucery.FrameworkOnly.Microbench;

/// <summary>
/// In-process xUnit runner.
///
/// Why this exists:
/// The xunit.v3.runner.utility NuGet package does NOT ship a single assembly
/// called "xunit.v3.runner.utility.dll". It ships platform-specific assemblies:
///   xunit.v3.runner.utility.netfx.dll
///   xunit.v3.runner.utility.net8.dll
///   etc.
///
/// So Type.GetType("..., xunit.v3.runner.utility") will always fail.
/// We must dynamically load the assemblies and then search for the runner type.
/// </summary>
internal sealed class XunitRunner {
    private readonly Type _assemblyRunnerType;

    public XunitRunner() {
        LoadRunnerUtilityAssemblies();

        _assemblyRunnerType =
            ResolveType("Xunit.Runners.AssemblyRunner") ??
            ResolveType("Xunit.SimpleRunner.AssemblyRunner") ??
            throw new InvalidOperationException(
                "Could not locate xUnit AssemblyRunner. " +
                "Ensure xunit.v3.runner.utility is copied to the output directory.");
    }

    public int RunAssembly(string assemblyPath) {
        using var completion = new ManualResetEventSlim(false);

        object? runner = null;
        Exception? executionException = null;
        int exitCode = 0;

        try {
            runner = InvokeStatic(_assemblyRunnerType, "WithoutAppDomain", assemblyPath)
                ?? throw new InvalidOperationException("AssemblyRunner.WithoutAppDomain returned null.");

            // error callback
            TrySetProperty(runner, "OnError", new Action<string>(msg => {
                executionException = new InvalidOperationException(msg);
                exitCode = 1;
                completion.Set();
            }));

            // Different versions use different completion callback names/signatures.
            // - Some are Action (no args)
            // - Some are Action<T> (one arg, usually ExecutionSummary or similar)
            TrySetProperty(runner, "OnExecutionComplete", new Action<object>(_ => completion.Set()));
            TrySetProperty(runner, "OnExecutionComplete", new Action(completion.Set));
            TrySetProperty(runner, "OnComplete", new Action(completion.Set));

            // optional start options
            var startOptionsType =
                _assemblyRunnerType.Assembly.GetType("Xunit.Runners.AssemblyRunnerStartOptions") ??
                _assemblyRunnerType.Assembly.GetType("Xunit.SimpleRunner.AssemblyRunnerStartOptions");

            object? startOptions = startOptionsType is null ? null : Activator.CreateInstance(startOptionsType);

            if(startOptions is not null) {
                TrySetProperty(startOptions, "DiagnosticMessages", false);
                TrySetProperty(startOptions, "InternalDiagnosticMessages", false);
                TrySetProperty(startOptions, "ParallelizeTestCollections", false);
                TrySetProperty(startOptions, "MaxParallelThreads", 1);
                TrySetProperty(startOptions, "StopOnFail", false);
            }

            // try all common Start overloads
            if(!TryInvokeInstance(runner, "Start", startOptions))
                if(!TryInvokeInstance(runner, "Start", null))
                    if(!TryInvokeInstance(runner, "Start", new object?[] { startOptions, null }))
                        throw new MissingMethodException("Could not find a compatible AssemblyRunner.Start overload.");

            if(!completion.Wait(TimeSpan.FromMinutes(10)))
                throw new TimeoutException("xUnit runner did not signal completion within 10 minutes.");

            if(executionException is not null)
                throw executionException;

            return exitCode;
        } finally {
            if(runner is IAsyncDisposable ad)
                ad.DisposeAsync().AsTask().GetAwaiter().GetResult();
            else if(runner is IDisposable d)
                d.Dispose();
        }
    }

    private static void LoadRunnerUtilityAssemblies() {
        var baseDir = AppContext.BaseDirectory;

        foreach(var path in Directory.EnumerateFiles(baseDir, "xunit.v3.runner.utility*.dll")) {
            try {
                var name = AssemblyName.GetAssemblyName(path);

                if(AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), name)))
                    continue;

                Assembly.LoadFrom(path);
            } catch {
                // ignored intentionally
            }
        }
    }

    private static Type? ResolveType(string fullName) {
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            var type = asm.GetType(fullName, false);
            if(type != null)
                return type;
        }
        return null;
    }

    private static object? InvokeStatic(Type type, string name, params object?[] args)
        => type.GetMethod(name, BindingFlags.Public | BindingFlags.Static)?.Invoke(null, args);

    private static bool TryInvokeInstance(object target, string name, object? arg) {
        var methods = target.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == name);

        foreach(var m in methods) {
            var ps = m.GetParameters();

            if(arg == null && ps.Length == 0) {
                m.Invoke(target, []);
                return true;
            }

            if(arg != null && ps.Length == 1 && ps[0].ParameterType.IsInstanceOfType(arg)) {
                m.Invoke(target, [arg]);
                return true;
            }

            if(arg is object?[] arr && ps.Length == arr.Length) {
                m.Invoke(target, arr);
                return true;
            }
        }

        return false;
    }

    private static void TrySetProperty(object target, string name, object value) {
        var prop = target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if(prop?.CanWrite == true)
            prop.SetValue(target, value);
    }
}