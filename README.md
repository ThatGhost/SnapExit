
# Snap exit
nuget link: [SnapExit](https://www.nuget.org/packages/SnapExit/)

A middleware package that allows for exception-like behavior to validate state in an ASP.NET Core API project, but with all the performance benefits of cancellation tokens.

Any feedback is helpful. Please leave it in the Issues section.

## Performance
The package is meant to replace exceptions but still keeps (or improves) the performance of IActionResult

| Exceptions | IActionResult | SnapExit |
|------------|---------------|----------|
| 300ms      | 80ms          | 40ms     |

## Usage

Register SnapExit in your Program.cs

```csharp
    builder.Services.AddSnapExit(); // registers the services (with options)

    var app = builder.Build();
    app.AddSnapExit(); // registers the middleware
```

Inject the ExecutionControlService into your flow and use it like you would an exception:

```csharp
  public class YourService {
      private readonly IExecutionControlService _executionControlService;
      public YourService(IExecutionControlService executionControlService) {
          _executionControlService = executionControlService;
      }

      // New way
      public Task ThisShouldThrow() {
          _executionControlService.StopExecution();
      }

      // Old way
      public Task ThisShouldThrow() {
          throw new Exception();
      }
  }
```

This halts the flow of the request and immediately returns a response to the client.
You can also return a specialized response like so (body and headers are optional):
```csharp
  _executionControlService.StopExecution(new CustomResponseData() {
      StatusCode = 404,
      Body = new { message = "Not found" },
      Headers = new IDictionary<string, string>{ { "Authentication":"123abc456def" } }
  });
```
## Recommendation

I recommend creating your own service with specialized responses based on events.
For example:

```csharp
public class ErrorService {
    private readonly IExecutionControlService _executionControlService;
    public YourService(IExecutionControlService executionControlService) {
        _executionControlService = executionControlService;
    }

    public void NotFound(string message) {
        _executionControlService.StopExecution(new CustomResponseData() {
            StatusCode = 404,
            Body = new { message = message }
        });
    }

    public void Unauthorized() {
        _executionControlService.StopExecution(new CustomResponseData() {
            StatusCode = 401,
        });
    }

    // Etc
}
```

If there is a need for a default implementation, I might create a template.
