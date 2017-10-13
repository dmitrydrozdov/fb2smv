using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2SMV.FBCollections;

namespace FB2SMV
{
    namespace Core
    {
        internal class FbSmvCommonAsynch : FbSmvCommon
        {
            public override string ModuleParametersString(IEnumerable<Event> events, IEnumerable<Variable> variables, string preffix = null)
            {
                return _moduleParamsEvVarString(events, variables, preffix).TrimEnd(Smv.ModuleParameters.Splitter.ToCharArray());
            }
            public override string VarSamplingRule(string varName, IEnumerable<WithConnection> withConnections, bool basic, int arrayIndex = -1)
            {
                string samplingEvents = "";
                //string rules = "";
                foreach (WithConnection connection in withConnections.Where(conn => conn.Var == varName))
                {
                    samplingEvents += Smv.ModuleParameters.Event(connection.Event) + " | ";
                }
                string rule = "";
                if (basic) rule += Smv.OsmStateVar + "=" + Smv.Osm.S0;
                if (samplingEvents != "")
                    rule += String.Format(" & ({0})", samplingEvents.Trim(Smv.OrTrimChars));
                string arrayIndexString = arrayIndex > -1 ? "[" + arrayIndex + "]" : "";
                rule += " : " + Smv.ModuleParameters.Variable(varName) + arrayIndexString + " ;\n";
                rule = "\t" + rule.Trim(Smv.AndTrimChars);
                return rule;
            }
            public override string ModuleFooter(Settings settings)
            {
                if (settings.UseProcesses) return "\n" + Smv.Fairness(Smv.Running);
                else return "";
            }
        }


        internal class FbSmvCommon
        {
            public virtual string ModuleParametersString(IEnumerable<Event> events, IEnumerable<Variable> variables, string preffix = null)
            {
                string moduleParameters = _moduleParamsEvVarString(events, variables, preffix);
                if (preffix == null)
                {
                    moduleParameters += Smv.Alpha + Smv.ModuleParameters.Splitter;
                    moduleParameters += Smv.Beta + Smv.ModuleParameters.Splitter;
                }
                else
                {
                    moduleParameters += preffix + "_" + Smv.Alpha + Smv.ModuleParameters.Splitter;
                    moduleParameters += preffix + "_" + Smv.Beta + Smv.ModuleParameters.Splitter;
                }
                return moduleParameters.TrimEnd(Smv.ModuleParameters.Splitter.ToCharArray());
            }

            protected string _moduleParamsEvVarString(IEnumerable<Event> events, IEnumerable<Variable> variables, string preffix = null)
            {
                string moduleParameters = "";
                foreach (Event ev in events)
                {
                    if (preffix == null)
                        moduleParameters += (Smv.ModuleParameters.Event(ev.Name) + Smv.ModuleParameters.Splitter);
                    else moduleParameters += (preffix + "_" + ev.Name + Smv.ModuleParameters.Splitter);
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
                return moduleParameters;
            }

            public virtual string VarSamplingRule(string varName, IEnumerable<WithConnection> withConnections, bool basic, int arrayIndex = -1)
            {
                string samplingEvents = "";
                //string rules = "";
                foreach (WithConnection connection in withConnections.Where(conn => conn.Var == varName))
                {
                    samplingEvents += Smv.ModuleParameters.Event(connection.Event) + " | ";
                }
                string rule = "\t" + Smv.Alpha;
                if (basic) rule += " & " + Smv.OsmStateVar + "=" + Smv.Osm.S0;
                if (samplingEvents != "")
                    rule += String.Format(" & ({0})", samplingEvents.Trim(Smv.OrTrimChars));
                string arrayIndexString = arrayIndex > -1 ? "[" + arrayIndex + "]" : "";
                rule += " : " + Smv.ModuleParameters.Variable(varName) + arrayIndexString + " ;\n";
                return rule;
            }

            public string DefineExistsInputEvent(IEnumerable<Event> events)
            {
                string inputEvents = events.Where(ev => ev.Direction == Direction.Input).Aggregate("", (current, ev) => current + (Smv.ModuleParameters.Event(ev.Name) + " | "));
                if (inputEvents == "") inputEvents = Smv.False;
                return String.Format(Smv.DefineBlock, Smv.ExistsInputEvent, inputEvents.Trim(Smv.OrTrimChars));
            }

            public virtual string ModuleFooter(Settings settings)
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

            public string SmvModuleDeclaration(IEnumerable<Event> events, IEnumerable<Variable> variables, string fbTypeName)
            {

                return String.Format(Smv.ModuleDef, fbTypeName, this.ModuleParametersString(events, variables));
            }

        }

    }
}