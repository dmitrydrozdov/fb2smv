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
            public Connection(string source, string destination, ConnectionType type, string fbType)
            {
                Source = new ConnectionNode(fbType, source);
                Destination = new ConnectionNode(fbType, destination);
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
        
        public class ConnectionNode
        {
            public readonly string FbType; // todo: set to type of instance
            public readonly string InstanceName;
            public readonly string Variable;

            public ConnectionNode(string fbType, string instName, string variable)
            {
                FbType = fbType;
                InstanceName = instName;
                Variable = variable;
            }

            public ConnectionNode(string fbType, string connectionNodeName)
            {
                FbType = fbType;
                var nameSplit = connectionNodeName.Split('.');
                switch (nameSplit.Length)
                {
                    case 1:
                        InstanceName = "";
                        Variable = nameSplit[0];
                        break;
                    case 2:
                        InstanceName = nameSplit[0];
                        Variable = nameSplit[1];
                        break;
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