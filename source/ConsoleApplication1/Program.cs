using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FB2SMV.Core;
using FB2SMV.FBCollections;

namespace FB_to_nuSMV
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = args[0];

            FBClassParcer parcer = new FBClassParcer();
            parcer.ParseRecursive(filename);

            var compositeBlocks = parcer.Storage.Types.Where((fbType) => fbType.Type == FBClass.Composite);
            bool solveDispatchingProblem = true;
            IEnumerable<IDispatcher> dispatchers = DispatchersCreator.Create(compositeBlocks, parcer.Storage.Instances, solveDispatchingProblem);
            SmvCodeGenerator translator = new SmvCodeGenerator(parcer.Storage, dispatchers);

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
