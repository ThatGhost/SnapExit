using Microsoft.AspNetCore.Mvc;
using SnapExit.Example.Entities;
using SnapExit.Interfaces;
using SnapExit.Services;

namespace SnapExit.Benchmark.Controller;

[ApiController]
[Route("[controller]")]
public class BenchMarkController : ControllerBase
{
    private readonly ExecutionControlService _service;

    public BenchMarkController(ExecutionControlService service)
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
        await Task.Delay(1000);
        await Task.Delay(1000);
    }
}
