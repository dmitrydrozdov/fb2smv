using FB2SMV.FBCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        static class LibraryFBTypes
        {
            public static string EDelayFBModule()
            {
                throw new NotImplementedException();
            }

            public static string ESplitFBModule(Storage storage, Settings settings)
            {
                string smvModule = "";
                var events = storage.Events.Where(ev => ev.FBType == LibraryTypes.E_SPLIT);
                var variables = new List<Variable>();

                smvModule += FbSmvCommon.SmvModuleDeclaration(events, variables, LibraryTypes.E_SPLIT);
                smvModule += String.Format(Smv.DefineBlock, "event_EI_reset", "event_EI");
                smvModule += String.Format(Smv.DefineBlock, "event_EO1_set", "event_EI");
                smvModule += String.Format(Smv.DefineBlock, "event_EO2_set", "event_EI");

                smvModule += String.Format(Smv.DefineBlock, "alpha_reset", Smv.Alpha);
                smvModule += String.Format(Smv.DefineBlock, "beta_set", Smv.Alpha);

                smvModule += FbSmvCommon.ModuleFooter(settings) + "\n";
                return smvModule;
            }
        }
    }
}
