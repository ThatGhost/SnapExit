using SnapExit.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SnapExit.Tests.Services.BenchmarksHelpers;

class SnapExitBenchmarkClass : ExitManager<object>
{
    public async Task SnapExit_StopExecution()
    {
        await SetupSnapExit(new Task(async() =>
        {
            await Snap.Exit();
        }));
    }

    public async Task SnapExit_HappyPath()
    {
        await SetupSnapExit(new Task(() =>
        {
            
        }));
    }

    public async Task SnapExit_Exception()
    {
        try
        {
            await SetupSnapExit(new Task(() =>
            {
                throw new Exception();
            }));
        }
        catch(Exception) { }
    }
}

public sealed class VanillaBenchmarkClass
{
    public async Task Vanilla_Exception()
    {
        try
        {
            await Task.Run(async () =>
            {
                await DoWork();
                throw new Exception();
            });
        }
        catch (Exception)
        {
        }
    }

    public async Task Vanilla_HappyPath()
    {
        try
        {
            await Task.Run(async () =>
            {
                await DoWork();
            });
        }
        catch (Exception)
        {
        }
    }

    private Task DoWork()
    {
        return Task.CompletedTask;
    }
}
