using Xunit;
using SnapExit.Tests.Services;
using SnapExit.Tests.Entities;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SnapExit.Benchmark;

[Collection("Sequential")]
public class Tests
{
    [Fact(Skip = "")]
    public async Task Service_NoSnapExit_Immediate()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();
        TestResponseObject reponse = new TestResponseObject() { Message = "This is a message that has passed" };

        // Act
        await testService.SnapExit_NoExit(0, reponse);
    }

    [Fact(Skip = "")]
    public async Task Service_NoSnapExit_Delay()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();
        TestResponseObject reponse = new TestResponseObject() { Message = "This is a message that has passed" };

        // Act
        await testService.SnapExit_NoExit(50, reponse);
    }

    [Fact(Skip = "")]
    public async Task Service_ShouldSnapExit_ImmediateSnapExit()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();
        TestResponseObject reponse = new TestResponseObject() { Message = "This is a message that has passed" };

        // Act
        await testService.SnapExit_WithDelayTest(0, reponse);
    }

    [Fact(Skip = "")]
    public async Task Service_ShouldSnapExit_DelayedSnapExit()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();
        TestResponseObject reponse = new TestResponseObject() { Message = "This is a message that has passed" };

        // Act
        await testService.SnapExit_WithDelayTest(500, reponse);
    }

    [Fact(Skip = "")]
    public async Task Service_ShouldSnapExit_TimeDifferentSnapExits()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();

        // Act
        var t1 = testService.SnapExit_WithDelayTest(500, new TestResponseObject() { Message = "1" });
        var t2 = testService.SnapExit_WithDelayTest(0, new TestResponseObject() { Message = "2" });
        var t3 = testService.SnapExit_WithDelayTest(250, new TestResponseObject() { Message = "3" });

        // Assert
        await Task.WhenAll(t1, t2, t3);
    }

    [Fact(Skip = "")]
    public async Task Service_ShouldSnapExit_ConcurrentSnapExits()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();

        // Act
        var t1 = testService.SnapExit_WithDelayTest(500, new TestResponseObject() { Message = "1" });
        var t2 = testService.SnapExit_WithDelayTest(500, new TestResponseObject() { Message = "2" });
        var t3 = testService.SnapExit_WithDelayTest(500, new TestResponseObject() { Message = "3" });

        // Assert
        await Task.WhenAll(t1, t2, t3);
    }

    [Fact(Skip = "i dont have a solve for this yet")]
    public async Task Service_ShouldSnapExit_ImmediateConcurrentSnapExits()
    {
        // Arrange
        SnapExitManagerTest testService = new SnapExitManagerTest();

        // Act
        var t1 = testService.SnapExit_WithDelayTest(0, new TestResponseObject() { Message = "1" });
        var t2 = testService.SnapExit_WithDelayTest(0, new TestResponseObject() { Message = "2" });
        var t3 = testService.SnapExit_WithDelayTest(0, new TestResponseObject() { Message = "3" });

        // Assert
        await Task.WhenAll(t1, t2, t3);
    }
}
