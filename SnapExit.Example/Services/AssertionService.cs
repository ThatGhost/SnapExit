using SnapExit.Example.Entities;
using SnapExit.Example.Services.Interfaces;
using SnapExit.Interfaces;

namespace SnapExit.Example.Services;

public sealed class AssertionService : IAssertionService
{
    private readonly ISnap executionControlService;

    public AssertionService(ISnap executionControlService)
    {
        this.executionControlService = executionControlService;
    }

    public void Forbidden(string message, string token)
    {
        executionControlService.Exit(new CustomResponseData()
        {
            StatusCode = 403,
            Body = new { message },
            Headers = new Dictionary<string, string>(){ { "Auth", token } }
        });
    }

    public void NotFound()
    {
        executionControlService.Exit(new CustomResponseData()
        {
            StatusCode = 404,
        });
    }

    public void Teapot(string message)
    {
        executionControlService.Exit(new CustomResponseData()
        {
            StatusCode = 418,
            Body = new { message }
        });
    }
}
