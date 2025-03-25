using SnapExit.Interfaces;

namespace SnapExit.Services;

public class SnapExitManager<TResponse,TEnviroument>
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

    private bool IsTaskCompleted(Task task)
    {
        return task.Status == TaskStatus.RanToCompletion
         || task.Status == TaskStatus.Faulted
         || task.Status == TaskStatus.Canceled;
    }

    private async Task DoTaskRace(Task task, ExecutionControlService executionControlService, CancellationTokenSource? linkedToken = null)
    {
        if (IsTaskCompleted(task)) return;
        
        var etc = executionControlService.GetTokenSource();
        var token = linkedToken?.Token ?? etc.Token;
        
        Task originalTask = Task.Run(async () => {
            await task;
        }, token);
        using Task cancelTask = Task.Delay(Timeout.Infinite, etc.Token);

        var completedTask = await Task.WhenAny(originalTask, cancelTask);

        if (completedTask == originalTask && !token.IsCancellationRequested)
        {
            etc.Cancel();
            return;
        }
        else
        {
            if (onSnapExit is not null)
                await onSnapExit.Invoke((TResponse?)(executionControlService.ResponseData), (TEnviroument?)executionControlService.EnviroumentData);
            if(linkedToken is not null && linkedToken.Token.CanBeCanceled) linkedToken.Cancel();
            if(etc.Token.CanBeCanceled) etc.Cancel();
        }
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. 
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async void RegisterSnapExit(Task task, CancellationTokenSource? linkedToken = null, ExecutionControlService? executionControlService = null)
    {
        await DoTaskRace(task, 
            executionControlService ?? _executionControlService ?? throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters"),
            linkedToken);
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. 
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async Task RegisterSnapExitAsync(Task task, CancellationTokenSource? linkedToken = null, ExecutionControlService? executionControlService = null)
    {
        await DoTaskRace(task,
            executionControlService ?? _executionControlService ?? throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters"),
            linkedToken);
    }

    protected virtual Task SnapExitResponse(TResponse? responseData, TEnviroument? enviroumentData)
    {
        return Task.CompletedTask;
    }
}
