using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class YamlAttribute : Attribute
    {
        public string Key { get; set; }

        public YamlAttribute(string key)
        {
            Key = key;
        }
    }


    public class YamlHelper
    {
        class Node
        {
            public string Key { get; set; }
            public object Value { get; set; }
        }

        private Node Root;

        public YamlHelper(object obj)
        {
            Root = Parse(obj);
        }

        private Node Parse(object obj)
        {
            var result = new Node();
            var type = obj.GetType();
            if (typeof(int).IsAssignableFrom(type))
            {
                result.Value = obj;
            }
            else if (typeof(string).IsAssignableFrom(type))
            {
                result.Value = obj;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (obj is IEnumerable enumerable)
                {
                    var children = new List<Node>();
                    foreach (var item in enumerable)
                    {
                        children.Add(Parse(item));
                    }
                    result.Value = children;
                }
            }
            else
            {
                var children = new List<Node>();
                foreach (var property in obj.GetType().GetProperties())
                {
                    var value = property.GetValue(obj);
                    var node = Parse(value);
                    node.Key = property.Name;
                    var attr = property.GetCustomAttribute<YamlAttribute>();
                    if (attr != null) node.Key = attr.Key;
                    children.Add(node);
                }
                result.Value = children;
            }
            return result;
        }
        private string RepeatChar(char ch, int count)
        {
            var str = string.Empty;
            for (int i = 0; i < count; i++)
            {
                str += ch;
            }
            return str;
        }
        private string ToString(Node node, int level)
        {
            var sb = new StringBuilder();
            var type = node.Value.GetType();
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (node.Value is IEnumerable enumerable)
                {
                    sb.AppendLine($"{node.Key}:");
                    foreach (var item in enumerable)
                    {
                        if (item is Node childNode)
                        {
                            var line = RepeatChar('\t', level) + (string.IsNullOrEmpty(childNode.Key) ? "- " : childNode.Key) + ToString(childNode, level + 1);
                            sb.AppendLine(line);
                        }
                    }
                }
            }
            else
            {
                sb.Append(node.Value.ToString());
            }
            return sb.ToString();
        }
        public override string ToString()
        {
            return ToString(Root, 0);
        }
    }
}