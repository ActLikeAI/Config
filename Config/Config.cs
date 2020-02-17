using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;

namespace ActLikeAI.Config
{
    public static class Config
    {
        public static char Separator { get; private set; }
 

        public static void Load(string defaultsFile, string userDirectory, char separator = '.')
        {                       
            if (string.IsNullOrEmpty(defaultsFile) || string.IsNullOrEmpty(userDirectory))
                throw new ArgumentException($"Please specify both {nameof(defaultsFile)} and {nameof(userDirectory)}.");

            Separator = separator;
            
            if (IsPathAbsolute(defaultsFile))
                defaultsLocation = defaultsFile;
            else
            {
                string callerDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                defaultsLocation = Path.GetFullPath(Path.Combine(callerDirectory, defaultsFile));
            }

            if (IsPathAbsolute(userDirectory))
                userLocation = Path.Combine(userDirectory, Path.GetFileName(defaultsFile));
            else
                userLocation = Path.Combine(Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), userDirectory)), Path.GetFileName(defaultsFile));

            currentLocation = Path.Combine(Directory.GetCurrentDirectory(), defaultsFile);

            if (!File.Exists(defaultsLocation))
                throw new ArgumentException($"Can't find \"{defaultsLocation}\". Please check if it's located where you expect it.");

            XElement rootXml = XElement.Load(defaultsLocation);
            root = ParseXml(rootXml, null);

            if (File.Exists(userLocation))
            {
                var userXml = XElement.Load(userLocation);
                var user = ParseXml(userXml, null);
                UpdateNode(root, user);
            }

            if (currentLocation != defaultsLocation && File.Exists(currentLocation))
            { 
                var currentXml = XElement.Load(currentLocation);
                var current = ParseXml(currentXml, null);
                UpdateNode(root, current);
            }
        }


        public static void Save()
        {
            if (configChanged)
            {                
                var xmlRoot = File.Exists(userLocation) ? XElement.Load(userLocation) : new XElement(root.Name);
                UpdateXml(xmlRoot, root);

                string directory = Path.GetDirectoryName(userLocation);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                xmlRoot.Save(userLocation);
            }        
        }


        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"{nameof(key)} can't be null or empty.");

            string[] tokens = key.Split(new char[] { Separator });                                                
            
            // Root node is optional. If specified, we simply start counting from the next member of the array. 
            int start = (tokens[0] == root.Name) ? 1 : 0;

            var current = root;
            var previous = root;
            int i = start;
            do
            {
                previous = current;
                current = current.Children.Find(node => node.Name == tokens[i]);
                i++;
            }
            while (i < tokens.Length && current != null);

            // This can happen in two scenarios:
            if (current is null) 
            {
                // 1. We reached then end of the array and the last token wasn't matched,
                //    so we try to see whether it's an attribute. If yes, we return its value.
                if (i == tokens.Length)
                {
                    var attribute = previous.Attributes.Find(attr => attr.Name == tokens[i - 1]);
                    if (attribute != null)
                        return attribute.Value;
                    else
                        throw new ArgumentException($"Attribute {tokens[i - 1]} not found.");
                }
                // 2. Token wasn't matched somewhere before the end of the array,
                //    which means that requested node is not present.
                else
                    throw new ArgumentException($"Node {tokens[i]} not found.");
            }
            else
                return current.Value;
        }


        public static T Get<T>(string key, IFormatProvider formatProvider)
        {
            return (T)Convert.ChangeType(Get(key), typeof(T), formatProvider);
        }


        public static T Get<T>(string key)
            => Get<T>(key, CultureInfo.CurrentCulture);
        

        public static void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"{nameof(key)} can't be null or empty.");

            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} can't be null.");

            string[] tokens = key.Split(new char[] { Separator });

            // Root node is optional. If specified, we simply start counting from the next member of the array. 
            int start = (tokens[0] == root.Name) ? 1 : 0;

            var current = root;
            var previous = root;
            int i = start;
            do
            {
                previous = current;
                current = current.Children.Find(node => node.Name == tokens[i]);
                i++;
            }
            while (i < tokens.Length && current != null);

            // This can happen in two scenarios:
            if (current is null)
            {
                // 1. We reached then end of the array and the last token wasn't matched,
                //    so we try to see whether it's an attribute. If yes, we return its value.
                if (i == tokens.Length)
                {
                    var attribute = previous.Attributes.Find(attr => attr.Name == tokens[i - 1]);
                    if (attribute != null)
                        attribute.Update(value);
                    else
                        throw new ArgumentException($"Attribute {tokens[i - 1]} not found.");
                }
                // 2. Token wasn't matched somewhere before the end of the array,
                //    which means that requested node is not present.
                else
                    throw new ArgumentException($"Node {tokens[i]} not found.");
            }
            else
                current.Update(value);

            configChanged = true;
        }


        public static void Set<T>(string key, T value, IFormatProvider formatProvider) 
            => Set(key, string.Format(formatProvider, "{0}", value));


        public static void Set<T>(string key, T value)
            => Set<T>(key, value, CultureInfo.CurrentCulture);


        private static bool IsPathAbsolute(string path) => (Path.GetFullPath(path) == path);


        private static ConfigNode ParseXml(XElement element, ConfigNode parent)
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


        private static void UpdateXml(XElement element, ConfigNode node)
        {
            if (node.Changed)
            {
                if (node.Value.Length > 0)
                    element.Value = node.Value;

                foreach (var attribute in node.Attributes)
                {
                    if (attribute.Changed)
                    {
                        var xmlAttribute = element.Attribute(attribute.Name);
                        if (xmlAttribute != null)
                            xmlAttribute.Value = attribute.Value;
                        else
                            element.Add(new XAttribute(attribute.Name, attribute.Value));
                    }
                }

                foreach (var child in node.Children)
                {
                    if (child.Changed)
                    {
                        var xmlChild = element.Element(child.Name);
                        if (xmlChild == null)
                        {
                            xmlChild = new XElement(child.Name);
                            element.Add(xmlChild);
                        }
                        UpdateXml(xmlChild, child);
                    }
                }
            }
        }


        private static void UpdateNode(ConfigNode node, ConfigNode update)
        {            
            if (node.Value != update.Value)
                node.Update(update.Value, false);

            foreach (var updateAttribute in update.Attributes)
            {
                var nodeAttribute = node.Attributes.Find(a => a.Name == updateAttribute.Name);
                if (nodeAttribute != null && nodeAttribute.Value != updateAttribute.Value)
                    nodeAttribute.Update(updateAttribute.Value, false);
            }

            foreach (var updateNode in update.Children)
            {
                var childNode = node.Children.Find(n => n.Name == updateNode.Name);
                if (childNode != null)
                    UpdateNode(childNode, updateNode);
            }      
        }


        private static ConfigNode root;
        private static string defaultsLocation;
        private static string userLocation;
        private static string currentLocation;

        private static bool configChanged = false;        
    }
}
