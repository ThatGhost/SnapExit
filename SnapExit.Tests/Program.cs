using BenchmarkDotNet.Running;
using SnapExit.Tests;

var summary = BenchmarkRunner.Run<Benchmarks>();