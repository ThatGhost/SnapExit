using SnapExit.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SnapExit.Tests.Services.BenchmarksHelpers;

class SnapExitBenchmarkClass : SnapExitManager<object, object>
{
    private readonly ExecutionControlService executionControlService;

    public SnapExitBenchmarkClass(
        ExecutionControlService executionControlService): base(executionControlService)
    {
        this.executionControlService = executionControlService;
    }

    public async Task SnapExit_StopExecution()
    {
        await RegisterSnapExitAsync(new Task(async() =>
        {
            await DoWork();
            await executionControlService.StopExecution();
        }));
    }

    public async Task SnapExit_HappyPath()
    {
        await RegisterSnapExitAsync(new Task(async () =>
        {
            await DoWork();
        }));
    }

    public async Task SnapExit_Exception()
    {
        try
        {
            await RegisterSnapExitAsync(new Task(async () =>
            {
                await DoWork();
                throw new Exception();
            }));
        }
        catch(Exception) { }
    }

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
        catch (Exception){
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
