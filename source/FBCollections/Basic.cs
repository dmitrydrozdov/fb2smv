using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace FBCollections
    {
        public enum AlgorithmLanguages
        {
            ST
        }

        [Serializable]
        public class ECState : FBPart
        {
            public ECState(string name, string comment, string fbType, int actionsCount)
            {
                ActionsCount = actionsCount;
                Name = name;
                Comment = comment;
                FBType = fbType;
            }

            public readonly int ActionsCount;

            public override string ToString()
            {
                return String.Format("{0}({1})", Name, FBType);
            }
        }

        [Serializable]
        public class ECTransition
        {
            public ECTransition(string fbType, string source, string destination, string condition)
            {
                Source = source;
                Destination = destination;
                Condition = condition;
                FBType = fbType;
            }

            public readonly string FBType;
            public readonly string Source;
            public readonly string Destination;
            public readonly string Condition;

            public override string ToString()
            {
                return String.Format("{0}-({2})->{1}", Source, Destination, Condition);
            }
        }

        [Serializable]
        public class ECAction
        {
            public ECAction(string fbType, int number, string ecState, string algorithm, string output)
            {
                Number = number;
                ECState = ecState;
                Algorithm = algorithm;
                Output = output;
                FBType = fbType;
            }

            public readonly int Number;
            public readonly string ECState;
            public readonly string Algorithm;
            public readonly string Output;
            public readonly string FBType;

        }

        [Serializable]
        public class Algorithm : FBPart
        {
            public Algorithm(string name, string comment, string fbType, AlgorithmLanguages language, string text)
            {
                Language = language;
                Text = text;
                Name = name;
                Comment = comment;
                FBType = fbType;
            }

            public readonly AlgorithmLanguages Language;
            public readonly string Text;
        }
    }
}