using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        public static class IEC61499
        {
            public static class DataTypes
            {
                public static string BOOL = "BOOL";
                public static string INT = "INT";
                public static string UINT = "UINT";
                public static string DINT = "DINT";
                public static string TIME = "TIME";
                public static string STRING = "STRING";
                public static string REAL = "REAL";
            }

            public static bool DataTypeMatch(string type1, string type2)
            {
                return (String.Compare(type1, type2, StringComparison.InvariantCultureIgnoreCase) == 0);
            }
        }
    }
}