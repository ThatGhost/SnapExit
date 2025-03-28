using SnapExit.Interfaces;

namespace SnapExit.Services;

public class SnapExitManager<TResponse,TEnvironment>
{
    protected delegate Task OnSnapExit(TResponse? responseData, TEnviroument? enviroumentData);
    protected OnSnapExit onSnapExit;

    private ExecutionControlService? _executionControlService { get; set; }

    protected SnapExitManager() {
        onSnapExit += SnapExitResponse;
    }
    protected SnapExitManager(ExecutionControlService executionControlService)
    {
        _executionControlService = executionControlService;
        onSnapExit += SnapExitResponse;
    }

    protected SnapExitManager(IExecutionControlService executionControlService)
    {
        _executionControlService = (ExecutionControlService)executionControlService;
        onSnapExit += SnapExitResponse;
    }
    private async Task DoTaskRace(Task task, ExecutionControlService executionControlService)
    {
        if (task.IsCompleted) return;
        
        var etc = executionControlService.GetTokenSource();

        var originalTs = new CancellationTokenSource();
        var token = originalTs.Token;
        
        Task originalTask = Task.Run(async () => {
            await task;
        }, token);
        using Task cancelTask = Task.Delay(Timeout.Infinite, etc.Token);

        var completedTask = await Task.WhenAny(originalTask, cancelTask);

        if (completedTask == originalTask && !etc.Token.IsCancellationRequested)
        {
            etc.Cancel();
        }
        else
        {
            if (onSnapExit is not null)
                await onSnapExit.Invoke((TResponse?)(executionControlService.ResponseData), (TEnvironment?)executionControlService.EnviroumentData);
            if(token.CanBeCanceled)
            {
                originalTs.Cancel();
            }
        }
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. Fire and forget
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async void RegisterSnapExit(Task task, ExecutionControlService? executionControlService = null)
    {
        await DoTaskRace(
            task,
            executionControlService ?? _executionControlService ?? throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters")
            );
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. waits untill its done
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async Task RegisterSnapExitAsync(Task task, ExecutionControlService? executionControlService = null)
    {
        await DoTaskRace(task,
            executionControlService ?? _executionControlService ?? throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters")
            );
    }

    protected virtual Task SnapExitResponse(TResponse? responseData, TEnvironment? enviroumentData)
    {
        return Task.CompletedTask;
    }
}
