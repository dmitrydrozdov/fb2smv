using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace ServiceClasses
    {
        public class Comparisions
        {
            public static int IntCompareGreater(int a, int b)
            {
                return a == b ? 0 : (a > b ? 1 : -1);
            }
        }
    }
}
