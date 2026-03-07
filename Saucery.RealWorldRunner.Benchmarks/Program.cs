using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Saucery.RealWorldRunner.Benchmarks;

public static class Program {
    public static int Main(string[] args) {
        // BenchmarkDotNet API note:
        // BenchmarkRunner.Run(...) overloads are generic/type-based.
        // To pass CLI args and a config, use BenchmarkSwitcher.
        var config = DefaultConfig.Instance;
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        return 0;
    }
}