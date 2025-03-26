
# Snap exit
nuget link: [SnapExit](https://www.nuget.org/packages/SnapExit/)

A nuget package that allows for exception-like behavior to validate state in any ASP.NET project, but with all the performance benefits of cancellation tokens.

Any feedback is helpful. Please leave it in the Issues section.

## Performance
The package is meant to replace exceptions but still keeps the performance cost at a minimum.
Still with a dept of 1. The performance is still **x10** over regular exception.
Most of the lost performance in this benchmarks is because of initialization. When u negate for that the loss is even smaller (-200ns).

| Exceptions | SnapExit | HappyPath | SnapExit HappyPath |
|------------|----------|-----------|--------------------|
| 11_500ns   | 1100ns   | 740ns     | 1100ns             |


When u increase the stack dept the performance of exceptions becomes worse. So with a 100 deep stack these are the benchmarking results.
As you can see with a deep stack debt the performance increase can be up to **x50**.

| Exceptions | SnapExit | HappyPath | SnapExit HappyPath |
|------------|----------|-----------|--------------------|
| 460μs      | 8μs      | 1,3μs     | 1,4μs              |

## Usage

Register SnapExit in your Program.cs (you can also just instantiate the manager)

```csharp
    builder.Services.AddSnapExit(); // registers the services
```

Inject the ExecutionControlService into your flow and use it like you would an exception:

```csharp
  public class YourService {
      private readonly IExecutionControlService _executionControlService;
      public YourService(IExecutionControlService executionControlService) {
          _executionControlService = executionControlService;
      }

      // New way
      public async Task ThisShouldThrow() {
          await _executionControlService.StopExecution();
      }

      // Old way
      public Task ThisShouldThrow() {
          throw new Exception();
      }
  }
```

This halts the flow of task immediatly, you can even return custom defined data to the return point:
```csharp
  _executionControlService.StopExecution(new {
      // any response data can be passed here
  });
```

To make this work you will need to define a return point for SnapExit. This is made easy by using the SnapExitManager base class!
```csharp
  public class SnapExitReturnPoint : SnapExitManager<ResponseData, EnviroumentData> // the generics are to be implemented by you
  {
      public void ThisCanBeAnyPointOfCode() {
          onSnapExit += OnSnapExit; // register the callback function or use the virtual protected callback
          Task task = SomeLongTask();
          RegisterSnapExit(task);
      }

      private Task OnSnapExit(ResponseData responseData, EnviroumentData enviroumentData) { // response and enviroument is data passed at error time and register time
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

    public Task Foo(string message) {
        return _executionControlService.StopExecution(new {
            message
        });
    }

    public Task Bar() {
        return _executionControlService.StopExecution(new {
            message = "Something went wrong!"
        });
    }

    // Etc
}
```

If there is a need for a default implementation, I might create a template.
