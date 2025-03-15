using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapExit.Entities
{
    public class CustomResponseData
    {
        public int StatusCode { get; set; }
        public object? Body { get; set; }
        public IDictionary<string, string>? Headers { get; set; }
    }
}
