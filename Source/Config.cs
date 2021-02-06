using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;

namespace ActLikeAI.Config
{
    /// <summary>
    /// Represents a static wrapper around <see cref="ConfigManager"/> class.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Loads an XML definition file and specifies save directory relative to user's application data directory.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="saveDirectory">Folder relative to user's application data directory.</param>
        /// <param name="configProvider">Configuration file format provider.</param> 
        /// <param name="cultureInfo"><see cref="CultureInfo"/> instance to use for conversion between strings and build-in types.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public static void Load(string definitionFile, string saveDirectory, IConfigProvider configProvider, CultureInfo cultureInfo, bool ignoreCase = true)
        {
            if (manager is null)
                manager = new ConfigManager();

            var file = new ConfigFile(definitionFile, configProvider, cultureInfo, ignoreCase)
                .AddAppData(saveDirectory, true);

            manager.Add(file);
        }


        /// <summary>
        /// Loads an XML definition file and specifies save directory relative to user's application data directory.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="saveDirectory">Folder relative to user's application data directory.</param>
        /// <param name="configProvider">Configuration file format provider.</param>         
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public static void Load(string definitionFile, string saveDirectory, IConfigProvider configProvider, bool ignoreCase = true)
            => Load(definitionFile, saveDirectory, configProvider, CultureInfo.InvariantCulture, ignoreCase);


        /// <summary>
        /// Loads an XML definition file and specifies save directory relative to user's application data directory.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="saveDirectory">Folder relative to user's application data directory.</param>
        /// <param name="cultureInfo"><see cref="CultureInfo"/> instance to use for conversion between strings and build-in types.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public static void Load(string definitionFile, string saveDirectory, CultureInfo cultureInfo, bool ignoreCase = true)
            => Load(definitionFile, saveDirectory, new XmlConfigProvider(), cultureInfo, ignoreCase);


        /// <summary>
        /// Loads an XML definition file and specifies save directory relative to user's application data directory.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="saveDirectory">Folder relative to user's application data directory.</param>
        /// <param name="ignoreCase"><see langword="true"/> to ignore case during the comparison; otherwise, <see langword="false"/>.</param>
        public static void Load(string definitionFile, string saveDirectory, bool ignoreCase = true)
            => Load(definitionFile, saveDirectory, new XmlConfigProvider(), CultureInfo.InvariantCulture, ignoreCase);


        /// <summary>
        /// Loads a configuration file.
        /// </summary>
        /// <param name="file">Configuration to load.</param>
        public static void Load(ConfigFile file)
        {
            if (manager is null)
                manager = new ConfigManager();

            manager.Add(file);
        }


        /// <summary>
        /// Saves changed configuration files.
        /// </summary>
        public static void Save() 
            => manager.Save();


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>Value associated with the specified key.</returns>
        public static string Get(string key) 
            => manager.Get(key);


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>Value associated with the specified key converted to type <typeparamref name="T"/>.</returns>
        public static T Get<T>(string key) 
            => manager.Get<T>(key);


        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">Value of the specified key.</param>
        public static void Set(string key, string value) 
            => manager.Set(key, value);


        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">Value of the specified key.</param>
        public static void Set<T>(string key, T value)
            => manager.Set<T>(key, value);


        private static ConfigManager manager;
    }
}
