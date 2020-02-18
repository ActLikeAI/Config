using System;
using System.Collections.Generic;
using System.Text;

namespace ActLikeAI.Config
{
    public interface IConfigProvider
    {
        public ConfigNode Load(string file);
        public void Save(ConfigNode root, string defaultsLocation, string userLocation);
    }
}
