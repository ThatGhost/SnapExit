namespace SnapExit.Example.Entities;

public sealed class CustomResponseData
{
    public int StatusCode { get; set; }
    public object? Body { get; set; }
    public IDictionary<string, string>? Headers { get; set; }
}
