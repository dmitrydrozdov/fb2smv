using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FB2SMV.Core;

namespace FB_to_nuSMV
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = args[0];

            FBClassParcer parcer = new FBClassParcer();
            parcer.ParseRecursive(filename);
            CM_SMV translator = new CM_SMV(parcer.Storage);

            string outFileName = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".smv");
            StreamWriter wr = new StreamWriter(outFileName);
            foreach (string fbSmv in translator.TranslateAll())
            {
                wr.Write(fbSmv + "\n");
            }
            wr.Close();
        }
    }
}
