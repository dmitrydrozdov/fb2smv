using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace FBCollections
    {
        [Serializable]
        public class AlgorithmLine
        {
            public readonly int NI;
            public readonly string Variable;
            public readonly string Condition;
            public readonly string Value;
            public readonly string FBType;
            public readonly string AlgorithmName;

            public AlgorithmLine(int counter, string variable, string condition, string value, string fbType,
                string algorithmName)
            {
                NI = counter;
                Variable = variable;
                if (condition == "") Condition = "1";
                else Condition = condition;
                Value = value;
                FBType = fbType;
                AlgorithmName = algorithmName;
            }
        }
    }
}