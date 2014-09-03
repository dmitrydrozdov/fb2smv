using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2SMV.Core;
using FB2SMV.FBCollections;

namespace GUI
{
    [Serializable]
    struct ProjectFileStructure
    {
        public Storage Storage;
        public List<ExecutionModel> ExecutionModels;
    }
}
