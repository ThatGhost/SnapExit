namespace SnapExit.Entities
{
    public class SnapExitOptions<T> where T : class
    {
        public T? DefaultValue { get; set; }
    }
}
