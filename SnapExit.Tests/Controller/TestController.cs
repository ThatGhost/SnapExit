using Microsoft.AspNetCore.Mvc;
using SnapExit.Example.Entities;
using SnapExit.Services;

namespace SnapExit.Benchmark.Controller;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("SnapExit")]
    public async Task SnapExit()
    {
        // Example of a response
        await Snap.Exit(new CustomResponseData()
        {
            Body = new
            {
                Message = "Wowzer"
            },
            StatusCode = 403
        });
        throw new Exception("this better not be thrown");
    }

    [HttpGet("SnapExit/succes")]
    public Task SnapExitNoProblems()
    {
        return Task.CompletedTask;
    }
}
