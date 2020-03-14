using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;

namespace ActLikeAI.Config
{
    public static class Config
    {

        public static void Load(string definitionFile, string saveDirectory)
        {
            if (manager is null)
                manager = new ConfigManager();

            var file = new ConfigFile(definitionFile, new XmlConfigProvider())
                .AddAppData(saveDirectory, true)
                .AddCurrentDirectory();

            manager.Add(file);
        }


        public static void Load(ConfigFile file)
        {
            if (manager is null)
                manager = new ConfigManager();

            manager.Add(file);
        }


        public static void Save() 
            => manager.Save();


        public static string Get(string key) 
            => manager.Get(key);


        public static T Get<T>(string key) 
            => manager.Get<T>(key);


        public static void Set(string key, string value) 
            => manager.Set(key, value);


        public static void Set<T>(string key, T value)
            => manager.Set<T>(key, value);

        
        private static ConfigManager manager;
    }
}
