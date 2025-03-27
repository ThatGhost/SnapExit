using SnapExit.Example.Entities;
using SnapExit.Example.Services.Interfaces;
using SnapExit.Services;

namespace SnapExit.Example.Services;

public sealed class AssertionService : IAssertionService
{
    public async Task Forbidden(string message, string token)
    {
        await Snap.Exit(new CustomResponseData()
        {
            StatusCode = 403,
            Body = new { message },
            Headers = new Dictionary<string, string>(){ { "Auth", token } }
        });
    }

    public async Task NotFound()
    {
        await Snap.Exit(new CustomResponseData()
        {
            StatusCode = 404,
        });
    }

    public async Task Teapot(string message)
    {
        await Snap.Exit(new CustomResponseData()
        {
            StatusCode = 418,
            Body = new { message }
        });
    }
}
