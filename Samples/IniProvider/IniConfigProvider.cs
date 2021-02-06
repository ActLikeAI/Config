using System;
using System.IO;
using IniParser;
using IniParser.Model;

namespace ActLikeAI.Config.Ini
{
    /// <summary>
    /// Represents an INI configuration file format.
    /// </summary>
    public class IniConfigProvider : IConfigProvider
    {
        /// <summary>
        /// Loads an INI configuration file.
        /// </summary>
        /// <param name="file">File to load.</param>
        /// <returns>Root node of the loaded configuration tree.</returns>
        public ConfigNode Load(string file)
        {
            var parser = new FileIniDataParser();
            var data = parser.ReadFile(file);
           
            var root = new ConfigNode(null, Path.GetFileNameWithoutExtension(file));
            foreach (var global in data.Global)
                root.Children.Add(new ConfigNode(root, global.KeyName, global.Value));

            foreach (var section in data.Sections)
            {
                var child = new ConfigNode(root, section.SectionName);
                foreach (var key in section.Keys)
                    child.Children.Add(new ConfigNode(child, key.KeyName, key.Value));

                root.Children.Add(child);
            }

            return root;
        }


        /// <summary>
        /// Saves the configuration file.
        /// </summary>
        /// <param name="root">Root node of the configuration tree to save.</param>
        /// <param name="saveLocation">Full path to the location where to save the file.</param>
        public void Save(ConfigNode root, string saveLocation)
        {
            if (root is null)
                throw new ArgumentNullException(nameof(root));

            var parser = new FileIniDataParser();
            var data = File.Exists(saveLocation) ? parser.ReadFile(saveLocation) : new IniData();

            foreach (var node in root.Children)
            {
                if (node.Children.Count == 0)
                {
                    if (data.Global.ContainsKey(node.Key))
                        data.Global[node.Key] = node.Value;
                    else
                        data.Global.AddKey(node.Key, node.Value);
                }
                else
                {
                    if (!data.Sections.ContainsSection(node.Key))                    
                        data.Sections.AddSection(node.Key);

                    foreach (var child in node.Children)
                    {
                        if (data.Sections[node.Key].ContainsKey(child.Key))
                            data.Sections[node.Key][child.Key] = node.Value;
                        else
                            data.Sections[node.Key].AddKey(child.Key, child.Value);
                    }
                }            
            }

            string directory = Path.GetDirectoryName(saveLocation);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            parser.WriteFile(saveLocation, data);            
        }
    }
}
