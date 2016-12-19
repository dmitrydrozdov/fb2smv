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
        internal class BasicFbSmvTrans
        {
            static Smv Smv = new Smv(new nuXmvtransPattern());

            static string transOrBlockTemplate = "| ( {0} )\n";
            const string modFormatSuffix = "(({0}) mod {1}) ";

            public static string GenerateSMVCode(FBType fbType,
                                                    Storage storage,
                                                    IEnumerable<ExecutionModel> executionModels,
                                                    Settings settings,
                                                    ShowMessageDelegate showMessage,
                                                    bool showUnconditionalTransitions,
                                                    bool eventSignalResetSolve
                                                )
            {
                string smvModule = "";
                ExecutionModel executionModel = executionModels.FirstOrDefault(em => em.FBTypeName == fbType.Name);
                var events = storage.Events.Where(ev => ev.FBType == fbType.Name);
                var variables = storage.Variables.Where(ev => ev.FBType == fbType.Name);
                var states = storage.EcStates.Where(ev => ev.FBType == fbType.Name);
                var algorithms = storage.Algorithms.Where(alg => alg.FBType == fbType.Name && alg.Language == AlgorithmLanguages.ST);
                var smvAlgs = _translateAlgorithms(algorithms);
                var actions = storage.EcActions.Where(act => act.FBType == fbType.Name);
                var withConnections = storage.WithConnections.Where(conn => conn.FBType == fbType.Name);
                var transitions = storage.EcTransitions.Where(tr => tr.FBType == fbType.Name);

                smvModule += BasicFbSmv.ModuleHeader(events, variables, fbType.Name);

                smvModule += BasicFbSmv.OsmStatesDeclaration();
                smvModule += BasicFbSmv.EccStatesDeclaration(states) + "\n";
                smvModule += BasicFbSmv.EcActionsCounterDeclaration(states);
                smvModule += BasicFbSmv.AlgStepsCounterDeclaration(smvAlgs);

                smvModule += Smv.InitStatement;
                smvModule += String.Format(Smv.Eq, Smv.EccStateVar, Smv.EccState(states.First(s => true).Name)) + Smv.And
                            + String.Format(Smv.Eq, Smv.OsmStateVar, Smv.Osm.S0) + Smv.And
                            + String.Format(Smv.Eq, Smv.EcActionsCounterVar, "0") + Smv.And
                            + String.Format(Smv.Eq, Smv.AlgStepsCounterVar, "0");
                smvModule += ModuleVariablesInitBlock(variables) + ";\n";

                smvModule += Smv.Trans;

                //smvModule += "(" + ModuleTransitionRelation() + ")";
                smvModule += "(";
                smvModule += BasicOsmTransitions();
                smvModule += EcStateChangeTransitions(transitions, events);
                smvModule += OsmRollBack();

                smvModule += EcActionsCounterChangeBlock(states) + "\n";
                smvModule += AlgStepsCounterChangeBlock(states, actions, smvAlgs) + "\n";

                smvModule += DeadlockHandlingBlock();

                smvModule += ")\n";
                /*smvModule += Smv.Assign;
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.EccStateVar, Smv.EccState(states.First(s => true).Name));
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.OsmStateVar, Smv.Osm.S0);
                smvModule += BasicFbSmv.ModuleVariablesInitBlock(variables) + "\n";
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.EcActionsCounterVar, "0");
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.AlgStepsCounterVar, "0");

                smvModule += BasicFbSmv.EcStateChangeBlock(transitions, events);
                smvModule += Smv.OsmStateChangeBlock + "\n";
                smvModule += BasicFbSmv.EcActionsCounterChangeBlock(states) + "\n";
                smvModule += BasicFbSmv.AlgStepsCounterChangeBlock(states, actions, smvAlgs) + "\n";

                smvModule += BasicFbSmv.InputVariablesSampleBasic(variables, withConnections) + "\n";
                smvModule += BasicFbSmv.OutputVariablesChangingRules(variables, actions, storage.AlgorithmLines.Where(line => line.FBType == fbType.Name), settings) + "\n";
                smvModule += BasicFbSmv.SetOutputVarBuffers(variables, events, actions, withConnections, showMessage) + "\n";*/
                smvModule += BasicFbSmv.SetServiceSignals(settings.UseProcesses) + "\n";

                smvModule += BasicFbSmv.EventInputsResetRules(events, executionModel, eventSignalResetSolve, settings.UseProcesses) + "\n";
                smvModule += BasicFbSmv.OutputEventsSettingRules(events, actions, settings.UseProcesses) + "\n";

                smvModule += BasicFbSmv.BasicModuleDefines(states, events, transitions, showUnconditionalTransitions) + "\n";

                return smvModule;
            }

            private static IEnumerable<TranslatedAlg> _translateAlgorithms(IEnumerable<Algorithm> algorithms) //TODO: put translated algorithms to storage
            {
                var smvAlgs = new List<TranslatedAlg>();
                foreach (Algorithm alg in algorithms)
                {
                    TranslatedAlg translated = new TranslatedAlg(alg.Name, Translator.Translate(alg.Text));
                    if (translated.Lines.Any()) smvAlgs.Add(translated);
                }
                return smvAlgs;
            }

            private static string OsmRollBack()
            {
                string rollbackRule = Smv.OsmStateVar + " = " + Smv.Osm.S2 + Smv.And +
                                        Smv.EcActionsCounterVar + " = 0 " + Smv.And +
                                        CommonVariablesNext(Smv.EccStateVar, Smv.Osm.S1, Smv.EcActionsCounterVar, Smv.AlgStepsCounterVar);

                return String.Format(transOrBlockTemplate, rollbackRule);
            }

            private static string BasicOsmTransitions()
            {
                string returnStr = "";
                //(alpha & S_smv=s0_osm & ExistsInputEvent    & next...
                string condition = Smv.Alpha + Smv.And
                                 + String.Format(Smv.Eq, Smv.OsmStateVar, Smv.Osm.S0) + Smv.And
                                 + Smv.ExistsInputEvent;
                returnStr += "(";
                returnStr += condition + Smv.And
                           + CommonVariablesNext(Smv.EccStateVar, Smv.Osm.S1, Smv.EcActionsCounterVar, Smv.AlgStepsCounterVar)
                           + algVarsNextTransitions();
                returnStr += ")\n";

                //(S_smv=s1_osm & (!ExistsEnabledECTran)	  & next ....
                condition = String.Format(Smv.Eq, Smv.OsmStateVar, Smv.Osm.S1) + Smv.And
                          + Smv.NotFunc(Smv.ExistsEnabledEcTran);
                returnStr += "| (";
                returnStr += condition + Smv.And
                           + CommonVariablesNext(Smv.EccStateVar, Smv.Osm.S0, "1", "1")
                           + algVarsNextTransitions();
                returnStr += ")\n";

                return returnStr;
            }

            private static string CommonVariablesNext(string ecState, string osmState, string na, string ni)
            {
                string returnStr = "";
                returnStr += String.Format(Smv.NextVarAssignment, Smv.EccStateVar, ecState) + Smv.And
                           + String.Format(Smv.NextVarAssignment, Smv.OsmStateVar, osmState) + Smv.And
                           + String.Format(Smv.NextVarAssignment, Smv.EcActionsCounterVar, na) + Smv.And
                           + String.Format(Smv.NextVarAssignment, Smv.AlgStepsCounterVar, ni);

                return returnStr;
            }
            private static string algVarsNextTransitions()
            {
                //TODO: implement
                return "";
            }

            public static string EcStateChangeTransitions(IEnumerable<ECTransition> transitions, IEnumerable<Event> events)
            {
                string ecTransitionsSmv = "";

                foreach (var transition in transitions)
                {
                    string condition = Smv.EccStateVar + "=" + Smv.EccState(transition.Source) + Smv.And
                                     + Smv.OsmStateVar + "=" + Smv.Osm.S1;

                    if (transition.Condition != null && transition.Condition != "1")
                    {
                        condition += Smv.And + _translateEventNames(Smv.ClearConditionExpr(transition.Condition), events);
                    }

                    

                    ecTransitionsSmv += "| (";
                    ecTransitionsSmv += condition + Smv.And 
                                      + CommonVariablesNext(Smv.EccState(transition.Destination), Smv.Osm.S2, "1", "1")
                                      + algVarsNextTransitions();

                    ecTransitionsSmv += ")\n";
                }
                return ecTransitionsSmv;
            }

            public static string EcActionsCounterChangeBlock(IEnumerable<ECState> states)
            {

                string res = "";
                

                string actionZeroRule = Smv.OsmStateVar + "=" + Smv.Osm.S2 + Smv.And + Smv.AlgStepsCounterVar + "=0" + Smv.And + "(";

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

                actionZeroRule += rule2.TrimEnd(Smv.OrTrimChars);
                actionZeroRule += ")" + Smv.And;
                actionZeroRule += CommonVariablesNext(Smv.EccStateVar, Smv.OsmStateVar, "0", Smv.AlgStepsCounterVar);


                string actionIncRule = Smv.OsmStateVar + "=" + Smv.Osm.S2 + Smv.And + Smv.AlgStepsCounterVar + "=0" + Smv.And + "(";

                actionIncRule += rule1.TrimEnd(Smv.OrTrimChars);
                actionIncRule += ")" + Smv.And;
                string naIncValue = String.Format(modFormatSuffix, Smv.EcActionsCounterVar + " + 1", states.Max(state => state.ActionsCount) + 1); // (NA + 1) mod (max(NA)+1)
                actionIncRule += CommonVariablesNext(Smv.EccStateVar, Smv.OsmStateVar, naIncValue, Smv.AlgStepsCounterVar);


                res += String.Format(transOrBlockTemplate, actionZeroRule);
                res += String.Format(transOrBlockTemplate, actionIncRule);

                return res;
            }
            public static string AlgStepsCounterChangeBlock(IEnumerable<ECState> states, IEnumerable<ECAction> actions, IEnumerable<TranslatedAlg> algorithms)
            {
                

                string res = "";

                string algStepZeroRule = Smv.OsmStateVar + "=" + Smv.Osm.S2 + Smv.And + "(";
                string algStepIncRule = Smv.OsmStateVar + "=" + Smv.Osm.S2 + Smv.And + "(";
                //if (!algorithms.Any()) return "";
                /*string rules = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S1 + ": 1;\n";
                string rformat = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S2 + " & ({0}):";
                const string modFormatSuffix = "({1}) mod {2};\n";
                const string normalFormatSuffix = "{1};\n";*/
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

                algStepZeroRule += rule2.TrimEnd(Smv.OrTrimChars);
                algStepZeroRule += ")" + Smv.And;
                algStepZeroRule += CommonVariablesNext(Smv.EccStateVar, Smv.OsmStateVar, Smv.EcActionsCounterVar, "0");

                algStepIncRule += rule1.TrimEnd(Smv.OrTrimChars);
                algStepIncRule += ")" + Smv.And;
                string niIncValue = String.Format(modFormatSuffix, Smv.EcActionsCounterVar + " + 1", maxAlgStepsCount + 1); // (NI + 1) mod (max(NI)+1)
                algStepIncRule += CommonVariablesNext(Smv.EccStateVar, Smv.OsmStateVar, Smv.EcActionsCounterVar, niIncValue);
                /*rules += String.Format(rformat + modFormatSuffix, rule1.TrimEnd(Smv.OrTrimChars), Smv.AlgStepsCounterVar + " + 1", maxAlgStepsCount + 1);
                rules += String.Format(rformat + normalFormatSuffix, rule2.TrimEnd(Smv.OrTrimChars), " 0 ");

                return String.Format(Smv.NextCaseBlock, Smv.AlgStepsCounterVar, rules);*/
                res += String.Format(transOrBlockTemplate, algStepZeroRule);
                res += String.Format(transOrBlockTemplate, algStepIncRule);

                return res;
            }
            public static string DeadlockHandlingBlock()
            {
                string res = "";
                string noInputEventsRule = Smv.OsmStateVar + "=" + Smv.Osm.S0 + 
                                            Smv.And + String.Format("({0})",Smv.Not + Smv.ExistsInputEvent) +
                                            Smv.And + CommonVariablesNext(Smv.EccStateVar, Smv.OsmStateVar, Smv.EcActionsCounterVar, Smv.AlgStepsCounterVar);
                string noAlphaRule = Smv.OsmStateVar + "=" + Smv.Osm.S0 +
                                            Smv.And + String.Format("({0})", Smv.Not + Smv.Alpha) +
                                            Smv.And + CommonVariablesNext(Smv.EccStateVar, Smv.OsmStateVar, Smv.EcActionsCounterVar, Smv.AlgStepsCounterVar);
                res += String.Format(transOrBlockTemplate, noInputEventsRule);
                res += String.Format(transOrBlockTemplate, noAlphaRule);
                return res;
            }

            public static string ModuleVariablesInitBlock(IEnumerable<Variable> variables)
            {
                string varsInit = "";
                foreach (var variable in variables)
                {
                    if (!variable.IsConstant)
                    {
                        if (variable.ArraySize == 0)
                        {
                            varsInit += Smv.And;
                            varsInit += String.Format(Smv.Eq, variable.Name, Smv.InitialValue(variable));
                        }
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                varsInit += Smv.And;
                                varsInit += String.Format(Smv.Eq, variable.Name + Smv.ArrayIndex(i), Smv.InitialValue(variable));
                            }
                        }
                    }
                }
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
                int c = (rangeBegin + rangeEnd) / 2;
                int d = (rangeEnd - rangeBegin) / 2 + 1;
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
            public static string BasicModuleDefines(IEnumerable<ECState> states, IEnumerable<Event> events, IEnumerable<ECTransition> transitions, bool showUnconditionalTransitions)
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
                return FbSmvCommon.DefineExistsInputEvent(events) + existsEnabledECTran + absentsEnabledECTran; // + alphabeta;
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
