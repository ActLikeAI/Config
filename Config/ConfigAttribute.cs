using System;
using System.Collections.Generic;
using System.Text;

namespace ActLikeAI.Config
{
    /// <summary>
    /// Represent a configuration node attribute.
    /// </summary>
    public class ConfigAttribute : IConfigKeyValuePair
    {
        /// <summary>
        /// Parent node of the attribute.
        /// </summary>
        public ConfigNode Parent { get; private set; }


        /// <summary>
        /// Gets the key in the key/value pair.
        /// </summary>
        public string Key { get; private set; }


        /// <summary>
        /// Gets the value in the key/value pair.
        /// </summary>
        public string Value { get; private set; }


        /// <summary>
        /// Indicates whether the value in key/value pair was explicitly updated during the lifetime of the instance.
        /// </summary>
        public bool Changed { get; private set; }



        /// <summary>
        /// Initializes a new instance of ConfigAttribute class.
        /// </summary>
        /// <param name="parent">Parent node of the attribute.</param>
        /// <param name="key">Name of the attribute.</param>
        /// <param name="value">Value of the attribute.</param>
        public ConfigAttribute(ConfigNode parent, string key, string value = "")
        {
            Parent = parent;
            Key = key;
            Value = value;
            Changed = false;
        }


        /// <summary>
        /// Updates the value of the attribute.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="notifyParent">Indicates whether attribute should notify its parent node of change.</param>
        public void Update(string value, bool notifyParent = true)
        {
            Value = value;

            if (notifyParent)
            {
                Changed = true;
                ConfigNode.Notify(Parent);
            }
        }
    }
}
