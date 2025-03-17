using Microsoft.AspNetCore.Mvc;
using SnapExit.Interfaces;
using SnapExit.Services;

namespace SnapExit.Benchmark
{
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
            _service.StopExecution();
            await Task.Delay(5000);
        }
    }

}
