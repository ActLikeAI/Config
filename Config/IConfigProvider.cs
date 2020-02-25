using System;
using System.Collections.Generic;
using System.Text;

namespace ActLikeAI.Config
{
    public interface IConfigProvider
    {
        ConfigNode Load(string file);
        void Save(ConfigNode root, string saveLocation);
    }
}
