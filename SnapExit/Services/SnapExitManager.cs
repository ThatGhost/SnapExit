namespace SnapExit.Services;

public class SnapExitManager<TResponse>
{
    protected delegate Task OnSnapExit(TResponse? responseData);
    protected OnSnapExit? onSnapExit;

    private bool IsTaskCompleted(Task task)
    {
        return task.Status == TaskStatus.RanToCompletion
         || task.Status == TaskStatus.Faulted
         || task.Status == TaskStatus.Canceled;
    }

    private async Task SnapRegister(Task task)
    {
        if (IsTaskCompleted(task)) return;

        var etc = Snap.GetTokenSource(true);

        if(etc.Item2)
        {
            Task originalTask = new Task(async () =>
            {
                await task;
            }, etc.Item1.Token);

            try
            {
                if (!originalTask.IsCompleted) originalTask.Start();
                await originalTask;
                if(!originalTask.IsCanceled) Snap.GetTokenSource(true);
            }
            catch (TaskCanceledException) { }

            if (originalTask.IsCanceled && onSnapExit is not null)
            {
                await onSnapExit.Invoke((TResponse?)(Snap.GetResponseData(etc.Item1.Token)));
            }
        }
        else // edge case where exit was called before register
        {
            etc.Item1.Cancel();
            if(onSnapExit is not null)
                await onSnapExit.Invoke((TResponse?)(Snap.GetResponseData(etc.Item1.Token)));
        }

    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. Fire and forget
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async void RegisterSnapExit(Task task)
    {
        await SnapRegister(task);
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. waits untill its done
    /// </summary>
    /// <param name="task">The Task that uses SnapExit</param>
    /// <param name="linkedToken">A token already in use by your program</param>
    /// <param name="executionControlService">If the ExecutionControlService is not passed through the constructor you need to pass it here</param>
    /// <exception cref="ArgumentException">If the ExecutionControlService was not passed in either the constructor or the function</exception>
    public async Task RegisterSnapExitAsync(Task task)
    {
        await SnapRegister(task);
    }
}
