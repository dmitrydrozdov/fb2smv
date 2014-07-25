using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FB2SMV
{
    namespace FBXML
    {
        public class NamedXmlDeclaration
        {
            [XmlAttribute] public string Name;
            [XmlAttribute] public string Comment;

            public override string ToString()
            {
                return Name;
            }
        }

        public class CompilerInfo
        {
            [XmlAttribute] public string header;
        }

        /*public class NamedXmlDeclarationWithComment : NamedXmlDeclaration
    {
        [XmlAttribute] public string Comment;
    }*/

        public class Identification
        {
            [XmlAttribute] public string Standard;
        }

        public class VersionInfo
        {
            [XmlAttribute] public string Organization;
            [XmlAttribute] public string Version;
            [XmlAttribute] public string Author;
            [XmlAttribute] public string Date;
        }

        public class With
        {
            [XmlAttribute] public string Var;
        }

        public class Event : NamedXmlDeclaration
        {
            [XmlElement] public List<With> With;
        }

        public class InterfaceList
        {
            [XmlArray] public List<Event> EventInputs;

            [XmlArray] public List<Event> EventOutputs;

            [XmlArray] public List<VarDeclaration> InputVars;

            [XmlArray] public List<VarDeclaration> OutputVars;
        }

        public class VarDeclaration : NamedXmlDeclaration
        {
            [XmlAttribute] public string Type;
            [XmlAttribute] public string InitialValue;
            [XmlAttribute] public int ArraySize;

            public override string ToString()
            {
                return InitialValue == null
                    ? String.Format("({0}) {1}", Type, Name)
                    : String.Format("({0}) {1} = {2}", Type, Name, InitialValue);
            }
        }
    }
}