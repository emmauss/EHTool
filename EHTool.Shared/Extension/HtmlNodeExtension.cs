using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHTool.Shared.Extension
{
    public static class HtmlNodeExtension
    {
        public static IEnumerable<HtmlNode> GetElements(this HtmlNode node, string nodeName) =>
            node.Elements(nodeName);

        public static HtmlNode GetNodeByName(this HtmlNode doc, string name)
        {
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                var item = doc.ChildNodes[i];
                if (item.Name == name)
                {
                    return item;
                }
                else
                {
                    var node = GetNodeByName(item, name);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public static HtmlNode GetNodeByClassName(this HtmlNode doc, string className)
        {
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                var item = doc.ChildNodes[i];
                if (item.Attributes?["class"]?.Value == className)
                {
                    return item;
                }
                else
                {
                    var node = GetNodeByClassName(item, className);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        private static List<HtmlNode> _nodes = new List<HtmlNode>();

        public static IEnumerable<HtmlNode> GetNodesByClassName(this HtmlNode doc, string className)
        {
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                var item = doc.ChildNodes[i];
                if (item.Attributes?["class"]?.Value == className)
                {
                    _nodes.Add(item);
                }
                else
                {
                    _nodes = GetNodesByClassName(item, className).ToList();
                }
            }
            var nodes = _nodes;
            _nodes = new List<HtmlNode>();
            return nodes;
        }

        public static HtmlNode GetNodeById(this HtmlNode doc, string id)
        {
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                var item = doc.ChildNodes[i];
                if (item.Attributes?["id"]?.Value == id)
                {
                    return item;
                }
                else
                {
                    var node = GetNodeById(item, id);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }
    }
}
