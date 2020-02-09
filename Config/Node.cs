using System;
using System.Collections.Generic;

namespace ActLikeAI.Config
{
    class Node
    {
        public Node Parent { get; private set; }

        public string Name { get; private set; }

        public string Value { get; private set; }
       
        public bool Changed { get; set; }

        public List<Node> Children { get; } = new List<Node>();

        public List<Attribute> Attributes { get; } = new List<Attribute>();


        public Node(Node parent, string name, string value = "")
        {
            Parent = parent;
            Name = name;
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
