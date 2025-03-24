using SnapExit.Example.Entities;
using SnapExit.Example.Services.Interfaces;
using SnapExit.Interfaces;

namespace SnapExit.Example.Services;

public class AssertionService : IAssertionService
{
    private readonly IExecutionControlService executionControlService;

    public AssertionService(IExecutionControlService executionControlService)
    {
        this.executionControlService = executionControlService;
    }

    public void Forbidden(string message, string token)
    {
        executionControlService.StopExecution(new CustomResponseData()
        {
            StatusCode = 403,
            Body = new { message },
            Headers = new Dictionary<string, string>(){ { "Auth", token } }
        });
    }

    public void NotFound()
    {
        executionControlService.StopExecution(new CustomResponseData()
        {
            StatusCode = 404,
        });
    }

    public void Teapot(string message)
    {
        executionControlService.StopExecution(new CustomResponseData()
        {
            StatusCode = 418,
            Body = new { message }
        });
    }
}
