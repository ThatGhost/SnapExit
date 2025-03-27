namespace SnapExit.Example.Services.Interfaces;

public interface IAssertionService
{
    public Task NotFound();
    public Task Teapot(string message);
    public Task Forbidden(string message, string token);
}
