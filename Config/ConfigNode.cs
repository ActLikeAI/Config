using System;
using System.Collections.Generic;

namespace ActLikeAI.Config
{
    /// <summary>
    /// Represent a configuration node.
    /// </summary>
    public class ConfigNode : IConfigKeyValuePair
    {
        /// <summary>
        /// Parent of the node. It's null for the root node.
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
        /// List of node's child nodes.
        /// </summary>
        public List<ConfigNode> Children { get; } = new List<ConfigNode>();


        /// <summary>
        /// List of node's attributes.
        /// </summary>
        public List<ConfigAttribute> Attributes { get; } = new List<ConfigAttribute>();


        /// <summary>
        /// Initializes a new instance of ConfigNode class.
        /// </summary>
        /// <param name="parent">Parent node of the current instance.</param>
        /// <param name="key">Name of the node.</param>
        /// <param name="value">Value of the node.</param>
        public ConfigNode(ConfigNode parent, string key, string value = "")
        {
            Parent = parent;
            Key = key;
            Value = value;
            Changed = false;
        }


        /// <summary>
        /// Updates the value of the node.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="notifyParent">Indicates whether node should notify its parent node of change.</param>
        public void Update(string value, bool notifyParent = true)
        {
            Value = value;

            if (notifyParent)            
                Notify(this);            
        }


        /// <summary>
        /// Notifies the nodes up the hierarchy that the value has changed.
        /// </summary>
        /// <param name="node">Starting node.</param>
        public static void Notify(ConfigNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            node.Changed = true;
            var parent = node.Parent;
            while (parent != null)
            {
                parent.Changed = true;
                parent = parent.Parent;
            }
        }
    }
}
