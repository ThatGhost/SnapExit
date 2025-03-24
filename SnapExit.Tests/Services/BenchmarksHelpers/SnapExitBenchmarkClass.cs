using SnapExit.Services;

namespace SnapExit.Tests.Services.BenchmarksHelpers;

class SnapExitBenchmarkClass : SnapExitManager<object, object>
{
    private readonly ExecutionControlService executionControlService;
    private readonly Random random = new Random();

    public SnapExitBenchmarkClass(ExecutionControlService executionControlService): base(executionControlService)
    {
        this.executionControlService = executionControlService;
    }

    public async Task<int> DoSnapExit()
    {
        int rnd = random.Next();
        Task newTask = new Task(() => { rnd++; executionControlService.StopExecution(); Task.Delay(10000); });
        await newTask;
        RegisterSnapExit(newTask);
        return rnd;
    }

    public async Task<int> DoSnapExitAfterRegister()
    {
        int rnd = random.Next();
        Task newTask = new Task(() => { rnd++; executionControlService.StopExecution(); Task.Delay(10000); });
        RegisterSnapExit(newTask);
        await newTask;
        return rnd;
    }

    public async Task<int> DoException()
    {
        int rnd = random.Next();
        try
        {
            var newTask = new Task(() => { rnd++; throw new Exception(); });
            await newTask;
            return rnd;
        }
        catch (Exception){ return rnd; }
    }
}
