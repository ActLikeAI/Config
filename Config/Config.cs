using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;

namespace ActLikeAI.Config
{
    /// <summary>
    /// 
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Loads XML definition file and specifies save directory relative user's AppData.
        /// </summary>
        /// <param name="definitionFile">Definition file name.</param>
        /// <param name="saveDirectory">Folder relative to user's AddData.</param>
        public static void Load(string definitionFile, string saveDirectory)
        {
            if (manager is null)
                manager = new ConfigManager();

            var file = new ConfigFile(definitionFile, new XmlConfigProvider())
                .AddAppData(saveDirectory, true)
                .AddCurrentDirectory();

            manager.Add(file);
        }


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
        /// <returns>Value associated with the specified key converted to type T.</returns>
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(string key, T value)
            => manager.Set<T>(key, value);


        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">Value of the specified key.</param>
        private static ConfigManager manager;
    }
}
