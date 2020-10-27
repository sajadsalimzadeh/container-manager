using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class YamlAttribute : Attribute
    {
        public string Key { get; set; }
        public bool WithQuotes { get; set; }
        public bool IsIgnore { get; set; }

        public YamlAttribute()
        {

        }
        public YamlAttribute(string key)
        {
            Key = key;
        }
    }
}
