using SnapExit.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SnapExit.Tests.Services.BenchmarksHelpers;

class SnapExitBenchmarkClass : SnapExitManager<object>
{
    public async Task SnapExit_StopExecution()
    {
        await RegisterSnapExitAsync(new Task(async() =>
        {
            await Snap.Exit();
        }));
    }

    public async Task SnapExit_HappyPath()
    {
        await RegisterSnapExitAsync(new Task(() =>
        {
            
        }));
    }

    public async Task SnapExit_Exception()
    {
        try
        {
            await RegisterSnapExitAsync(new Task(() =>
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
