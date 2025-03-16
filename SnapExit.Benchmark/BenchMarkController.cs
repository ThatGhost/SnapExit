using Microsoft.AspNetCore.Mvc;
using SnapExit.Interfaces;

namespace SnapExit.Benchmark
{
    [ApiController]
    [Route("[controller]")]
    public class BenchMarkController : ControllerBase
    {
        private readonly IExecutionControlService _service;

        public BenchMarkController(IExecutionControlService service)
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
        public void SnapExit()
        {
            _service.StopExecution();
        }
    }

}
