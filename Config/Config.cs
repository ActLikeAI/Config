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
 

        public static void Load(string definitionFile, string saveDirectory, char separator = '.')
        {
            ConfigFile.Separator = separator;
            
            file = new ConfigFile(definitionFile, new XmlConfigProvider())                
                .AddAppData(saveDirectory, true)               
                .AddCurrentDirectory();
        }


        public static void Save() 
            => file.Save();


        public static string Get(string key) 
            => file.Get(key);


        public static T Get<T>(string key) 
            => file.Get<T>(key);


        public static void Set(string key, string value) 
            => file.Set(key, value);


        public static void Set<T>(string key, T value)
            => file.Set<T>(key, value);


        private static ConfigFile file;
    }
}
