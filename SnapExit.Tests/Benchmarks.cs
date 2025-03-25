using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using SnapExit.Services;
using SnapExit.Tests.Services.BenchmarksHelpers;
using System.Diagnostics;

namespace SnapExit.Tests;

public class Benchmarks
{
    private SnapExitBenchmarkClass benchmarkClass;

    public void Setup()
    {
        benchmarkClass = new SnapExitBenchmarkClass(new ExecutionControlService());
    }

    public async Task NormalService_SnapExitAsync()
    {
        await benchmarkClass.DoSnapExit();
    }

    public async Task NormalService_SnapExitAsync_NoExit()
    {
        await benchmarkClass.DoNoSnapExit();
    }

    public async Task NormalService_SnapExitAsync_Throw()
    {
        await benchmarkClass.DoThrowWithSnapAction();
    }

    public async Task NormalService_ExceptionsAsync()
    {
        await benchmarkClass.DoException();
    }
}
