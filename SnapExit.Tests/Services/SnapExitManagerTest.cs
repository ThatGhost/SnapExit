using SnapExit.Services;
using SnapExit.Tests.Entities;

namespace SnapExit.Tests.Services
{
    class SnapExitManagerTest : ExitManager<TestResponseObject>
    {
        public async Task SnapExit_WithDelayTest(int delay, TestResponseObject data)
        {
            await SetupSnapExit(SomeLongTask(data, delay), (response) =>
            {
                if (response?.Message != data.Message) throw new Exception("Response data is not the same");
                return Task.CompletedTask;
            });
        }

        public async Task SnapExit_NoExit(int delay, TestResponseObject data)
        {
            await SetupSnapExit(new Task(async () => { await Task.Delay(delay); }), (response) =>
            {
                if (response?.Message != data.Message) throw new Exception("Response data is not the same");
                return Task.CompletedTask;
            });
        }

        private async Task SomeLongTask(TestResponseObject response, int delay)
        {
            await Task.Delay(delay);
            await Snap.Exit(response);
        }
    }
}
