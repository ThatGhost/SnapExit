using BenchmarkDotNet.Running;
using SnapExit.Tests;

// Does not work yet because Async is causing problems
var summary = BenchmarkRunner.Run<Benchmarks>();