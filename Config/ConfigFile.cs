using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;


using static System.Environment;

namespace ActLikeAI.Config
{
    public class ConfigFile
    {
        /// <summary>
        /// Initializes a new instance of ConfigFile class.
        /// </summary>
        /// <param name="fileName">Location of the definition file.</param>
        /// <param name="configProvider">Config file format provider.</param>
        /// <param name="formatProvider">Provides formating information for the conversion is string values to built-in types.</param>
        public ConfigFile(string fileName, IConfigProvider configProvider, IFormatProvider formatProvider) 
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Please specify the config definition file.");

            if (configProvider is null)
                throw new ArgumentNullException(nameof(configProvider));

            if (formatProvider is null)
                throw new ArgumentNullException(nameof(formatProvider));

            if (IsPathAbsolute(fileName))
                definitionFile = fileName;
            else
            {
                string callerDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                definitionFile = Path.GetFullPath(Path.Combine(callerDirectory, fileName));
            }

            if (!File.Exists(definitionFile))
                throw new ArgumentException($"Can't find the definition file: {definitionFile}.");

            this.configProvider = configProvider;
            root = configProvider.Load(definitionFile);

            this.formatProvider = formatProvider;
        }


        /// <summary>
        /// Initializes a new instance of ConfigFile class.
        /// </summary>
        /// <param name="fileName">Location of the definition file.</param>
        /// <param name="configProvider">Config file format provider.</param>
        public ConfigFile(string fileName, IConfigProvider configProvider) :
            this(fileName, configProvider, CultureInfo.InvariantCulture)
        { }



        /// <summary>
        /// Gets the key of the root node.
        /// </summary>
        public string RootKey => root.Key;



        /// <summary>
        /// Adds a location for config file overlay.
        /// </summary>
        /// <param name="directory">Config file overlay location.</param>
        /// <param name="save">If true, location is marked as save location.</param>
        /// <returns>Current instance of the ConfigFile.</returns>
        public ConfigFile AddLocation(string directory, bool save = false)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException("Please specify the target directory.");

            string location = Path.Combine(directory, Path.GetFileName(definitionFile));
            
            if (location != definitionFile && File.Exists(location))
                UpdateNode(root, configProvider.Load(location));

            if (save)             
                saveLocation = location;

            return this;
        }


        /// <summary>
        /// Adds the current directory to the list of config file overlays. It's always read-only.
        /// </summary>      
        /// <returns>Current instance of the ConfigFile.</returns>
        public ConfigFile AddCurrentDirectory()
            => AddLocation(Directory.GetCurrentDirectory(), false);


        /// <summary>
        /// Adds path relative to user's application data directory to the list of config file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's application data directory.</param>
        /// <param name="save">If true, location is marked as save location.</param>
        /// <returns>Current instance of the ConfigFile.</returns>
        public ConfigFile AddAppData(string relativePath, bool save = false)
        {
            if (relativePath is null)
                throw new ArgumentNullException(nameof(relativePath));

            if (IsPathAbsolute(relativePath))
                throw new ArgumentException($"{relativePath} must be relative. With absolute paths use AddLocation instead.");
            else
                return AddLocation(Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), relativePath), save);
        }


        /// <summary>
        /// Adds path relative to user's local application data directory to the list of config file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's local application data directory.</param>
        /// <param name="save">If true, location is marked as save location.</param>
        /// <returns>Current instance of the ConfigFile.</returns>
        public ConfigFile AddLocalAppData(string relativePath, bool save = false)
        {
            if (relativePath is null)
                throw new ArgumentNullException(nameof(relativePath));

            if (IsPathAbsolute(relativePath))
                throw new ArgumentException($"{relativePath} must be relative. With absolute paths use AddLocation instead.");
            else
                return AddLocation(Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData), relativePath), save);
        }


        /// <summary>
        /// Adds path relative to common application data directory to the list of config file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to common application data directory.</param>
        /// <param name="save">If true, location is marked as save location.</param>
        /// <returns>Current instance of the ConfigFile.</returns>
        public ConfigFile AddCommonAppData(string relativePath, bool save = false)
        {
            if (relativePath is null)
                throw new ArgumentNullException(nameof(relativePath));

            if (IsPathAbsolute(relativePath))
                throw new ArgumentException($"{relativePath} must be relative. With absolute paths use AddLocation instead.");
            else
                return AddLocation(Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), relativePath), save);
        }


        /// <summary>
        /// Adds path relative to user's home directory to the list of config file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's home directory.</param>
        /// <param name="save">If true, location is marked as save location.</param>
        /// <returns>Current instance of the ConfigFile.</returns>
        public ConfigFile AddUserHome(string relativePath, bool save = false)
        {
            if (relativePath is null)
                throw new ArgumentNullException(nameof(relativePath));

            if (IsPathAbsolute(relativePath))
                throw new ArgumentException($"{relativePath} must be relative. With absolute paths use AddLocation instead.");
            else
                return AddLocation(Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), relativePath), save);
        }


        /// <summary>
        /// Adds path relative to user's documents directory to the list of config file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's documents directory.</param>
        /// <param name="save">If true, location is marked as save location.</param>
        /// <returns>Current instance of the ConfigFile.</returns>
        public ConfigFile AddMyDocuments(string relativePath, bool save = false)
        {
            if (relativePath is null)
                throw new ArgumentNullException(nameof(relativePath));

            if (IsPathAbsolute(relativePath))
                throw new ArgumentException($"{relativePath} must be relative. With absolute paths use AddLocation instead.");
            else
                return AddLocation(Path.Combine(GetFolderPath(SpecialFolder.Personal), relativePath), save);
        }


        /// <summary>
        /// Saves the changes to the save location.
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrEmpty(saveLocation) && changed)                            
                configProvider.Save(root, saveLocation);            
        }


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>Value associated with the specified key.</returns>
        public string Get(string key)
            => GetKeyValuePair(key).Value;

       
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>Value associated with the specified key converted to type T.</returns>
        public T Get<T>(string key) 
            => (T)Convert.ChangeType(Get(key), typeof(T), formatProvider);


        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">Value of the specified key.</param>
        public void Set(string key, string value)            
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            GetKeyValuePair(key).Update(value);
            changed = true;
        }


        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">Value of the specified key.</param>
        public void Set<T>(string key, T value)
            => Set(key, string.Format(formatProvider, "{0}", value));


        /// <summary>
        /// Key separator.
        /// </summary>
        public static char Separator { get; set; } = '.';


        /// <summary>
        /// Gets the key/value pair that corresponds to the specified key.
        /// </summary>
        /// <param name="key">The key of the key/value pair to find.</param>
        /// <returns>IConfigKeyValuePair instance that corresponds to the specified key.</returns>
        private IConfigKeyValuePair GetKeyValuePair(string key)
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
                        return attribute;
                    else
                        throw new ArgumentException($"Attribute {tokens[i - 1]} not found.");
                }
                // 2. Token wasn't matched somewhere before the end of the array,
                //    which means that requested node is not present.
                else
                    throw new ArgumentException($"Node {tokens[i]} not found.");
            }
            else
                return current;
        }


        /// <summary>
        /// Updates the config tree with values from another (partial) tree.
        /// </summary>
        /// <param name="node">Root of the tree to be updated.</param>
        /// <param name="update">Root of the tree that contains new values.</param>
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


        /// <summary>
        /// Checks whether the supplied path is absolute.
        /// </summary>
        /// <param name="path">Path to be checked.</param>
        /// <returns>True if path is absolute, false otherwise.</returns>
        private static bool IsPathAbsolute(string path) 
            => (Path.GetFullPath(path) == path);


        private string definitionFile;
        private string saveLocation;
        
        private ConfigNode root;
        private bool changed = false;        
        private IConfigProvider configProvider;
        private IFormatProvider formatProvider;
    }
}
