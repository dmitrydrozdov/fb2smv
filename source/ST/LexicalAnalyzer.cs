using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace ST
    {

        internal class StringSplitter
            //split input string to basic expressions (a:=b / IF / <condition> / THEN / ELSE / END_IF)
        {
            private static string[] splitters = {"end_if", "then", "else", "if", ";"};

            public StringSplitter(string input)
            {
                _sourceCode = input;
                _splitStrings = separateStrings(_sourceCode);
                _index = 0;
            }

            public string GetNextToken()
            {
                return _splitStrings[_index++];
            }

            public bool AnyString()
            {
                return _splitStrings.Any();
            }

            public bool End()
            {
                return _index >= _splitStrings.Count;
            }

            public List<string> Strings
            {
                get { return _splitStrings; }
                private set { }
            }

            private List<string> separateStrings(string input)
            {
                int index;
                input = " " + input;
                SortedSet<int> splitIndex = new SortedSet<int>();

                foreach (var splitter in splitters)
                {
                    index = 0;
                    while (index >= 0)
                    {
                        index = input.IndexOf(splitter, index + 1, StringComparison.InvariantCultureIgnoreCase);
                        if (index >= 0 && input.Substring(index - 1, 1) != "_")
                        {
                            splitIndex.Add(index);
                            index += splitter.Length;
                            splitIndex.Add(index);
                            index--; //correction for next cycle iteration
                        }
                    }
                }

                List<string> output = new List<string>();
                //bool first = true;
                int prevIndex = 0;
                foreach (var i in splitIndex)
                {
                    string s = input.Substring(prevIndex, i - prevIndex);
                    char[] trimChars = {' ', '\r', '\n', ';', '\t'};
                    s = s.Trim(trimChars);

                    if (s != "") output.Add(s);
                    prevIndex = i;
                }
                return output;
            }

            private string _sourceCode;
            private List<string> _splitStrings;
            private int _index;
        }
    }
}