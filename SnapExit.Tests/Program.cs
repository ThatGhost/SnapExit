using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using SnapExit.Tests;

var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddExporter(HtmlExporter.Default);
var summary = BenchmarkRunner.Run<Benchmarks2>(config);