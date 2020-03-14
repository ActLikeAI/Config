using System;
using System.Collections.Generic;
using System.Text;

namespace ActLikeAI.Config
{
    /// <summary>
    /// Provides a common interface to node and attribute.
    /// </summary>
    public interface IConfigKeyValuePair
    {
        /// <summary>
        /// Gets the key in the key/value pair.
        /// </summary>
        string Key { get; }


        /// <summary>
        /// Gets the value in the key/value pair.
        /// </summary>
        string Value { get; }


        /// <summary>
        /// Indicates whether the value in key/value pair was explicitly updated during the lifetime of the instance.
        /// </summary>
        bool Changed { get; }


        /// <summary>
        /// Updates the value in the key/value pair.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="notifyParent"></param>
        void Update(string value, bool notifyParent = true);
    }
}
