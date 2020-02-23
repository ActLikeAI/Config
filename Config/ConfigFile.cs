using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace ActLikeAI.Config
{
    public class ConfigFile
    {
        public ConfigFile(string fileName, IConfigProvider provider)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Please specify the config definition file.");

            if (provider is null)
                throw new ArgumentNullException("ConfigProvider can't be null.");            
            
            if (IsPathAbsolute(fileName))
                definitionFile = fileName;
            else
            {
                string callerDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                definitionFile = Path.GetFullPath(Path.Combine(callerDirectory, fileName));
            }

            if (!File.Exists(definitionFile))
                throw new ArgumentException($"Can't find the definition file: {definitionFile}.");

            configProvider = provider;
            root = configProvider.Load(definitionFile);
        }


        public void AddLocation(string directory, bool save = false)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException("Please specify the target directory.");

            string location = Path.Combine(directory, Path.GetFileName(definitionFile));

            if (location != definitionFile && File.Exists(location))
                UpdateNode(root, configProvider.Load(location));

            if (save)
                saveLocation = location;
        }


        public void Save()
        {
            if (changed)                            
                configProvider.Save(root, saveLocation);            
        }


        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"{nameof(key)} can't be null or empty.");

            string[] tokens = key.Split(new char[] { Separator });
           
            var current = root;
            var previous = root;
            int i = 0;
            do
            {
                previous = current;
                current = current.Children.Find(node => node.Key == tokens[i]);
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
                    var attribute = previous.Attributes.Find(attr => attr.Key == tokens[i - 1]);
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


        public T Get<T>(string key, IFormatProvider formatProvider) 
            => (T)Convert.ChangeType(Get(key), typeof(T), formatProvider);


        public T Get<T>(string key)
            => Get<T>(key, CultureInfo.CurrentCulture);


        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"{nameof(key)} can't be null or empty.");

            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} can't be null.");

            string[] tokens = key.Split(new char[] { Separator });

            var current = root;
            var previous = root;
            int i = 0;
            do
            {
                previous = current;
                current = current.Children.Find(node => node.Key == tokens[i]);
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
                    var attribute = previous.Attributes.Find(attr => attr.Key == tokens[i - 1]);
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

            changed = true;
        }


        public void Set<T>(string key, T value, IFormatProvider formatProvider)
            => Set(key, string.Format(formatProvider, "{0}", value));


        public void Set<T>(string key, T value)
            => Set<T>(key, value, CultureInfo.CurrentCulture);


        private void UpdateNode(ConfigNode node, ConfigNode update)
        {
            if (node.Value != update.Value)
                node.Update(update.Value, false);

            foreach (var updateAttribute in update.Attributes)
            {
                var nodeAttribute = node.Attributes.Find(a => a.Key == updateAttribute.Key);
                if (nodeAttribute != null && nodeAttribute.Value != updateAttribute.Value)
                    nodeAttribute.Update(updateAttribute.Value, false);
            }

            foreach (var updateNode in update.Children)
            {
                var childNode = node.Children.Find(n => n.Key == updateNode.Key);
                if (childNode != null)
                    UpdateNode(childNode, updateNode);
            }
        }


        private bool IsPathAbsolute(string path) 
            => (Path.GetFullPath(path) == path);


        private string definitionFile;
        private string saveLocation;
        
        private ConfigNode root;
        private bool changed = false;
        private readonly IConfigProvider configProvider;

        public static char Separator { get; set; } = '.';
    }
}
