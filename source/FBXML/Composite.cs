using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FB2SMV
{
    namespace FBXML
    {
        public class InstanceParameter : NamedXmlDeclaration
        {
            [XmlAttribute] public string Value;
        }

        public class FBInstance : NamedXmlDeclaration
        {
            [XmlAttribute] public string Type;
            [XmlElement("Parameter")] public List<InstanceParameter> Parameters;
        }

        public class Connection
        {
            [XmlAttribute] public string Source;

            [XmlAttribute] public string Destination;

            public override string ToString()
            {
                return String.Format("{0}->{1}", Source, Destination);
            }
        }

        public class FBNetwork
        {
            [XmlElement] public List<FBInstance> FB;

            [XmlArray] [XmlArrayItem("Connection")] public List<Connection> EventConnections;

            [XmlArray] [XmlArrayItem("Connection")] public List<Connection> DataConnections;
        }
    }
}