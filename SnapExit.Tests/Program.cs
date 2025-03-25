using SnapExit.Tests;
using System.Diagnostics;

// Does not work yet because Async is causing problems
Benchmarks bm = new Benchmarks();
Stopwatch stopwatch = new Stopwatch();
Dictionary<string, List<long>> timings = new Dictionary<string, List<long>>();
const int TestAmount = 100;

timings.Add(nameof(bm.NormalService_ExceptionsAsync), new List<long>());
timings.Add(nameof(bm.NormalService_SnapExitAsync), new List<long>());
timings.Add(nameof(bm.NormalService_SnapExitAsync_NoExit), new List<long>());
timings.Add(nameof(bm.NormalService_SnapExitAsync_Throw), new List<long>());

_ = Task.Run(() =>
{
    Console.WriteLine($"testing x{TestAmount}...");
});

for (int i = 0; i < TestAmount; i++)
{
    await TestFunction(bm.NormalService_ExceptionsAsync, nameof(bm.NormalService_ExceptionsAsync));
    await TestFunction(bm.NormalService_SnapExitAsync, nameof(bm.NormalService_SnapExitAsync));
    await TestFunction(bm.NormalService_SnapExitAsync_NoExit, nameof(bm.NormalService_SnapExitAsync_NoExit));
    await TestFunction(bm.NormalService_SnapExitAsync_Throw, nameof(bm.NormalService_SnapExitAsync_Throw));
}

foreach (var timing in timings)
{
    double total = 0;
    double min = double.MaxValue;
    double max = double.MinValue;

    foreach (var item in timing.Value)
    {
        if (item < min) min = item;
        if (item > max) max = item;
        total += item;
    }

    Console.WriteLine($"{timing.Key}");
    Console.WriteLine($"   Total: {total}ms");
    Console.WriteLine($"   Average: {total/TestAmount}ms");
    Console.WriteLine($"   Min time: {min}ms");
    Console.WriteLine($"   Max time: {max}ms");
}

async Task TestFunction(Func<Task> func, string name)
{
    bm.Setup();
    stopwatch.Start();

    for (int i = 0; i < 10000; i++)
    {
        await func.Invoke();
    }

    stopwatch.Stop();
    timings[name].Add(stopwatch.ElapsedMilliseconds);
    stopwatch.Restart();
}