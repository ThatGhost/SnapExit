
# Snap exit

A nuget package that allows for exception like behaviour to validate state in a given ASP.NET Core API project. But with all the performance benifits of cancelation tokens.

Any feedback is helpfull. Please leave it in the Issues section



## Usage

Register snap exit in your program.cs

```csharp
    builder.Services.AddSnapExit(); // registers the services (with options)

    var app = builder.Build();
    app.AddSnapExit(); // registers the middleware
```

Inject the excecution controll service into your flow and use it like you would an exception;

```csharp
  public class YourService {
      private readonly IExcecutionControlService _excecutionControlService;
      public YourService(IExcecutionControlService excecutionControlService) {
          _excecutionControlService = excecutionControlService;
      }

      public Task ThisShouldThrow() {
          _excecutionControlService.StopExcecution();
      }
  }
```

This will halt the flow of the request and immediatly return a response to the client.
You can also return a specialized response like so (body and headers are optional)
```csharp
  _excecutionControlService.StopExcecution(new CustomResponseData() {
      StatusCode = 404,
      Body = new { message = "Not found" },
      Headers = new IDictionary<string, string>{ { "Authentication":"123abc456def" } }
  });
```
## Recomendation

I recommend creating your own service that has specialized reponses based on events.
for example.

```csharp
public class ErrorService {
    private readonly IExcecutionControlService _excecutionControlService;
    public YourService(IExcecutionControlService excecutionControlService) {
        _excecutionControlService = excecutionControlService;
    }

    public void NotFound(string message) {
        _excecutionControlService.StopExcecution(new CustomResponseData() {
            StatusCode = 404,
            Body = new { message = message }
        });
    }

    public void Unauthorized() {
        _excecutionControlService.StopExcecution(new CustomResponseData() {
            StatusCode = 401,
        });
    }

    // Etc
}
```

If there is need for a default one i might make a template.
