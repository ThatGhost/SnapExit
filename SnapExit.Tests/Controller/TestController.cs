using Microsoft.AspNetCore.Mvc;
using SnapExit.Example.Entities;
using SnapExit.Services;

namespace SnapExit.Benchmark.Controller;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ExecutionControlService _service;

    public TestController(ExecutionControlService service)
    {
        _service = service;
    }

    [HttpGet("Exception")]
    public IActionResult Exception()
    {
        throw new Exception();
    }

    [HttpGet("Result")]
    public IActionResult Result()
    {
        return NotFound();
    }

    [HttpGet("SnapExit")]
    public async Task SnapExit()
    {
        // Example of a response
        _service.StopExecution(new CustomResponseData()
        {
            Body = new
            {
                Message = "Wowzer"
            },
            StatusCode = 404
        });
        await Task.Delay(10000); // makes sure the next line is not done
    }
}
