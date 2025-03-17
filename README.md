
# Snap exit
nuget link: [SnapExit](https://www.nuget.org/packages/SnapExit/)

A middleware package that allows for exception like behaviour to validate state in a given ASP.NET Core API project. But with all the performance benifits of cancelation tokens.

Any feedback is helpfull. Please leave it in the Issues section

## Performance
The package is meant to replace exceptions, but on average still has a 2x performance gain over the default IActionResult.

| Exceptions | IActionResult | SnapExit |
|------------|---------------|----------|
| 300ms      | 80ms          | 40ms     |

## Usage

Register snap exit in your program.cs

```csharp
    builder.Services.AddSnapExit(); // registers the services (with options)

    var app = builder.Build();
    app.AddSnapExit(); // registers the middleware
```

Inject the execution controll service into your flow and use it like you would an exception;

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

This will halt the flow of the request and immediatly return a response to the client.
You can also return a specialized response like so (body and headers are optional)
```csharp
  _executionControlService.StopExecution(new CustomResponseData() {
      StatusCode = 404,
      Body = new { message = "Not found" },
      Headers = new IDictionary<string, string>{ { "Authentication":"123abc456def" } }
  });
```
## Recommendation

I recommend creating your own service that has specialized reponses based on events.
for example.

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

If there is need for a default one i might make a template.
