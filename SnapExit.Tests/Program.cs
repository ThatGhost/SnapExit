using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using SnapExit.Tests;

var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddExporter(HtmlExporter.Default);
//BenchmarkRunner.Run<Benchmarks>(config);
BenchmarkRunner.Run<BenchmarksStack>(config);

//await new Benchmarks2().SnapExit_HappyPath();