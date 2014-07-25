using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FB2SMV
{
    namespace FBXML
    {
        public class BasicFB
        {
            //[XmlElement] public Identification Identification { get; private set; }
            [XmlArray]
            public List<VarDeclaration> InternalVars { get; set; }

            [XmlElement]
            public ECC ECC { get; set; }

            [XmlElement("Algorithm")]
            public List<Algorithm> Algorithms { get; set; }
        }

        public class AlgorithmDecl
        {
            [XmlAttribute] public string Text;

            public override string ToString()
            {
                return Text;
            }
        }

        public class Algorithm : NamedXmlDeclaration
        {
            [XmlElement] public AlgorithmDecl ST;
        }

        public class ECC
        {
            [XmlElement]
            public List<ECState> ECState { get; set; }

            [XmlElement]
            public List<ECTransition> ECTransition { get; set; }
        }

        public class ECState : NamedXmlDeclaration
        {
            [XmlElement]
            public List<ECAction> ECAction { get; set; }
        }

        public class ECAction
        {
            [XmlAttribute]
            public string Algorithm { get; set; }

            [XmlAttribute]
            public string Output { get; set; }

            public override string ToString()
            {
                return Algorithm + " | " + Output;
            }
        }

        public class ECTransition
        {
            [XmlAttribute]
            public string Source { get; set; }

            [XmlAttribute]
            public string Destination { get; set; }

            [XmlAttribute]
            public string Condition { get; set; }

            public override string ToString()
            {
                return String.Format("{0}-({2})->{1}", Source, Destination, Condition);
            }
        }

    }
}