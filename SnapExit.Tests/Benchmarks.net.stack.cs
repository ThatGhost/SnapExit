﻿using BenchmarkDotNet.Attributes;
using SnapExit.Tests.Services.BenchmarksHelpers;

namespace SnapExit.Tests;

public class BenchmarksStack
{
    [Benchmark]
    public async Task SnapExit_WithExit()
    {
        var bm = new SnapExitBenchmarkClassStack();
        await bm.SnapExit_StopExecution();
    }

    [Benchmark]
    public async Task SnapExit_HappyPath()
    {
        var bm = new SnapExitBenchmarkClassStack();
        await bm.SnapExit_HappyPath();
    }

    [Benchmark]
    public async Task SnapExit_WithException()
    {
        var bm = new SnapExitBenchmarkClassStack();
        await bm.SnapExit_Exception();
    }

    [Benchmark]
    public async Task Vanilla_HappyPath()
    {
        var bm = new VannilaBenchmarkClassStack();
        await bm.Vanilla_HappyPath();
    }

    [Benchmark]
    public async Task Vanilla_Exception()
    {
        var bm = new VannilaBenchmarkClassStack();
        await bm.Vanilla_Exception();
    }
}
