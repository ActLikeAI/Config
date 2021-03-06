using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;


using static System.Environment;

namespace ActLikeAI.Config
{
    /// <summary>
    /// Represents single configuration file.
    /// </summary>
    public class ConfigFile
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConfigFile"/> class.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="configProvider">Configuration file format provider.</param>
        /// <param name="cultureInfo"><see cref="CultureInfo"/> instance to use for conversion between strings and build-in types.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public ConfigFile(string definitionFile, IConfigProvider configProvider, CultureInfo cultureInfo, bool ignoreCase = true) 
        {
            if (string.IsNullOrEmpty(definitionFile))
                throw new ArgumentException("Please specify the configuration definition file.");

            if (configProvider is null)
                throw new ArgumentNullException(nameof(configProvider));

            if (cultureInfo is null)
                throw new ArgumentNullException(nameof(cultureInfo));

            if (IsPathAbsolute(definitionFile))
                this.definitionFile = definitionFile;
            else
            {
                string callerDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                this.definitionFile = Path.GetFullPath(Path.Combine(callerDirectory, definitionFile));
            }

            if (!File.Exists(this.definitionFile))
                throw new ArgumentException($"Can't find the definition file: {this.definitionFile}.");

            this.configProvider = configProvider;
            root = configProvider.Load(this.definitionFile);

            this.cultureInfo = cultureInfo;
            this.ignoreCase = ignoreCase;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ConfigFile"/> class.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="configProvider">Configuration file format provider.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public ConfigFile(string definitionFile, IConfigProvider configProvider, bool ignoreCase = true) :
            this(definitionFile, configProvider, CultureInfo.InvariantCulture, ignoreCase)
        { }


        /// <summary>
        /// Initializes a new instance of ConfigFile class.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="cultureInfo">CultureInfo instance to use for conversion between strings and build-in types.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public ConfigFile(string definitionFile, CultureInfo cultureInfo, bool ignoreCase = true) :
            this(definitionFile, new XmlConfigProvider(), cultureInfo, ignoreCase)
        { }


        /// <summary>
        /// Initializes a new instance of ConfigFile class.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public ConfigFile(string definitionFile, bool ignoreCase = true) :
            this(definitionFile, new XmlConfigProvider(), CultureInfo.InvariantCulture, ignoreCase)
        { }


        /// <summary>
        /// Gets the key of the root node.
        /// </summary>
        public string RootKey => root.Key;


        /// <summary>
        /// Key separator.
        /// </summary>
        public static char Separator => '.';


        /// <summary>
        /// Adds a location for configuration file overlay.
        /// </summary>
        /// <param name="directory">Configuration file overlay location.</param>
        /// <param name="save">If <see langword="true"/>, location is marked as save location; otherwise, it's read-only.</param>
        /// <returns>Current instance of the <see cref="ConfigFile"/>.</returns>
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
        /// Adds the current directory to the list of configuration file overlays. It's always read-only.
        /// </summary>      
        /// <returns>Current instance of the <see cref="ConfigFile"/>.</returns>
        public ConfigFile AddCurrentDirectory()
            => AddLocation(Directory.GetCurrentDirectory(), false);


        /// <summary>
        /// Adds path relative to user's application data directory to the list of configuration file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's application data directory.</param>
        /// <param name="save">If <see langword="true"/>, location is marked as save location; otherwise, it's read-only.</param>
        /// <returns>Current instance of the <see cref="ConfigFile"/>.</returns>
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
        /// Adds path relative to user's local application data directory to the list of configuration file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's local application data directory.</param>
        /// <param name="save">If <see langword="true"/>, location is marked as save location; otherwise, it's read-only.</param>
        /// <returns>Current instance of the <see cref="ConfigFile"/>.</returns>
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
        /// Adds path relative to common application data directory to the list of configuration file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to common application data directory.</param>
        /// <param name="save">If <see langword="true"/>, location is marked as save location; otherwise, it's read-only.</param>
        /// <returns>Current instance of the <see cref="ConfigFile"/>.</returns>
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
        /// Adds path relative to user's home directory to the list of configuration file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's home directory.</param>
        /// <param name="save">If <see langword="true"/>, location is marked as save location; otherwise, it's read-only.</param>
        /// <returns>Current instance of the <see cref="ConfigFile"/>.</returns>
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
        /// Adds path relative to user's documents directory to the list of configuration file overlays.
        /// </summary>
        /// <param name="relativePath">Path relative to user's documents directory.</param>
        /// <param name="save">If <see langword="true"/>, location is marked as save location; otherwise, it's read-only.</param>
        /// <returns>Current instance of the <see cref="ConfigFile"/>.</returns>
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
        /// Saves changed values to the save location.
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
        /// <returns>Value associated with the specified key converted to type <typeparamref name="T"/>.</returns>
        public T Get<T>(string key) 
            => (T)Convert.ChangeType(Get(key), typeof(T), cultureInfo);


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
            => Set(key, string.Format(cultureInfo, "{0}", value));

       
        /// <summary>
        /// Gets the key/value pair that corresponds to the specified key.
        /// </summary>
        /// <param name="key">The key of the key/value pair to find.</param>
        /// <returns><see cref="IConfigKeyValuePair"/> instance that corresponds to the specified key.</returns>
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
                current = current.Children.Find(node => string.Compare(node.Key,tokens[i], ignoreCase, cultureInfo) == 0);
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
                    var attribute = previous.Attributes.Find(attr => string.Compare(attr.Key, tokens[i - 1], ignoreCase, cultureInfo) == 0);
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
        /// Updates the configuration tree with values from another (partial) tree.
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
        /// <returns><see langword="true"/> if path is absolute; otherwise <see langword="false"/>.</returns>
        private static bool IsPathAbsolute(string path) 
            => (Path.GetFullPath(path) == path);


        private string definitionFile;
        private string saveLocation;
        
        private ConfigNode root;
        private bool changed = false;        
        private IConfigProvider configProvider;
        private CultureInfo cultureInfo;
        private bool ignoreCase;
    }
}
