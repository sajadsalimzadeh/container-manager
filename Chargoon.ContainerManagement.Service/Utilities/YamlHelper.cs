using System;

namespace Chargoon.ContainerManagement.Service.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class YamlAttribute : Attribute {
        public string Name { get; set; }

        public YamlAttribute(string name)
        {
            Name = name;
        }
    }

    internal class YamlNode {
        public string Name { get; set; }
        public List<YamlNode> Children { get; set; }
    }

    public class YamlHelper
    {
        public static YamlNode ParseObject(object obj)
        {
            var node = new YamlNode();
            foreach (var property in obj.GetType().GetProperties())
            {
                var attr = property.
            }
        }    
    }
}