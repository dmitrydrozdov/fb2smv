using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FB2SMV
{
    namespace FBXML
    {
        [XmlRoot("FBType", Namespace = "")]
        public class FBType : NamedXmlDeclaration
        {
            [XmlElement] public Identification Identification;

            [XmlElement] public List<VersionInfo> VersionInfo;

            [XmlElement] public CompilerInfo CompilerInfo;

            [XmlElement] public InterfaceList InterfaceList;

            [XmlElement] public BasicFB BasicFB;

            [XmlElement] public FBNetwork FBNetwork;
        }
    }
}