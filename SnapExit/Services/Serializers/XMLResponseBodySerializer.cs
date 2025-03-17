using SnapExit.Interfaces;
using System.Xml.Serialization;

namespace SnapExit.Services.Serializers
{
    public class XMLResponseBodySerializer : IResponseBodySerializer
    {
        public string ContentType { get => "application/xml"; }

        public string GetBody(object body)
        {
            Type type = body.GetType();
            var serializer = new XmlSerializer(type);
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, body);
                return textWriter.ToString();
            }
        }
    }
}
