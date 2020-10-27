using Chargoon.ContainerManagement.Domain.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chargoon.ContainerManagement.Service.Utilities
{
    public class YamlHelper
    {
        enum NodeType
        {
            None = 0,
            Array = 1,
            Object = 2,
        }
        class Node
        {
            public string Key { get; set; }
            public object Value { get; set; }
            public string Prefix { get; set; }
            public NodeType Type { get; set; } = NodeType.None;
            public bool WithQuotes { get; set; }

            private string RepeatString(string str, int count)
            {
                var tmp = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    tmp += str;
                }
                return tmp;
            }

            private string ToYaml(int indent)
            {
                var type = Value.GetType();

                if (typeof(int).IsAssignableFrom(type) || typeof(string).IsAssignableFrom(type))
                {
                    return RepeatString("  ", indent) + Prefix +
                        (string.IsNullOrEmpty(Key) ? "- " : Key.ToLower() + ": ") +
                        (WithQuotes ? $"'{Value}'" : Value.ToString());
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    var flag = false;
                    var sb = new StringBuilder();
                    if (Value is IEnumerable enumerable)
                    {
                        if (!string.IsNullOrEmpty(Key)) sb.AppendLine(RepeatString("  ", indent) + Prefix + Key.ToLower() + ":");
                        foreach (var item in enumerable)
                        {
                            if (item is Node childNode)
                            {
                                var line = childNode.ToYaml(indent + 1);
                                sb.AppendLine(line);
                            }
                            flag = true;
                        }
                    }
                    if (flag) return sb.ToString();
                    return "";
                }
                return base.ToString();
            }
            public string ToYaml()
            {
                return ToYaml(-1);
            }
        }

        private Node Root;

        public YamlHelper(object obj)
        {
            Root = Parse(obj);
        }

        private Node Parse(object obj)
        {
            if (obj == null) return null;
            var result = new Node();
            var type = obj.GetType();
            if (typeof(int).IsAssignableFrom(type))
            {
                result.Value = Convert.ToInt32(obj);
            }
            else if (typeof(string).IsAssignableFrom(type))
            {
                result.Value = obj.ToString();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (obj is IEnumerable enumerable)
                {
                    var children = new List<Node>();
                    foreach (var item in enumerable)
                    {
                        var keyProp = item.GetType().GetProperty("Key");
                        var valueProp = item.GetType().GetProperty("Value");
                        if (keyProp != null && valueProp != null)
                        {
                            var childNode = Parse(valueProp.GetValue(item));
                            if (childNode != null)
                            {
                                childNode.Key = keyProp.GetValue(item).ToString();
                                children.Add(childNode);
                            }
                        }
                        else
                        {
                            var childNode = Parse(item);
                            if (childNode != null)
                            {
                                if (childNode.Type == NodeType.Object)
                                {
                                    if (childNode.Value is IEnumerable childEnumerable)
                                    {
                                        var childIndex = 0;
                                        foreach (var childValue in childEnumerable)
                                        {
                                            if (childValue is Node childValueNode)
                                            {
                                                childValueNode.Prefix = (childIndex == 0 ? "- " : "  ");
                                            }
                                            childIndex++;
                                        }
                                    }
                                }
                                children.Add(childNode);
                            }
                        }
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
                    if (node != null)
                    {
                        node.Key = property.Name;
                        var attr = property.GetCustomAttribute<YamlAttribute>();
                        if (attr != null)
                        {
                            if (attr.IsIgnore) continue;
                            if (!string.IsNullOrEmpty(attr.Key)) node.Key = attr.Key;
                            node.WithQuotes = attr.WithQuotes;
                        }
                        children.Add(node);
                    }
                }
                result.Type = NodeType.Object;
                result.Value = children;
            }
            return result;
        }
        public string ToYaml()
        {
            return Root.ToYaml();
        }
    }
}