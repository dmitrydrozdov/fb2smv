using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Core
{
    class GraphContainer<KeyType, NodeType>
    {
        public GraphContainer(NodeType value)
        {
            Value = value;
            _connectivityList = new List<KeyType>();
        }

        public void AddConnection(KeyType connectedNodeKey, Func<KeyType, bool> comparisionDelegate)
        {
            KeyType exists = _connectivityList.FirstOrDefault(comparisionDelegate);
            if (exists == null) _connectivityList.Add(connectedNodeKey);
            //else throw new DuplicateNameException(); //TODO: uncomment and test
        }

        public IEnumerable<KeyType> ConnectedNodes
        {
            get
            {
                return _connectivityList;
            }
        }
        public readonly NodeType Value;
        private List<KeyType> _connectivityList;
    }

    class GraphStorage<KeyType, NodeType>
    {
        public void AddNode(KeyType key, NodeType node)
        {
            _nodes.Add(key, new GraphContainer<KeyType, NodeType>(node));
        }

        public void AddConnection(KeyType nodeKey, KeyType connectedNodeKey, Func<KeyType, bool> comparisionDelegate)
        {
            GraphContainer<KeyType, NodeType> node = _nodes[nodeKey];
            if (node == null) throw new ArgumentNullException();
            node.AddConnection(connectedNodeKey, comparisionDelegate);
        }

        public NodeType GetNode(KeyType key)
        {
            return _nodes[key].Value;
        }

        public GraphContainer<KeyType, NodeType> this[KeyType key]
        {
            get
            {
                return _nodes[key];
            }
        }

        private Dictionary<KeyType, GraphContainer<KeyType, NodeType>> _nodes = new Dictionary<KeyType, GraphContainer<KeyType, NodeType>>();

    }
}
