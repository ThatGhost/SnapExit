using SnapExit.Interfaces;

namespace SnapExit.Services;

public class SnapExitManager<TResponse,TEnvironment>
{
    /// <summary>
    /// Provides the response and environment data when invoking <see cref="SnapExitManager{TResponse,TEnvironment}.OnSnapExit"/>.
    /// Asynchronous handlers should register their asynchronous continuations via <see cref="OnSnapExitEventArgs.AddTask"/>.
    /// </summary>
    protected sealed class OnSnapExitEventArgs : EventArgs
    {
        internal OnSnapExitEventArgs(TResponse? responseData, TEnvironment? environmentData)
        {
            ResponseData = responseData;
            EnvironmentData = environmentData;
        }

        public TResponse? ResponseData { get; }
        public TEnvironment? EnvironmentData { get; }
        internal List<Task> Tasks { get; } = [];
        public void AddTask(Task task) => Tasks.Add(task);
    }
    protected event EventHandler<OnSnapExitEventArgs>? OnSnapExit;

    private ExecutionControlService? _executionControlService { get; set; }

    protected SnapExitManager() {
        OnSnapExit += SnapExitResponse;
    }
    protected SnapExitManager(ExecutionControlService executionControlService)
        :this()
    {
        _executionControlService = executionControlService;
    }

    protected SnapExitManager(IExecutionControlService executionControlService)
        :this()
    {
        _executionControlService = (ExecutionControlService)executionControlService;
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
            await InvokeOnSnapExit(executionControlService);
            
            if(token.CanBeCanceled)
            {
                originalTs.Cancel();
            }
        }
    }

    private Task InvokeOnSnapExit(ExecutionControlService executionControlService)
    {
        var responseData = (TResponse?)executionControlService.ResponseData;
        var environmentData = (TEnvironment?)executionControlService.EnvironmentData;

        var args = new OnSnapExitEventArgs(responseData, environmentData);
        
        OnSnapExit?.Invoke(sender:this, args);

        var result = Task.WhenAll(args.Tasks);

        return result;
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

    protected virtual void SnapExitResponse(object? sender,OnSnapExitEventArgs args)
    { }
}
