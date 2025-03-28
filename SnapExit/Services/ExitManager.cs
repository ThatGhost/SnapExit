namespace SnapExit.Services;

public class ExitManager<TResponse>
{
    private bool IsTaskCompleted(Task task)
    {
        return task.Status == TaskStatus.RanToCompletion
         || task.Status == TaskStatus.Faulted
         || task.Status == TaskStatus.Canceled;
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. analog to try catch
    /// </summary>
    /// <param name="happyPath">The Task that can exit</param>
    /// <param name="faultyPath">The Task which will happen on an exit</param>
    public async Task SetupSnapExit(Task happyPath, Func<TResponse?, Task> faultyPath)
    {
        if (IsTaskCompleted(happyPath)) return;

        var etc = Snap.GetTokenSource(true);

        if (etc.Item2)
        {
            Task originalTask = new Task(async () =>
            {
                await happyPath;
            }, etc.Item1.Token);

            try
            {
                if (!originalTask.IsCompleted) originalTask.Start();
                await originalTask;
                if (!originalTask.IsCanceled) Snap.GetTokenSource(true);
            }
            catch (TaskCanceledException) { }

            if (originalTask.IsCanceled)
            {
                await faultyPath.Invoke((TResponse?)Snap.GetResponseData(etc.Item1.Token));
            }
        }
        else // edge case where exit was called before setup
        {
            etc.Item1.Cancel();
            await faultyPath.Invoke((TResponse?)Snap.GetResponseData(etc.Item1.Token));
        }
    }

    /// <summary>
    /// This registers that the current task is supposed to use snapExit. analog to try catch
    /// </summary>
    /// <param name="happyPath">The Task that can exit</param>
    public async Task SetupSnapExit(Task happyPath)
    {
        if (IsTaskCompleted(happyPath)) return;

        var etc = Snap.GetTokenSource(true);

        if (etc.Item2)
        {
            Task originalTask = new Task(async () =>
            {
                await happyPath;
            }, etc.Item1.Token);

            try
            {
                if (!originalTask.IsCompleted) originalTask.Start();
                await originalTask;
                if (!originalTask.IsCanceled) Snap.GetTokenSource(true);
            }
            catch (TaskCanceledException) { }
        }
        else // edge case where exit was called before setup
        {
            etc.Item1.Cancel();
        }
    }
}
