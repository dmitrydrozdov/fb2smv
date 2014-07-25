using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace ST
    {
        public class TranslatedAlg
        {
            public TranslatedAlg(string name, List<OutputLine> lines)
            {
                Name = name;
                Lines = lines;
            }

            public readonly List<OutputLine> Lines;
            public readonly string Name;
        }

        public class Translator //TODO: fix problem with ELSE-branches
        {
            public static List<OutputLine> Translate(string algorithm)
            {
                try
                {
                    StringSplitter stringSplitter = new StringSplitter(algorithm);
                    Parcer p = new Parcer(stringSplitter);
                    Generator g = new Generator(p.Parse());
                    return g.Lines;
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new ArgumentException("Error in algorithm: \""+algorithm+"\"");
                }

                
            }
        }
    }
}