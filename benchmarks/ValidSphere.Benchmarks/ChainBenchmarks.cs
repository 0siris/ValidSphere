using BenchmarkDotNet.Attributes;
using ValidSphere;

namespace ValidSphere.Benchmarks;

[MemoryDiagnoser]
public class ChainBenchmarks {
    private int value = 42;

    [Benchmark(Baseline = true)]
    public int Greater() => value.Is().Greater(0).Value;

    [Benchmark]
    public int Greater_OnFailure()
        => value.Is()
            .OnFailure(static f => new InvalidOperationException(f.Message))
            .Greater(0).Value;
}
