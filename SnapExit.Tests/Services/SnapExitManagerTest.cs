using SnapExit.Services;
using SnapExit.Tests.Entities;

namespace SnapExit.Tests.Services
{
    class SnapExitManagerTest : SnapExitManager<TestResponseObject>
    {
        // use to Assert in the unit test
        public TestResponseObject? response;

        public async Task SnapExit_SingleResponseTest(TestResponseObject response)
        {
            onSnapExit += (response) =>
            {
                this.response = response;
                return Task.CompletedTask;
            };
            await RegisterSnapExitAsync(SomeLongTask(response,0));
        }

        public async Task SnapExit_MultipleResponseTest(int delay, TestResponseObject data)
        {
            onSnapExit += (response) =>
            {
                if(response?.Message != data.Message) throw new Exception("Response data is not the same");
                return Task.CompletedTask;
            };
            await RegisterSnapExitAsync(SomeLongTask(data, delay));
        }

        private async Task SomeLongTask(TestResponseObject response, int delay)
        {
            await Task.Delay(delay);
            await Snap.Exit(response);
        }
    }
}
