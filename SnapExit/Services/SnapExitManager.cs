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

    private async Task Snap(Task task, ExecutionControlService executionControlService)
    {
        if (IsTaskCompleted(task)) return;

        var etc = executionControlService.GetTokenSource();
        var cancellationToken = etc.Token;

        Task originalTask = new Task(async () =>
        {
            await task;
        }, cancellationToken);

        try
        {
            if(!originalTask.IsCompleted) originalTask.Start();
            await originalTask;
        }
        catch (TaskCanceledException) { }

        if (originalTask.IsCanceled && onSnapExit is not null)
        {
            await onSnapExit.Invoke((TResponse?)(executionControlService.ResponseData), (TEnviroument?)executionControlService.EnviroumentData);
        }
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. Fire and forget
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async void RegisterSnapExit(Task task, ExecutionControlService? executionControlService = null)
    {
        await Snap(
            task,
            executionControlService ?? _executionControlService ?? throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters")
            );
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. waits untill its done
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async Task RegisterSnapExitAsync(Task task, ExecutionControlService? executionControlService = null)
    {
        await Snap(task,
            executionControlService ?? _executionControlService ?? throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters")
            );
    }

    protected virtual Task SnapExitResponse(TResponse? responseData, TEnviroument? enviroumentData)
    {
        return Task.CompletedTask;
    }
}
