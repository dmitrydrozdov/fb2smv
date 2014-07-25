using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FB2SMV.FBCollections;

namespace Core
{
    public class VarDependencyGraph
    {
        private Storage _storage;
        private GraphStorage<string, Variable> _graph;

        public static string VariableKey(Variable variable)
        {
            return variable.FBType + "." + variable.Name;
        }

        public Variable GetVariableByKey(string key)
        {
            string[] keySplit = key.Split('.'); //keySplit[0]=FBType; keySplit[1]=VariableName
            return _storage.Variables.FirstOrDefault(v => v.FBType == keySplit[0] && v.Name == keySplit[1]);
        }

        private string GetConnectionNodeKey(string connectionNodeString, string fbType)
        {
            string nodeKey;
            string[] srcSplit = connectionNodeString.Split('.');
            if (srcSplit.Count() == 0) throw new ArgumentNullException();
            else if (srcSplit.Count() == 1) nodeKey = fbType + '.' + connectionNodeString;
            else if (srcSplit.Count() == 2)
            {
                var instance = _storage.Instances.FirstOrDefault(inst => inst.Name == srcSplit[0] && inst.FBType == fbType);
                nodeKey = instance.InstanceType + '.' + srcSplit[1];
            }
            else throw new FormatException();

            return nodeKey;
        }

        public VarDependencyGraph(Storage storage)
        {
            _storage = storage;
        }

        public void Construct()
        {
            _graph = new GraphStorage<string, Variable>();
            foreach (Variable variable in _storage.Variables)
            {
                _graph.AddNode(VariableKey(variable), variable);
            }
            foreach (Connection connection in _storage.Connections.Where(conn => conn.Type == ConnectionType.Data))
            {
                string nodeKey = GetConnectionNodeKey(connection.Source, connection.FBType);
                string connectedNodeKey = GetConnectionNodeKey(connection.Destination, connection.FBType);

                _graph.AddConnection(nodeKey, connectedNodeKey, k => k == connectedNodeKey);
                _graph.AddConnection(connectedNodeKey, nodeKey, k => k == nodeKey);
            }
        }

        //Find connected component using breadth-first search and return a collection with all found nodes
        public IEnumerable<Variable> GetConnectedVariables(string variableKey)
        {
            List<Variable> found = new List<Variable>();
            Queue<string> searchQueue = new Queue<string>();
            HashSet<string> discovered = new HashSet<string>();
            searchQueue.Enqueue(variableKey);
            while (searchQueue.Any())
            {
                string currentVarKey = searchQueue.Dequeue();
                if (!discovered.Contains(currentVarKey))
                {
                    discovered.Add(currentVarKey);
                    if (currentVarKey != variableKey)
                    {
                        found.Add(GetVariableByKey(currentVarKey));
                    }
                    foreach (string node in _graph[currentVarKey].ConnectedNodes)
                    {
                        searchQueue.Enqueue(node);
                    }
                }
                //else throw new Exception();
            }
            return found;
        }

        /*private static void bfSearch()
        {
            
        }*/
    }
}
