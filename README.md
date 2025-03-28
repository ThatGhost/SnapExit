# Snap exit 3.0.0
[![NuGet](https://img.shields.io/nuget/v/SnapExit.svg)](https://www.nuget.org/packages/SnapExit/)
[![NuGet](https://img.shields.io/nuget/dt/SnapExit.svg)](https://www.nuget.org/packages/SnapExit/)

A nuget package that allows for exception-like behavior to validate state in any ASP.NET project, but with all the performance benefits of cancellation tokens.

Any feedback is helpful. Please leave it in the Issues section.
3.0 aims for a stable release with a few new features and a lot of bug fixes. From now on the package will be safelly maintained and updated with new features.

## Performance
The package is meant to replace exceptions but still keeps the performance cost at a minimum.
Still with a dept of 1 (worst case scenario). The performance is still **x10** over regular exception.
Most of the lost performance in this benchmarks is because of the one time cost of initialization. When u negate for that, the loss is even smaller (-200ns).

| Exceptions | SnapExit | HappyPath | SnapExit HappyPath |
|------------|----------|-----------|--------------------|
| 11_400ns   | 1300ns   | 700ns     | 1100ns             |


When u increase the stack dept the performance of exceptions becomes worse. So with a 100 deep stack these are the benchmarking results.
As you can see with a deep stack debt the performance increase can be up to **x60**.

| Exceptions | SnapExit | HappyPath | SnapExit HappyPath |
|------------|----------|-----------|--------------------|
| 460μs      | 5μs      | 1,3μs     | 1,3μs              |

## Usage

To throw a SnapExit it is as simple as using the static Snap class

**Do not forget to await the `Snap.Exit()`. if you dont the next lines might be executed**
```csharp
    // Old way
    throw new Exception(new {
        // pass data to the try catch block
    });

    // New way
    await Snap.Exit(new {
        // any response data defined in the ExitManager can be passed here
    });
```

And catching it also super simple!

```csharp
    // Old way
    Try{
        await SomeTask();
    } Catch(Exception e) {
        // catch code
    }

    // New way
    SetupSnapExit(SomeTask(), (response) => {
        // catch code
    });
```

To make this work you will need to define a return point for SnapExit. This is made easy by using the ExitManager class!
```csharp
  public class SnapExitReturnPoint : ExitManager<ResponseData> // can be inherited from or instantiated
  {
      public async Task ThisCanBeAnyPointOfCode() {
          SetupSnapExit(task, (response) => { // response is of type ResponseData
            // any catch code goes here
          });
      }
  }
```

Also look at the example project to see how you can turn SnapExit into a blazingly fast middleware for ASP.NET Core api!