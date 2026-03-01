using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace Saucery.FrameworkOnly.Microbench;

public static class Program {
    public static int Main(string[] args) {
        var config = ManualConfig.Create(DefaultConfig.Instance);
        TrySetBuildTimeout(config, TimeSpan.FromMinutes(10));

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        return 0;
    }

    /// <summary>
    /// BenchmarkDotNet does not publicly expose BuildTimeout in all versions,
    /// so we set it via reflection if the property exists.
    /// </summary>
    private static void TrySetBuildTimeout(IConfig config, TimeSpan timeout) {
        var type = config.GetType();

        var property =
            type.GetProperty("BuildTimeout", BindingFlags.Public | BindingFlags.Instance) ??
            type.GetProperty("BuildTimeout", BindingFlags.NonPublic | BindingFlags.Instance);

        if(property == null)
            return;

        if(property.PropertyType == typeof(TimeSpan) && property.CanWrite)
            property.SetValue(config, timeout);
    }
}