using SnapExit.Interfaces;
using System.Text.Json;

namespace SnapExit.Services.Serializers
{
    public class JSONResponseBodySerializer : IResponseBodySerializer
    {
        public string ContentType { get => "application/json"; }

        public string GetBody(object body)
        {
            return JsonSerializer.Serialize(body);
        }
    }
}
