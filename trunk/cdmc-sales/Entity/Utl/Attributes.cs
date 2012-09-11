using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class JsonIgnoreAttribute : Attribute
    {
        public List<string> IgnoreProperties { get; set; }
        public JsonIgnoreAttribute(params string[] properties)
        {
            IgnoreProperties = new List<string>();
            IgnoreProperties.AddRange(properties);
        }
    }
}
