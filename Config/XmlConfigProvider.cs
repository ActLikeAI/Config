using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ActLikeAI.Config
{
    public class XmlConfigProvider : IConfigProvider
    {
        public ConfigNode Load(string file)
        {
            XElement rootXml = XElement.Load(file);
            return ParseXml(rootXml, null);            
        }


        public void Save(ConfigNode root, string saveLocation)
        {
            var xmlRoot = File.Exists(saveLocation) ? XElement.Load(saveLocation) : new XElement(root.Key);
            UpdateXml(xmlRoot, root);

            string directory = Path.GetDirectoryName(saveLocation);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            xmlRoot.Save(saveLocation);
        }


        private ConfigNode ParseXml(XElement element, ConfigNode parent)
        {
            ConfigNode node = new ConfigNode(parent, element.Name.LocalName);
            if (element.HasAttributes)
                foreach (var attribute in element.Attributes())
                    node.Attributes.Add(new ConfigAttribute(node, attribute.Name.LocalName, attribute.Value));

            if (element.HasElements)
                foreach (var child in element.Elements())
                    node.Children.Add(ParseXml(child, node));
            else
                node.Update(element.Value.Trim(), false);

            return node;
        }


        private  void UpdateXml(XElement element, ConfigNode node)
        {
            if (node.Changed)
            {
                if (node.Value.Length > 0)
                    element.Value = node.Value;

                foreach (var attribute in node.Attributes)
                {
                    if (attribute.Changed)
                    {
                        var xmlAttribute = element.Attribute(attribute.Key);
                        if (xmlAttribute != null)
                            xmlAttribute.Value = attribute.Value;
                        else
                            element.Add(new XAttribute(attribute.Key, attribute.Value));
                    }
                }

                foreach (var child in node.Children)
                {
                    if (child.Changed)
                    {
                        var xmlChild = element.Element(child.Key);
                        if (xmlChild == null)
                        {
                            xmlChild = new XElement(child.Key);
                            element.Add(xmlChild);
                        }
                        UpdateXml(xmlChild, child);
                    }
                }
            }
        }
    }
}
