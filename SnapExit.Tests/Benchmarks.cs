using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using SnapExit.Services;
using SnapExit.Tests.Services.BenchmarksHelpers;

namespace SnapExit.Tests;

public class Benchmarks
{
    private SnapExitBenchmarkClass benchmarkClass;

    [IterationSetup]
    public void Setup()
    {
        benchmarkClass = new SnapExitBenchmarkClass(new ExecutionControlService());
    }

    [Benchmark(OperationsPerInvoke = 32)]
    public async Task NormalService_SnapExitAsync()
    {
        int rnd = await benchmarkClass.DoSnapExit();
        DeadCodeEliminationHelper.KeepAliveWithoutBoxing(rnd);
    }

    [Benchmark(OperationsPerInvoke = 32)]
    public async Task NormalService_SnapExitAfterRegisterAsync()
    {
        int rnd = await benchmarkClass.DoSnapExitAfterRegister();
        DeadCodeEliminationHelper.KeepAliveWithoutBoxing(rnd);
    }

    [Benchmark(OperationsPerInvoke = 32)]
    public async Task NormalService_ExceptionsAsync()
    {
        int rnd = await benchmarkClass.DoException();
        DeadCodeEliminationHelper.KeepAliveWithoutBoxing(rnd);
    }
}
