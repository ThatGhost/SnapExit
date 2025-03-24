namespace SnapExit.Example.Services.Interfaces;

public interface IAssertionService
{
    public void NotFound();
    public void Teapot(string message);
    public void Forbidden(string message, string token);
}
