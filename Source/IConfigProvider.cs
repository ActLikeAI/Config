using System;
using System.Collections.Generic;
using System.Text;

namespace ActLikeAI.Config
{
    /// <summary>
    /// Represents a generic configuration file format.
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// Loads a configuration file.
        /// </summary>
        /// <param name="file">File to load.</param>
        /// <returns>Root node of the loaded configuration tree.</returns>
        ConfigNode Load(string file);


        /// <summary>
        /// Saves the configuration file.
        /// </summary>
        /// <param name="root">Root node of the configuration tree to save.</param>
        /// <param name="saveLocation">Full path to the location where to save the file.</param>
        void Save(ConfigNode root, string saveLocation);
    }
}
