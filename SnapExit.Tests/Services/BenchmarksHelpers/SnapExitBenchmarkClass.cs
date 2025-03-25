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

    public async Task DoSnapExit()
    {
        await RegisterSnapExitAsync(Task.Run(() =>
        {
            return executionControlService.StopExecution();
        }));
    }

    public async Task DoNoSnapExit()
    {
        await RegisterSnapExitAsync(Task.Run(() =>
        {
            return Task.CompletedTask;
        }));
    }

    public async Task DoThrowWithSnapAction()
    {
        try
        {
            await RegisterSnapExitAsync(Task.Run(() =>
            {
                throw new Exception();
            }));
        }
        catch(Exception) { }

    }

    public async Task DoException()
    {
        try
        {
            await Task.Run(() =>
            {
                throw new Exception();
            });
        }
        catch (Exception){
        }
    }
}
