using SnapExit.Entities;
using SnapExit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapExit.Services
{
    public class SnapExitManager
    {
        protected Func<object, object, Task>? SnapReaction = null;

        public async void RegisterSnapAction(Task task, ExecutionControlService executionControlService)
        {
            if (SnapReaction is null) throw new Exception("You must register the SnapReaction field");

            var cts = executionControlService.GetTokenSource();

            // Setup task race
            using Task tokenTask = Task.Delay(Timeout.Infinite, cts.Token);
            try
            {
                var completedTask = await Task.WhenAny(task, tokenTask);
                if (completedTask == task)
                {
                    executionControlService.StopExecution();
                    return;
                }
            }
            catch (SnapExitException)
            {
                if (!tokenTask.IsCanceled) throw;
            }

            await SnapReaction.Invoke(executionControlService.ResponseData ?? new { }, executionControlService.EnviroumentData);
        }

        public async void RegisterSnapExit(Task task, CancellationToken linkedToken, ExecutionControlService executionControlService)
        {
            if (SnapReaction is null) throw new Exception("You must register the SnapReaction field");

            // Setup task race
            using Task tokenTask = Task.Delay(Timeout.Infinite, linkedToken);
            try
            {
                var completedTask = await Task.WhenAny(task, tokenTask);
                if (completedTask == task)
                {
                    executionControlService.StopExecution();
                    return;
                }
            }
            catch (SnapExitException)
            {
                if (!tokenTask.IsCanceled) throw;
            }

            await SnapReaction.Invoke(executionControlService.ResponseData ?? new { }, executionControlService.EnviroumentData);
        }

    }
}
