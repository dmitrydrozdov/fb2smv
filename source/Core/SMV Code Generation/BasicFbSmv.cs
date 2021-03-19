using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FB2SMV.FBCollections;
using FB2SMV.ST;

namespace FB2SMV
{
    namespace Core
    {
        internal class BasicFbSmv
        {
            public static string ModuleHeader(IEnumerable<Event> events, IEnumerable<Variable> variables, string fbTypeName)
            {
                string outp = "";
                outp += FbSmvCommon.SmvModuleDeclaration(events, variables, fbTypeName);
                foreach (var variable in variables)
                {
                    if (variable.ArraySize == 0)
                    {
                        if(variable.IsConstant) outp += String.Format(Smv.DefineBlock, variable.Name, variable.InitialValue);
                        else outp += String.Format(Smv.VarDeclarationBlock, variable.Name, variable.SmvType);
                    }
                    else
                    {
                        for (int i = 0; i < variable.ArraySize; i++)
                        {
                            if (variable.IsConstant) outp += String.Format(Smv.VarDeclarationBlock, variable.Name + Smv.ArrayIndex(i), variable.InitialValue); //TODO: parse initial values for arrays
                        }
                        if (!variable.IsConstant)
                            outp += String.Format(Smv.VarDeclarationBlock, variable.Name, " array " + "0.." + (variable.ArraySize - 1).ToString() + " of " + variable.SmvType);
                    }
                }
                return outp;
            }
            public static string OsmStatesDeclaration()
            {
                return String.Format("VAR {0} : {{{1}, {2}, {3}}};\n", Smv.OsmStateVar, Smv.Osm.S0, Smv.Osm.S1, Smv.Osm.S2);
            }
            public static string EccStatesDeclaration(IEnumerable<ECState> states)
            {
                string eccStates = "";
                foreach (var ecState in states)
                {
                    eccStates += Smv.EccState(ecState.Name) + Smv.ModuleParameters.Splitter;
                }
                return String.Format("VAR {0} : {{{1}}};\n", Smv.EccStateVar,
                    eccStates.TrimEnd(Smv.ModuleParameters.Splitter.ToCharArray()));
            }
            public static string EcActionsCounterDeclaration(IEnumerable<ECState> states)
            {
                return String.Format("VAR {0}: 0..{1};\n", Smv.EcActionsCounterVar, states.Max(state => state.ActionsCount));
            }
            public static string AlgStepsCounterDeclaration(IEnumerable<TranslatedAlg> translatedAlgorithms)
            {
                string output = "";
                if (translatedAlgorithms.Any()) output += String.Format("VAR {0}: 0..{1};\n", Smv.AlgStepsCounterVar, translatedAlgorithms.Max(alg => alg.Lines.Max(line => line.NI)));
                else output += String.Format("VAR {0}: 0..{1};\n", Smv.AlgStepsCounterVar, "1");
                return output;
            }

            public static string NdtDeclaration(Variable variable)
            {
                string outp = "";
                if (variable.Name == "NDT") 
                    outp += String.Format(Smv.VarDeclarationBlock, variable.Name, variable.SmvType);
                return outp;
            }
            public static string EcStateChangeBlock(IEnumerable<ECTransition> transitions, IEnumerable<Event> events)
            {
                string ecTransitionsSmv = "";

                foreach (var transition in transitions)
                {
                    ecTransitionsSmv += "\t";
                    ecTransitionsSmv += Smv.EccStateVar + "=" + Smv.EccState(transition.Source);
                    ecTransitionsSmv += " & ";
                    ecTransitionsSmv += Smv.OsmStateVar + "=" + Smv.Osm.S1;

                    if (transition.Condition != null && transition.Condition != "1")
                    {
                        ecTransitionsSmv += " & " + _translateEventNames(Smv.ClearConditionExpr(transition.Condition), events);
                    }
                    ecTransitionsSmv += " : ";
                    ecTransitionsSmv += Smv.EccState(transition.Destination);
                    ecTransitionsSmv += ";\n";
                }
                return String.Format("\n" + Smv.NextCaseBlock + "\n", Smv.EccStateVar, ecTransitionsSmv);
            }

            public static string NdtChangeBlock(IEnumerable<ECTransition> transitions)
            {
                string ecTransitionsSmv = "";

                foreach (var transition in transitions)
                {
                    if (transition.Condition != null && transition.Condition != "1" && transition.Condition.Contains("NDT"))
                    {
                        ecTransitionsSmv += "\t";
                        ecTransitionsSmv += Smv.EccStateVar + "=" + Smv.EccState(transition.Source);
                        ecTransitionsSmv += " & ";
                        ecTransitionsSmv += Smv.OsmStateVar + "=" + Smv.Osm.S1;
                        ecTransitionsSmv += " : ";
                        ecTransitionsSmv += "{TRUE,FALSE}";
                        ecTransitionsSmv += ";\n";
                    }
                }
                return String.Format("\n" + Smv.NextCaseBlock + "\n", "NDT", ecTransitionsSmv);
            }
            public static string EcActionsCounterChangeBlock(IEnumerable<ECState> states)
            {
                string rules = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S1 + ": 1;\n";
                string rformat = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S2 + " & " + Smv.AlgStepsCounterVar +
                                 "=0 & ({0}): ";
                const string modFormatSuffix = "({1}) mod {2};\n";
                const string normalFormatSuffix = "{1};\n";

                string rule1 = "";
                string rule2 = "";
                foreach (ECState state in states)
                {
                    string add = "(";
                    add += Smv.EccStateVar + "=" + Smv.EccState(state.Name);
                    add += " & ";
                    add += Smv.EcActionsCounterVar + " {0} " + (state.ActionsCount > 0 ? state.ActionsCount : 1);
                    add += ") | ";
                    rule1 += String.Format(add, "<");
                    rule2 += String.Format(add, "=");
                }
                rules += String.Format(rformat + modFormatSuffix, rule1.TrimEnd(Smv.OrTrimChars), Smv.EcActionsCounterVar + " + 1", states.Max(state => state.ActionsCount) + 1);
                rules += String.Format(rformat + normalFormatSuffix, rule2.TrimEnd(Smv.OrTrimChars), " 0 ");

                return String.Format(Smv.NextCaseBlock, Smv.EcActionsCounterVar, rules);
            }
            public static string AlgStepsCounterChangeBlock(IEnumerable<ECState> states, IEnumerable<ECAction> actions, IEnumerable<TranslatedAlg> algorithms)
            {
                //if (!algorithms.Any()) return "";
                string rules = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S1 + ": 1;\n";
                string rformat = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S2 + " & ({0}):";
                const string modFormatSuffix = "({1}) mod {2};\n";
                const string normalFormatSuffix = "{1};\n";
                string rule1 = "";
                string rule2 = "";

                foreach (ECState state in states)
                {
                    string stateName = state.Name;
                    if (state.ActionsCount == 0)
                    {
                        string add = "(";
                        add += Smv.EccStateVar + "=" + Smv.EccState(state.Name);
                        add += " & ";
                        add += Smv.EcActionsCounterVar + " = 1";
                        add += " & ";
                        add += Smv.AlgStepsCounterVar + " {0} 1";
                        add += ") | ";
                        rule1 += String.Format(add, "<");
                        rule2 += String.Format(add, "=");
                    }
                    else
                    {
                        foreach (ECAction action in actions.Where(act => act.ECState == stateName))
                        {
                            int algsCount = 0;
                            if (action.Algorithm != null)
                            {
                                var actionAlg = algorithms.FirstOrDefault(alg => alg.Name == action.Algorithm);
                                if (actionAlg != null)
                                {
                                    algsCount = actionAlg.Lines.Count;
                                }
                            }
                            _addCounterRules(ref rule1, ref rule2, state, algsCount, action);
                        }
                    }
                }
                int maxAlgStepsCount = algorithms.Any() ? algorithms.Max(alg => alg.Lines.Max(line => line.NI)) : 1;
                rules += String.Format(rformat + modFormatSuffix, rule1.TrimEnd(Smv.OrTrimChars), Smv.AlgStepsCounterVar + " + 1", maxAlgStepsCount + 1);
                rules += String.Format(rformat + normalFormatSuffix, rule2.TrimEnd(Smv.OrTrimChars), " 0 ");

                return String.Format(Smv.NextCaseBlock, Smv.AlgStepsCounterVar, rules);
            }
            public static string ModuleVariablesInitBlock(IEnumerable<Variable> variables)
            {
                string varsInit = "-- _moduleVariablesInitBlock\n";
                foreach (var variable in variables)
                {
                    if (!variable.IsConstant)
                    {
                        if (variable.ArraySize == 0)
                            varsInit += String.Format(Smv.VarInitializationBlock, variable.Name, Smv.InitialValue(variable));
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                varsInit += String.Format(Smv.VarInitializationBlock, variable.Name + Smv.ArrayIndex(i), Smv.InitialValue(variable));
                            }
                        }
                    }
                }
                return varsInit;
            }

            public static string NdtInitBlock(Variable variable)
            {
                string varsInit = "-- _NonDeterministicVariableInitBlock\n";
                varsInit += String.Format(Smv.NdtInitializationBlock, variable.Name);
                return varsInit;
            }
            public static string EventInputsResetRules(IEnumerable<Event> events, ExecutionModel executionModel, bool eventSignalResetSolve, bool useProcesses)
            {
                string rules = "";
                string commonResetRule = Smv.OsmStateVar + "=" + Smv.Osm.S1;
                if (!eventSignalResetSolve) commonResetRule += " & " + Smv.ExistsEnabledEcTran;
                string priorityResetRule = "";
                const string setValue = " : {0};\n";
                foreach (PriorityEvent ev in executionModel.InputEventsPriorities/*events.Where(ev => ev.Direction == Direction.Input)*/)
                {
                    if (priorityResetRule == "")
                    {
                        string rule = "\t" + commonResetRule;

                        if (useProcesses) rules += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Value.Name), rule + String.Format(setValue, Smv.False));
                        else rules += String.Format(Smv.DefineBlock, Smv.ModuleParameters.Event(ev.Value.Name) + "_reset", rule);
                    }
                    else
                    {
                        string rule = String.Format("\t({0} & ({1})) | ({2})", Smv.Alpha, priorityResetRule.Trim(Smv.OrTrimChars), commonResetRule);
                        if (useProcesses) rules += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Value.Name), rule + String.Format(setValue, Smv.False));
                        else rules += String.Format(Smv.DefineBlock, Smv.ModuleParameters.Event(ev.Value.Name) + "_reset", rule);
                    }

                    priorityResetRule += Smv.ModuleParameters.Event(ev.Value.Name) + " | ";
                }
                return rules;
            }

            public static string InputVariablesSampleBasic(IEnumerable<Variable> variables, IEnumerable<WithConnection> withConnections)
            {

                string varChangeBlocks = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Input))
                {
                    if (!variable.IsConstant)
                    {
                        if (variable.ArraySize == 0)
                            varChangeBlocks += String.Format(Smv.NextCaseBlock, variable.Name, FbSmvCommon.VarSamplingRule(variable.Name, withConnections, true));
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                varChangeBlocks += String.Format(Smv.NextCaseBlock, variable.Name + Smv.ArrayIndex(i), FbSmvCommon.VarSamplingRule(variable.Name, withConnections, true, i));
                            }
                        }
                    }
                }
                return varChangeBlocks;
            }

            private static string _modulo_range(string statement, int rangeBegin, int rangeEnd)//y= ((x-c) mod d) + c
            {
                int c = (rangeBegin + rangeEnd)/2;
                int d = (rangeEnd - rangeBegin)/2 + 1;
                return String.Format("(({0} - {1}) mod {2}) + {1}", statement, c, d);
            }

            private static string _getVarChangingRules(Variable variable, IEnumerable<AlgorithmLine> currrentVarLines, IEnumerable<ECAction> actions, Settings settings)
            {
                string rules = "";
                foreach (AlgorithmLine line in currrentVarLines)
                {
                    foreach (ECAction action in _findActionsByAlgorithmName(actions, line.AlgorithmName))
                    {
                        string rule = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S2;
                        rule += " & ";
                        rule += Smv.EccStateVar + "=" + Smv.EccState(action.ECState);
                        rule += " & ";
                        rule += Smv.EcActionsCounterVar + "=" + action.Number;
                        rule += " & ";
                        rule += Smv.AlgStepsCounterVar + "=" + line.NI;
                        if (line.Condition != "1")
                        {
                            rule += " & ";
                            rule += "(" + Smv.ClearConditionExpr(line.Condition) + ")";
                        }
                        //string val = line.Value;
                        string val = Smv.ClearConditionExpr(line.Value);
                        if (String.Compare(val, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
                            val = Smv.False;
                        if (String.Compare(val, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
                            val = Smv.True;

                        if (variable.SmvType.GetType() != typeof(Smv.DataTypes.BoolSmvType) && settings.ModularArithmetics)
                        {
                            //string rangeStr = variable.SmvType.Split()
                            Smv.DataTypes.RangeSmvType varType = (Smv.DataTypes.RangeSmvType)variable.SmvType;
                            rule += " : (" + _modulo_range(val, varType.RangeBegin, varType.RangeEnd) + ");\n"; ;
                        }
                        else
                        {
                            rule += " : (" + val + ");\n";
                        }
                        rules += rule;
                    }

                }
                return rules;
            }
            public static string OutputVariablesChangingRules(IEnumerable<Variable> variables, IEnumerable<ECAction> actions, IEnumerable<AlgorithmLine> lines, Settings settings)
            {
                string varChangeBlocks = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Output || v.Direction == Direction.Internal))
                {
                    if (variable.ArraySize == 0)
                    {
                        IEnumerable<AlgorithmLine> currrentVarLines = lines.Where(line => line.Variable == variable.Name);
                        string rules = _getVarChangingRules(variable, currrentVarLines, actions, settings);
                        varChangeBlocks += String.Format(Smv.NextCaseBlock, variable.Name, rules);
                    }
                    else
                    {
                        for (int i = 0; i < variable.ArraySize; i++)
                        {
                            string arrayElementVar = String.Format("{0}[{1}]", variable.Name, i);
                            IEnumerable<AlgorithmLine> currrentVarLines = lines.Where(line => line.Variable == arrayElementVar);

                            string rules = _getVarChangingRules(variable, currrentVarLines, actions, settings);
                            varChangeBlocks += String.Format(Smv.NextCaseBlock, variable.Name + Smv.ArrayIndex(i), rules);
                        }
                    }
                }
                return varChangeBlocks;
            }
            public static string OutputEventsSettingRules(IEnumerable<Event> events, IEnumerable<ECAction> actions, bool useProcesses)
            {
                string eventChangeString = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Output))
                {
                    string rule = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S2 + " & " + Smv.AlgStepsCounterVar + "=0" + " & ({0})";
                    const string setValue = " : {0};\n";
                    string outCond = "";
                    bool eventSignalSet = false;
                    foreach (ECAction action in actions.Where(act => act.Output == ev.Name))
                    {
                        outCond += "(" + Smv.EccStateVar + "=" + Smv.EccState(action.ECState) + " & " + Smv.EcActionsCounterVar + "=" + action.Number + ") | ";
                        eventSignalSet = true;
                    }
                    if (eventSignalSet)
                    {
                        rule = String.Format(rule, outCond.TrimEnd(Smv.OrTrimChars));
                        if (useProcesses) eventChangeString += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name), rule + String.Format(setValue, Smv.True));
                        else eventChangeString += String.Format(Smv.DefineBlock, Smv.ModuleParameters.Event(ev.Name) + "_set", rule);
                    }
                    else
                    {
                        if (useProcesses) eventChangeString += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name), "");
                        else eventChangeString += String.Format(Smv.DefineBlock, Smv.ModuleParameters.Event(ev.Name) + "_set", Smv.False);
                    }
                }
                return eventChangeString;
            }
            public static string SetOutputVarBuffers(IEnumerable<Variable> variables, IEnumerable<Event> events, IEnumerable<ECAction> actions, IEnumerable<WithConnection> withConnections, ShowMessageDelegate showMessage)
            {
                string outVarsChangeString = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Output))
                {
                    string rule = Smv.OsmStateVar + "=" + Smv.Osm.S2 + " & " + Smv.AlgStepsCounterVar + "=0" +
                                  " & ({0}) : {1};\n";
                    string outCond = "";

                    List<ECAction> ac = new List<ECAction>();
                    foreach (WithConnection connection in withConnections.Where(conn => conn.Var == variable.Name))
                    {
                        var outputEventsActions = actions.Where(act => act.Output == connection.Event);
                        if (outputEventsActions.Count() == 0)
                            showMessage(String.Format("Warning! Event {0} associated with output {1} never fires.", connection.Event, variable.Name));
                        foreach (ECAction action in outputEventsActions)
                        {
                            if (!ac.Contains(action)) ac.Add(action);
                        }
                    }

                    foreach (ECAction action in ac)
                    {
                        outCond += "(" + Smv.EccStateVar + "=" + Smv.EccState(action.ECState) + " & " +
                                   Smv.EcActionsCounterVar + "=" + action.Number + ") | ";
                    }

                    if (outCond != "")
                    {
                        rule = String.Format(rule, outCond.TrimEnd(Smv.OrTrimChars), variable.Name);
                        outVarsChangeString += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Variable(variable.Name), rule);
                    }
                    else showMessage(String.Format("Warning! No active events associated with output {0}.", variable.Name));
                }
                return outVarsChangeString;
            }
            public static string SetServiceSignals(bool useProcesses)
            {
                string ruleTemplate = "\t({0} & {1}={2} & !{3} | {1}={4} & {5})";
                string rule = String.Format(ruleTemplate,
                    Smv.Alpha,
                    Smv.OsmStateVar,
                    Smv.Osm.S0,
                    Smv.ExistsInputEvent,
                    Smv.Osm.S1,
                    Smv.AbsentsEnabledEcTran
                    );
                if (useProcesses)
                {
                    return String.Format(Smv.NextCaseBlock, Smv.Beta, rule + " : " + Smv.True + ";\n")
                           + String.Format(Smv.NextCaseBlock, Smv.Alpha, rule + " : " + Smv.False + ";\n");
                }
                else
                {
                    return String.Format(Smv.DefineBlock, "alpha_reset", rule) + String.Format(Smv.DefineBlock, "beta_set", rule); ;
                }
            }
            public static string BasicModuleDefines(IEnumerable<ECState> states, IEnumerable<Event> events, IEnumerable<ECTransition> transitions, bool showUnconditionalTransitions, bool NDT=false)
            {
                string ecTran = "";
                foreach (ECState state in states)
                {
                    IEnumerable<ECTransition> stateTrans = transitions.Where(t => t.Source == state.Name);
                    if (!stateTrans.Any()) continue;

                    ecTran += "(";
                    ecTran += Smv.EccStateVar + "=" + Smv.EccState(state.Name);

                    string transitionRules = "";
                    foreach (var transition in stateTrans)
                    {
                        transitionRules += "(";
                        if (transition.Condition == null || transition.Condition == "1")
                        {
                            if (!showUnconditionalTransitions)
                            {
                                transitionRules = null;
                                break;
                            }
                            else transitionRules += "1";

                        }
                        else
                        {
                            transitionRules += _translateEventNames(Smv.ClearConditionExpr(transition.Condition), events);
                        }

                        transitionRules += ") | ";
                    }
                    if (transitionRules != null) ecTran += "  & (" + transitionRules.TrimEnd(Smv.OrTrimChars) + ")";
                    ecTran += ") | ";
                }

                string existsEnabledECTran = String.Format("DEFINE {0}:= {1};\n", Smv.ExistsEnabledEcTran, ecTran.Trim(Smv.OrTrimChars));
                string absentsEnabledECTran = "\n";

                //string alphabeta = "--alpha/beta\nDEFINE alpha_beta := ( (alpha & S_smv=s0_osm & !ExistsInputEvent | S_smv=s1_osm & (!ExistsEnabledECTran)) );\n";
                return FbSmvCommon.DefineExistsInputEvent(events,NDT) + existsEnabledECTran + absentsEnabledECTran; // + alphabeta;
            }

            private static string _translateEventNames(string str, IEnumerable<Event> events)
            {
                Console.WriteLine("\n\n" + str);

                Regex evNamesRegex = new Regex(@"(\w+)");

                string[] strSplit = evNamesRegex.Split(str);

                for (int i = 0; i < strSplit.Count(); i++)
                {
                    var foundEvent = events.FirstOrDefault(ev => ev.Name == strSplit[i]);
                    if (foundEvent != null)
                    {
                        strSplit[i] = Smv.ModuleParameters.Event(foundEvent.Name);
                    }
                }
                return String.Concat(strSplit);
            }
            private static void _addCounterRules(ref string rule1, ref string rule2, ECState state, int algsCount, ECAction action)
            {
                string add = "(";
                add += Smv.EccStateVar + "=" + Smv.EccState(state.Name);
                add += " & ";
                add += Smv.EcActionsCounterVar + " = " + (action.Number);
                //(state.ActionsCount > 0 ? state.ActionsCount : 1);
                add += " & ";
                add += Smv.AlgStepsCounterVar + " {0} " + (algsCount > 0 ? algsCount : 1);
                add += ") | ";
                rule1 += String.Format(add, "<");
                rule2 += String.Format(add, "=");
            }
            private static IEnumerable<ECAction> _findActionsByAlgorithmName(IEnumerable<ECAction> actions, string algorithmName)
            {
                return actions.Where(act => act.Algorithm == algorithmName);
            }

        }

    }
}
