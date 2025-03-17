using SnapExit.Services.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapExit.Entities
{
    public class SnapExitOptions
    {
        public int DefaultStatusCode { get; set; } = 500;
        public object DefaultBody { get; set; } = "Internal Server Error";
        public IDictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();
        public Type ResponseSerializer { get; set; } = typeof(JSONResponseBodySerializer);
        public Func<object, string>? CustomResponseSerializer { get; set; } = null;
    }
}
