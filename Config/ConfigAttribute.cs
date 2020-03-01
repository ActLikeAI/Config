using System;
using System.Collections.Generic;
using System.Text;

namespace ActLikeAI.Config
{
    public class ConfigAttribute : IConfigKeyValuePair
    {
        public ConfigNode Parent { get; private set; }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public bool Changed { get; set; }


        public ConfigAttribute(ConfigNode parent, string key, string value = "")
        {
            Parent = parent;
            Key = key;
            Value = value;
            Changed = false;
        }


        public void Update(string value, bool notifyParent = true)
        {
            Value = value;

            if (notifyParent)
            {
                Changed = true;
                var parent = Parent;
                while (parent != null)
                {
                    parent.Changed = true;
                    parent = parent.Parent;
                }
            }
        }
    }
}
