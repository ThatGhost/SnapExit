using SnapExit.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SnapExit.Tests.Services.BenchmarksHelpers;

class SnapExitBenchmarkClassStack : SnapExitManager<object, object>
{
    private readonly ExecutionControlService executionControlService;
    private const int stackDepth = 100;

    public SnapExitBenchmarkClassStack(
        ExecutionControlService executionControlService): base(executionControlService)
    {
        this.executionControlService = executionControlService;
    }

    public async Task SnapExit_StopExecution()
    {
        await RegisterSnapExitAsync(StackerStopExecution(stackDepth));
    }

    private async Task StackerStopExecution(int i)
    {
        if (i != 0) await StackerStopExecution(i - 1);
        await executionControlService.StopExecution();
    }

    public async Task SnapExit_HappyPath()
    {
        await RegisterSnapExitAsync(StackerHappyPath(stackDepth));
    }

    private async Task StackerHappyPath(int i)
    {
        if (i != 0) await StackerStopExecution(i - 1);
    }


    public async Task SnapExit_Exception()
    {
        try
        {
            await RegisterSnapExitAsync(StackerException(stackDepth));
        }
        catch (Exception) { }
    }

    private async Task StackerException(int i)
    {
        if (i != 0) await StackerStopExecution(i - 1);
        await executionControlService.StopExecution();
    }

    public async Task Vanilla_Exception()
    {
        try
        {
            await StackerException(stackDepth);
        }
        catch (Exception){
        }
    }

    public async Task Vanilla_HappyPath()
    {
        try
        {
            await StackerHappyPath(stackDepth);
        }
        catch (Exception)
        {
        }
    }
}
