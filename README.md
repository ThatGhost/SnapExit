# Snap exit 3.0.0
[![NuGet](https://img.shields.io/nuget/v/SnapExit.svg)](https://www.nuget.org/packages/SnapExit/)
[![NuGet](https://img.shields.io/nuget/dt/SnapExit.svg)](https://www.nuget.org/packages/SnapExit/)
[![Build Status](https://dev.azure.com/robertsundstrom/SnapExit/_apis/build/status/robertsundstrom.SnapExit?branchName=master)](https://dev.azure.com/robertsundstrom/SnapExit/_build/latest?definitionId=1&branchName=master)

A nuget package that allows for exception-like behavior to validate state in any ASP.NET project, but with all the performance benefits of cancellation tokens.

Any feedback is helpful. Please leave it in the Issues section.
3.0 aims for a stable release with a few new features and a lot of bug fixes. From now on the package will be safelly maintained and updated with new features.

## Performance
The package is meant to replace exceptions but still keeps the performance cost at a minimum.
Still with a dept of 1 (worst case scenario). The performance is still **x10** over regular exception.
Most of the lost performance in this benchmarks is because of the one time cost of initialization. When u negate for that, the loss is even smaller (-200ns).

| Exceptions | SnapExit | HappyPath | SnapExit HappyPath |
|------------|----------|-----------|--------------------|
| 11_000ns   | 1300ns   | 640ns     | 1100ns             |


When u increase the stack dept the performance of exceptions becomes worse. So with a 100 deep stack these are the benchmarking results.
As you can see with a deep stack debt the performance increase can be up to **x60**.

| Exceptions | SnapExit | HappyPath | SnapExit HappyPath |
|------------|----------|-----------|--------------------|
| 450μs      | 5μs      | 1,3μs     | 1,3μs              |

## Usage

To throw a SnapExit it is as simple as using the static Snap class
**Do not forget to await the `Snap.Exit()`. if you dont the next lines might be executed**
```csharp
  public class YourService {

      // New way
      public async Task ThisShouldThrow() {
          await Snap.Exit();
      }

      // Old way
      public Task ThisShouldThrow() {
          throw new Exception();
      }
  }
```

This halts the flow of task immediatly, you can even return custom defined data to the return point:
```csharp
  Snap.Exit(new {
      // any response data can be passed here
  });
```

To make this work you will need to define a return point for SnapExit. This is made easy by using the SnapExitManager class!
```csharp
  public class SnapExitReturnPoint : SnapExitManager<ResponseData> // can be inherited from or instantiated
  {
      public async Task ThisCanBeAnyPointOfCode() {
          onSnapExit += OnSnapExit; // analog to catch
          Task task = SomeLongTask();
          RegisterSnapExit(task); // analog to try
      }
      private Task OnSnapExit(ResponseData responseData) { // response is data passed at error time
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
    public Task Foo(string message) {
        await Snap.Exit(new {
            message
        });
    }

    public Task Bar() {
        await Snap.Exit(new {
            message = "Something went wrong!"
        });
    }

    // Etc
}
```
