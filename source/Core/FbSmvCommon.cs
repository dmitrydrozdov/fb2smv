using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2SMV.FBCollections;

namespace FB2SMV
{
    namespace Core
    {
        internal class FbSmvCommon
        {
            public static string ModuleParametersString(IEnumerable<Event> events, IEnumerable<Variable> variables, string preffix = null)
            {
                string moduleParameters = "";
                foreach (Event ev in events)
                {
                    if (preffix == null)
                        moduleParameters += EventInstance.Name(ev.Name) + Smv.ModuleParameters.Splitter;
                    else moduleParameters += preffix + "_" + ev.Name + Smv.ModuleParameters.Splitter;
                }
                foreach (Variable variable in variables)
                {
                    if (variable.Direction != Direction.Internal && !variable.IsConstant)
                    {
                        if (preffix == null)
                            moduleParameters += (Smv.ModuleParameters.Variable(variable.Name) +
                                                 Smv.ModuleParameters.Splitter);
                        else moduleParameters += (preffix + "_" + variable.Name + Smv.ModuleParameters.Splitter);
                    }
                }
                if (preffix == null)
                {
                    moduleParameters += TimeScheduler.TGlobal + Smv.ModuleParameters.Splitter;
                    moduleParameters += Smv.Alpha + Smv.ModuleParameters.Splitter;
                    moduleParameters += Smv.Beta + Smv.ModuleParameters.Splitter;
                }
                else
                {
                    moduleParameters += preffix + "_" + TimeScheduler.TGlobal + Smv.ModuleParameters.Splitter;
                    moduleParameters += preffix + "_" + Smv.Alpha + Smv.ModuleParameters.Splitter;
                    moduleParameters += preffix + "_" + Smv.Beta + Smv.ModuleParameters.Splitter;
                }
                return moduleParameters.TrimEnd(Smv.ModuleParameters.Splitter.ToCharArray());
            }

            public static string VarSamplingRule(string varName, IEnumerable<WithConnection> withConnections, bool basic, int arrayIndex = -1)
            {
                string samplingEvents = "";
                //string rules = "";
                foreach (WithConnection connection in withConnections.Where(conn => conn.Var == varName))
                {
                    samplingEvents += EventInstance.Value(connection.Event) + " | ";
                }
                string rule = "\t" + Smv.Alpha;
                if (basic) rule += " & " + Smv.OsmStateVar + "=" + Smv.Osm.S0;
                if (samplingEvents != "")
                    rule += String.Format(" & ({0})", samplingEvents.Trim(Smv.OrTrimChars));
                string arrayIndexString = arrayIndex > -1 ? "[" + arrayIndex + "]" : "";
                rule += " : " + Smv.ModuleParameters.Variable(varName) + arrayIndexString + " ;\n";
                return rule;
            }

            public static string DefineExistsInputEvent(IEnumerable<Event> events)
            {
                string inputEvents = events.Where(ev => ev.Direction == Direction.Input).Aggregate("", (current, ev) => current + (EventInstance.Value(ev.Name) + " | "));
                if (inputEvents == "") inputEvents = Smv.False;
                return String.Format(Smv.DefineBlock, Smv.ExistsInputEvent, inputEvents.Trim(Smv.OrTrimChars));
            }


            public static string ModuleDefines()
            {
                string defines = "";
                defines += String.Format(Smv.DefineBlock, "systemclock", "TGlobal");
                return defines;
            }
            
            public static string ModuleFooter(Settings settings)
            {
                if (settings.UseProcesses) return "\n" + Smv.Fairness(Smv.Running);
                else
                {
                    string ret = "";
                    ret += Smv.Fairness(Smv.Alpha);
                    ret += Smv.Fairness(Smv.Beta);
                    return ret + "\n";
                }
            }

            public static string SmvModuleDeclaration(IEnumerable<Event> events, IEnumerable<Variable> variables, string fbTypeName)
            {

                return String.Format(Smv.ModuleDef, fbTypeName, FbSmvCommon.ModuleParametersString(events, variables));
            }

            public static string InvokedByRules(IEnumerable<Event> events)
            {
                var inputEvents = events.Where(e => e.Direction == Direction.Input);
                string rules = "";
                var valueRules = inputEvents.Aggregate("", (current, ev) => current + "\t" + EventInstance.Value(ev.Name) + " : " + EventInstance.Value(ev.Name) + ";\n");
                var tsLastRules = inputEvents.Aggregate("", (current, ev) => current + "\t" + EventInstance.Value(ev.Name) + " : " + EventInstance.TSLast(ev.Name) + ";\n");
                var tsBornRules = inputEvents.Aggregate("", (current, ev) => current + "\t" + EventInstance.Value(ev.Name) + " : " + EventInstance.TSBorn(ev.Name) + ";\n");
                rules += String.Format(Smv.NextCaseBlock, "INVOKEDBY.value", valueRules);
                rules += String.Format(Smv.NextCaseBlock, "INVOKEDBY.ts_last", tsLastRules);
                rules += String.Format(Smv.NextCaseBlock, "INVOKEDBY.ts_born", tsBornRules);
                return rules;
            }
        }

    }
}