using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace FBCollections
    {
        public enum ConnectionType
        {
            Data,
            Event
        }

        [Serializable]
        public class InstanceParameter : FBPart
        {
            public InstanceParameter(string name, string value, string instanceName, string fbType, string comment)
            {
                Name = name;
                Comment = comment;
                FBType = fbType;
                InstanceName = instanceName;
                Value = value;
            }
            public readonly string InstanceName;
            public readonly string Value;

            public override string ToString()
            {
                return String.Format("{0}.{1}.{2} = {3}", FBType, InstanceName, Name, Value);
            }
        }

        [Serializable]
        public class FBInstance : FBPart
        {
            public FBInstance(string name, string instanceType, string comment, string fbType)
            {
                Name = name;
                Comment = comment;
                FBType = fbType; //Type of Parent FB
                InstanceType = instanceType;
            }

            public readonly string InstanceType; //Attribute "Type"

            public override string ToString()
            {
                return String.Format("({0}) {1}/{2}", InstanceType, Name, FBType);
            }
        }

        [Serializable]
        public class Connection
        {
            public Connection(ConnectionNode source, ConnectionNode destination, ConnectionType type, string fbType)
            {
                Source = source;
                Destination = destination;
                Type = type;
                FBType = fbType;
            }

            public readonly ConnectionNode Source;
            public readonly ConnectionNode Destination;
            public readonly string FBType;
            public readonly ConnectionType Type;

            public override string ToString()
            {
                return String.Format("{0}->{1}/{2}", Source, Destination, FBType);
            }
        }
        
        [Serializable]
        public class ConnectionNode
        {
            public readonly string FbType;
            public readonly string InstanceName;
            public readonly string Variable;

            public ConnectionNode(string fbType, string instName, string variable)
            {
                FbType = fbType;
                InstanceName = instName;
                Variable = variable;
            }

            public static Tuple<string, string> splitConnectionName(string connectionNodeName)
            {
                var nameSplit = connectionNodeName.Split('.');
                switch (nameSplit.Length)
                {
                    case 1:
                        return Tuple.Create("", nameSplit[0]);
                    case 2:
                        return Tuple.Create(nameSplit[0], nameSplit[1]);
                        
                    default:
                        throw new FormatException();
                }
            }

            public override string ToString()
            {
                var res = "";
                if (InstanceName != "") res += InstanceName + ".";

                res += Variable;
                return res;
            }

            public bool IsComponentVar()
            {
                return InstanceName != "";
            }
        }
    }
}