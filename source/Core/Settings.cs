using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        public class Settings
        {
            //todo: reload after settings change
            public bool ModularArithmetics = false;
            public bool UseProcesses = false;
            public bool GenerateDummyProperty = true;
            public bool nuXmvInfiniteDataTypes = true;
        }
    }
}
