
# Snap exit
nuget link: [SnapExit](https://www.nuget.org/packages/SnapExit/)

A nuget package that allows for exception-like behavior to validate state in any ASP.NET project, but with all the performance benefits of cancellation tokens.

Any feedback is helpful. Please leave it in the Issues section.

## Performance
The package is meant to replace exceptions but still keeps (or improves) the performance of IActionResult

| Exceptions | IActionResult | SnapExit |
|------------|---------------|----------|
| 200-300ms  | 60-70ms       | 20-30ms  |

## Usage

Register SnapExit in your Program.cs

```csharp
    builder.Services.AddSnapExit(); // registers the services (with options to add enviroument variables)
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

This halts the flow of task immediatly, you can even return custom defined data to the return point:
```csharp
  _executionControlService.StopExecution(new CustomResponseData() {
      StatusCode = 404,
      Body = new { message = "Not found" },
      Headers = new IDictionary<string, string>{ { "Authentication":"123abc456def" } }
  });
```

To make this work you will need to define a return point for SnapExit. This is made easy by using the SnapExitManager base class!
```csharp
  public class SnapExitReturnPoint : SnapExitManager
  {
      public void ThisCanBeAnyPointOfCode() {
          onSnapExit += OnSnapExit; // register the callback function
          Task task = SomeLongTask();
          RegisterSnapExit(task);
      }

      private Task OnSnapExit(object stateData, object enviroumentData) {
          // Do some code here related to an exit
      }
  }
```

Also look at the example to see how you can turn it into a middleware for ASP.NET Core api!

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
