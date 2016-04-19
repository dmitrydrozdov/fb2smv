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
            public static string EDelayFBModule(Storage storage, Settings settings)
            {
                throw new NotImplementedException();
            }

            public static string ECycleFBModule(Storage storage, Settings settings)
            {
                string smvModule = "";
                var events = storage.Events.Where(ev => ev.FBType == LibraryTypes.E_CYCLE);
                var variables = storage.Variables.Where(v=>v.FBType == LibraryTypes.E_CYCLE);

                smvModule += FbSmvCommon.SmvModuleDeclaration(events, variables, LibraryTypes.E_CYCLE);
                smvModule += Smv.Assign;

                string doRule = "\n\talpha & event_START : Dt_;\n\talpha & event_STOP : -1;\n\talpha & Di_ = 0 : Dt_;\n\tDi_ >= 0 : Di_;\n\tTRUE: Do_; ";
                smvModule += String.Format(Smv.NextCaseBlock, "Do_", doRule);

                smvModule += String.Format(Smv.DefineBlock, "event_START_reset", Smv.Alpha);
                smvModule += String.Format(Smv.DefineBlock, "event_STOP_reset", "(alpha & (event_START))");
                smvModule += String.Format(Smv.DefineBlock, "event_EO_set", "(alpha & Di_=0)");

                smvModule += String.Format(Smv.DefineBlock, "alpha_reset", Smv.Alpha);
                smvModule += String.Format(Smv.DefineBlock, "beta_set", Smv.Alpha);

                smvModule += FbSmvCommon.ModuleFooter(settings) + "\n";
                return smvModule;
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
