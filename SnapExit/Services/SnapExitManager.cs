using SnapExit.Entities;
using SnapExit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    private async Task DoTaskRace(Task task, ExecutionControlService executionControlService)
    {
        var token = executionControlService.GetTokenSource().Token;
        using Task tokenTask = Task.Delay(Timeout.Infinite, token);
        try
        {
            task.Start();
            var completedTask = await Task.WhenAny(task, tokenTask);
            if (completedTask == task && !token.IsCancellationRequested)
            {
                executionControlService.StopExecution();
                return;
            }
            else
            {
                if (onSnapExit is not null)
                    await onSnapExit.Invoke((TResponse?)(executionControlService.ResponseData), (TEnviroument?)executionControlService.EnviroumentData);
            }
        }
        catch (SnapExitException)
        {
            if (!tokenTask.IsCanceled) return;
        }
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. 
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async void RegisterSnapAction(Func<Task> task, ExecutionControlService? executionControlService = null)
    {
        Task t = new Task(() =>
        {
            task.Invoke();
        });

        _executionControlService ??= executionControlService;
        if (_executionControlService is null)
            throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters"); ;                

        await DoTaskRace(t, _executionControlService);
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. 
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async void RegisterSnapExit(Func<Task> task, CancellationTokenSource linkedToken, ExecutionControlService? executionControlService = null)
    {
        Task t = new Task(async () =>
        {
            await task.Invoke();
        });

        _executionControlService ??= executionControlService;
        if (_executionControlService is null)
            throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters"); ;

        await DoTaskRace(t, _executionControlService);
        linkedToken.Cancel();
    }

    public async Task RegisterSnapActionAsync(Func<Task> task, ExecutionControlService? executionControlService = null)
    {
        Task t = new Task(async () =>
        {
            await task.Invoke();
        });

        _executionControlService ??= executionControlService;
        if (_executionControlService is null)
            throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters"); ;

        await DoTaskRace(t, _executionControlService);
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. 
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async Task RegisterSnapExitASync(Func<Task> task, CancellationTokenSource linkedToken, ExecutionControlService? executionControlService = null)
    {
        Task t = new Task(async () =>
        {
            await task.Invoke();
        });

        _executionControlService ??= executionControlService;
        if (_executionControlService is null)
            throw new ArgumentException("ExecutionControlService not registered. Add it to the constructor or pass it through the parameters"); ;

        await DoTaskRace(t, _executionControlService);
        linkedToken.Cancel();
    }

    protected virtual Task SnapExitResponse(TResponse? responseData, TEnviroument? enviroumentData)
    {
        return Task.CompletedTask;
    }
}
