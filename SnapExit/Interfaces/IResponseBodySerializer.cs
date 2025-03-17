namespace SnapExit.Interfaces
{
    public interface IResponseBodySerializer
    {
        public string ContentType { get; }
        public string GetBody(object body);
    }
}
