using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ActLikeAI.Config
{
    /// <summary>
    /// Represents a class that manages a collection of configuration files.
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// Adds a configuration file to the manager.
        /// </summary>
        /// <param name="file">Configuration file to add.</param>
        /// <returns>This instance of the ConfigManager.</returns>
        public ConfigManager Add(ConfigFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            files.Add(file.RootKey, file);
            return this;
        }


        /// <summary>
        /// Saves changed configuration files.
        /// </summary>
        public void Save()
        {
            foreach (var file in files.Values)
                file.Save();
        }


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>Value associated with the specified key.</returns>
        public string Get(string key)
        {
            var split = SplitKey(key);
            return split.File.Get(split.Key);
        }


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>Value associated with the specified key converted to type <typeparamref name="T"/>.</returns>
        public T Get<T>(string key)
        {
            var split = SplitKey(key);
            return split.File.Get<T>(split.Key);
        }


        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">Value of the specified key.</param>
        public void Set(string key, string value)
        {
            var split = SplitKey(key);
            split.File.Set(split.Key, value);
        }


        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">Value of the specified key.</param>
        public void Set<T>(string key, T value)
        {
            var split = SplitKey(key);
            split.File.Set(split.Key, value);
        }
    

        private (ConfigFile File, string Key) SplitKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"{nameof(key)} can't be null or empty.");

            int pos = key.IndexOf(ConfigFile.Separator);
            if (pos > 0)
            {
                string filePart = key.Substring(0, pos);
                string keyPart = key.Substring(pos + 1);

                if (files.Count == 1 && !files.ContainsKey(filePart))
                    return (files.Values.First(), key);
                else
                    return (files[filePart], keyPart);
            }
            else
            
                if (files.Count == 1)
                    return (files.Values.First(), key);
                else
                    throw new ArgumentException("Please provide a fully qualified key.");
        }


        private Dictionary<string, ConfigFile> files = new Dictionary<string, ConfigFile>();
    }
}
