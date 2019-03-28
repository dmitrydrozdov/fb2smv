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
            private static string _timeDelayModule(Storage storage, Settings settings, string fbTypeName, string setDoDi)
            {
                string smvModule = "";
                var events = storage.Events.Where(ev => ev.FBType == fbTypeName);
                var variables = storage.Variables.Where(v => v.FBType == fbTypeName);
                var start = new EventInstance(events.First(ev => ev.Name == "START"), null);
                var stop = new EventInstance(events.First(ev => ev.Name == "STOP"), null);
                var timedModule = start.Event.Timed || stop.Event.Timed;

                smvModule += FbSmvCommon.SmvModuleDeclaration(events, variables, fbTypeName);
                if (timedModule) smvModule += String.Format(Smv.VarDeclarationBlock, "INVOKEDBY", EventInstance.SmvType("FALSE", TimeScheduler.TGlobal));
                smvModule += Smv.Assign;

                smvModule += String.Format(Smv.VarInitializationBlock, "Do_", "-1");
                
                string rule = $"\n\talpha & {start.Value()} : Dt_;" +
                              $"\n\talpha & {stop.Value()} : -1;" +
                              $"\n\talpha & Di_ = 0 : {setDoDi};" +
                              "\n\tDi_ >= 0 : Di_;" +
                              "\n\tTRUE: Do_; ";
                smvModule += String.Format(Smv.NextCaseBlock, "Do_", rule);
                if (timedModule)
                {
                    smvModule += String.Format(Smv.NextVarAssignment, "INVOKEDBY.value", start.Value());
                    smvModule += String.Format(Smv.NextVarAssignment, "INVOKEDBY.ts_last", "systemclock");
                    smvModule += String.Format(Smv.NextVarAssignment, "INVOKEDBY.ts_born", 
                        LibraryTypes.E_CYCLE == fbTypeName ? "systemclock" : start.TSBorn());
                }
                smvModule += String.Format(Smv.DefineBlock, "systemclock", "TGlobal");
                smvModule += String.Format(Smv.DefineBlock, "event_START_reset", Smv.Alpha);
                smvModule += String.Format(Smv.DefineBlock, "event_STOP_reset", $"(alpha & ({start.Value()}))");
                smvModule += String.Format(Smv.DefineBlock, "event_EO_set", "(alpha & Di_=0)");

                smvModule += String.Format(Smv.DefineBlock, "alpha_reset", Smv.Alpha);
                smvModule += String.Format(Smv.DefineBlock, "beta_set", Smv.Alpha);

                smvModule += FbSmvCommon.ModuleFooter(settings) + "\n";
                return smvModule;
            }

            public static string EDelayFBModule(Storage storage, Settings settings)
            {
                return _timeDelayModule(storage, settings, LibraryTypes.E_DELAY, "-1 ");
            }

            public static string ECycleFBModule(Storage storage, Settings settings)
            {
                return _timeDelayModule(storage, settings, LibraryTypes.E_CYCLE, "Dt_");
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
