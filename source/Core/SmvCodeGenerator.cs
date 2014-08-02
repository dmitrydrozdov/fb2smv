using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using FB2SMV.FBCollections;
using FB2SMV.ST;
using FB2SMV.ServiceClasses;

namespace FB2SMV
{
    namespace Core
    {
        
        public class SmvCodeGenerator
        {
            public SmvCodeGenerator(Storage storage)
            {
                _storage = storage;
            }

            public int fbTypeCompare(FBType a, FBType b)
            {
                if (a.Type == FBClass.Basic && b.Type == FBClass.Composite) return -1;
                else if (a.Type == FBClass.Composite && b.Type == FBClass.Basic) return 1;
                else return 0;
            }

            public IEnumerable<string> TranslateAll()
            {
                List<string> blocks = new List<string>();
                _storage.Types.Sort(fbTypeCompare);
                foreach (FBType type in _storage.Types)
                {
                    blocks.Add(translateFB(type));
                }
                return blocks;
            }

            public string translateFB(FBType fbType)
            {
                if (fbType.Type == FBClass.Basic) return TranslateBasicFB(fbType);
                else return TranslateCompositeFB(fbType);
            }

            public string TranslateCompositeFB(FBType fbType)
            {
                string smvModule = "";

                var events = _storage.Events.Where(ev => ev.FBType == fbType.Name);
                var variables = _storage.Variables.Where(ev => ev.FBType == fbType.Name);
                var instances = _storage.Instances.Where(inst => inst.FBType == fbType.Name);
                var withConnections = _storage.WithConnections.Where(conn => conn.FBType == fbType.Name);
                var internalBuffers = _storage.Connections.Where(conn => conn.FBType == fbType.Name);

                bool solveDispatchingProblem = true;
                IDispatcher dispatcher = new CyclicDispatcher(instances, solveDispatchingProblem);

                //smvModule += _moduleHeader(events, variables, fbType.Name) + "\n";
                smvModule += FbSmvCommon.SmvModuleDeclaration(events, variables, fbType.Name);
                smvModule += CompositeFbSmv.FbInstances(instances, _storage.Events, _storage.Variables) + "\n";
                smvModule += CompositeFbSmv.InternalBuffersDeclaration(instances, _storage.Events, _storage.Variables) + "\n";
                smvModule += CmSmv.Assign;
                smvModule += CompositeFbSmv.InternalBuffersInitialization(instances, _storage.Events, _storage.Variables) + "\n";
                //smvModule += _moduleVariablesInitBlock(variables) + "\n";
                //smvModule += _inputVariablesSampleComposite(variables, withConnections) + "\n";
                smvModule += CompositeFbSmv.InternalEventConnections(internalBuffers) + "\n";
                smvModule += CompositeFbSmv.InternalDataConnections(internalBuffers, withConnections) + "\n";
                smvModule += CompositeFbSmv.ResetComponentEventOutputs(internalBuffers) + "\n";
                //smvModule += _eventInputsResetRules(events) + "\n";
                smvModule += CompositeFbSmv.InputEventsResetRules(events);
                smvModule += "\n-- ---DISPATCHER--- --\n";
                smvModule += "-- *************** --\n";
                smvModule += dispatcher.GetSmvCode() + "\n";


                smvModule += FbSmvCommon.DefineExistsInputEvent(events) + "\n";
                smvModule += CompositeFbSmv.DefineOmega(internalBuffers) + "\n";
                smvModule += FbSmvCommon.ModuleFooter() + "\n";
                //smvModule += Smv.AlphaBetaRules;

                return smvModule;
            }

            public string TranslateBasicFB(FBType fbType, bool eventSignalResetSolve = true, bool showUnconditionalTransitions = false)
            {
                string smvModule = "";

                var events = _storage.Events.Where(ev => ev.FBType == fbType.Name);
                var variables = _storage.Variables.Where(ev => ev.FBType == fbType.Name);
                var states = _storage.EcStates.Where(ev => ev.FBType == fbType.Name);
                var algorithms = _storage.Algorithms.Where(alg => alg.FBType == fbType.Name && alg.Language == AlgorithmLanguages.ST);
                var smvAlgs = _translateAlgorithms(algorithms);
                var actions = _storage.EcActions.Where(act => act.FBType == fbType.Name);
                var withConnections = _storage.WithConnections.Where(conn => conn.FBType == fbType.Name);
                var transitions = _storage.EcTransitions.Where(tr => tr.FBType == fbType.Name);

                smvModule += BasicFbSmv.ModuleHeader(events, variables, fbType.Name);

                smvModule += BasicFbSmv.OsmStatesDeclaration();
                smvModule += BasicFbSmv.EccStatesDeclaration(states) + "\n";
                smvModule += BasicFbSmv.EcActionsCounterDeclaration(states);
                smvModule += BasicFbSmv.AlgStepsCounterDeclaration(smvAlgs);

                smvModule += CmSmv.Assign;
                smvModule += String.Format(CmSmv.VarInitializationBlock, CmSmv.EccStateVar, CmSmv.EccState(states.First(s => true).Name));
                smvModule += String.Format(CmSmv.VarInitializationBlock, CmSmv.OsmStateVar, CmSmv.Osm.S0);
                smvModule += BasicFbSmv.ModuleVariablesInitBlock(variables) + "\n";
                smvModule += String.Format(CmSmv.VarInitializationBlock, CmSmv.EcActionsCounterVar, "0");
                smvModule += String.Format(CmSmv.VarInitializationBlock, CmSmv.AlgStepsCounterVar, "0");

                smvModule += BasicFbSmv.EcStateChangeBlock(transitions, events);
                smvModule += CmSmv.OsmStateChangeBlock + "\n";
                smvModule += BasicFbSmv.EcActionsCounterChangeBlock(states) + "\n";
                smvModule += BasicFbSmv.AlgStepsCounterChangeBlock(states, actions, smvAlgs) + "\n";
                smvModule += BasicFbSmv.EventInputsResetRules(events, eventSignalResetSolve) + "\n";
                smvModule += BasicFbSmv.InputVariablesSampleBasic(variables, withConnections) + "\n";
                smvModule += BasicFbSmv.OutputVariablesChangingRules(variables, actions, _storage.AlgorithmLines.Where(line => line.FBType == fbType.Name)) + "\n";
                smvModule += BasicFbSmv.OutputEventsSettingRules(events, actions) + "\n";
                smvModule += BasicFbSmv.SetOutputVarBuffers(variables, events, actions, withConnections) + "\n";
                smvModule += BasicFbSmv.SetServiceSignals() + "\n";
                smvModule += BasicFbSmv.BasicModuleDefines(states, events, transitions, showUnconditionalTransitions) + "\n";
                smvModule += FbSmvCommon.ModuleFooter() + "\n";
                return smvModule;
            }

            private IEnumerable<TranslatedAlg> _translateAlgorithms(IEnumerable<Algorithm> algorithms) //TODO: put translated algorithms to storage
            {
                var smvAlgs = new List<TranslatedAlg>();
                foreach (Algorithm alg in algorithms)
                {
                    TranslatedAlg translated = new TranslatedAlg(alg.Name, Translator.Translate(alg.Text));
                    if (translated.Lines.Any()) smvAlgs.Add(translated);
                }
                return smvAlgs;
            }

            public List<string> BasicBlocks = new List<string>();
            private Storage _storage;
        }
        internal class FbSmvCommon
        {
            public static string ModuleParametersString(IEnumerable<Event> events, IEnumerable<Variable> variables, string preffix = null)
            {
                string moduleParameters = "";
                foreach (Event ev in events)
                {
                    if (preffix == null)
                        moduleParameters += (CmSmv.ModuleParameters.Event(ev.Name) + CmSmv.ModuleParameters.Splitter);
                    else moduleParameters += (preffix + "_" + ev.Name + CmSmv.ModuleParameters.Splitter);
                }
                foreach (Variable variable in variables)
                {
                    if (variable.Direction != Direction.Internal)
                    {
                        if (preffix == null)
                            moduleParameters += (CmSmv.ModuleParameters.Variable(variable.Name) +
                                                 CmSmv.ModuleParameters.Splitter);
                        else moduleParameters += (preffix + "_" + variable.Name + CmSmv.ModuleParameters.Splitter);
                    }
                }
                if (preffix == null)
                {
                    moduleParameters += CmSmv.Alpha + CmSmv.ModuleParameters.Splitter;
                    moduleParameters += CmSmv.Beta + CmSmv.ModuleParameters.Splitter;
                }
                else
                {
                    moduleParameters += preffix + "_" + CmSmv.Alpha + CmSmv.ModuleParameters.Splitter;
                    moduleParameters += preffix + "_" + CmSmv.Beta + CmSmv.ModuleParameters.Splitter;
                }
                return moduleParameters.TrimEnd(CmSmv.ModuleParameters.Splitter.ToCharArray());
            }
            public static string VarSamplingRule(string varName, IEnumerable<WithConnection> withConnections, bool basic)
            {
                string samplingEvents = "";
                //string rules = "";
                foreach (WithConnection connection in withConnections.Where(conn => conn.Var == varName))
                {
                    samplingEvents += CmSmv.ModuleParameters.Event(connection.Event) + " | ";
                }
                string rule = "\t" + CmSmv.Alpha;
                if (basic) rule += " & " + CmSmv.OsmStateVar + "=" + CmSmv.Osm.S0;
                if (samplingEvents != "")
                    rule += String.Format(" & ({0})", samplingEvents.Trim(CmSmv.OrTrimChars));
                rule += " : " + CmSmv.ModuleParameters.Variable(varName) + " ;\n";
                return rule;
            }
            public static string DefineExistsInputEvent(IEnumerable<Event> events)
            {
                string inputEvents = events.Where(ev => ev.Direction == Direction.Input).Aggregate("", (current, ev) => current + (CmSmv.ModuleParameters.Event(ev.Name) + " | "));
                return String.Format(CmSmv.DefineBlock, CmSmv.ExistsInputEvent, inputEvents.Trim(CmSmv.OrTrimChars));
            }
            public static string ModuleFooter()
            {
                return "\n" + CmSmv.FairnessRunning;
            }
            public static string SmvModuleDeclaration(IEnumerable<Event> events, IEnumerable<Variable> variables, string fbTypeName)
            {

                return String.Format(CmSmv.ModuleDef, fbTypeName, FbSmvCommon.ModuleParametersString(events, variables));
            }
        }
        internal class CompositeFbSmv
        {
            public static string FbInstances(IEnumerable<FBInstance> instances, IEnumerable<Event> nonFilteredEvents, IEnumerable<Variable> nonFilteredVariables)
            {
                string inst = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType);
                    var instanceVars = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType);
                    string instImplementation = instance.InstanceType + " (" + FbSmvCommon.ModuleParametersString(instanceEvents, instanceVars, instance.Name) + ")";
                    inst += String.Format(CmSmv.VarDeclarationBlock, instance.Name, "process " + instImplementation);
                }
                return inst;
            }
            public static string InternalBuffersDeclaration(IEnumerable<FBInstance> instances, IEnumerable<Event> nonFilteredEvents, IEnumerable<Variable> nonFilteredVariables)
            {
                string buffers = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal);
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);
                    foreach (Event ev in instanceEvents)
                    {
                        buffers += String.Format(CmSmv.VarDeclarationBlock, instance.Name + "_" + ev.Name,
                            CmSmv.DataTypes.BoolType);
                    }
                    foreach (Variable variable in instanceVariables)
                    {
                        if (variable.ArraySize == 0)
                            buffers += String.Format(CmSmv.VarDeclarationBlock, instance.Name + "_" + variable.Name,
                                CmSmv.DataTypes.GetType(variable.Type));
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                buffers += String.Format(CmSmv.VarDeclarationBlock,
                                    instance.Name + "_" + variable.Name + "[" + i + "]",
                                    CmSmv.DataTypes.GetType(variable.Type));
                            }
                        }
                    }
                    buffers += String.Format(CmSmv.VarDeclarationBlock, instance.Name + "_" + CmSmv.Alpha,
                        CmSmv.DataTypes.BoolType);
                    buffers += String.Format(CmSmv.VarDeclarationBlock, instance.Name + "_" + CmSmv.Beta,
                        CmSmv.DataTypes.BoolType);
                    buffers += "\n";
                }
                return buffers;
            }
            public static string InternalBuffersInitialization(IEnumerable<FBInstance> instances, IEnumerable<Event> nonFilteredEvents, IEnumerable<Variable> nonFilteredVariables)
            {
                string buffersInit = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal);
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);
                    foreach (Event ev in instanceEvents)
                    {
                        buffersInit += String.Format(CmSmv.VarInitializationBlock, instance.Name + "_" + ev.Name, CmSmv.False);
                    }
                    foreach (Variable variable in instanceVariables)
                    {
                        if (variable.ArraySize == 0)
                            buffersInit += String.Format(CmSmv.VarInitializationBlock, instance.Name + "_" + variable.Name, CmSmv.InitialValue(variable));
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                buffersInit += String.Format(CmSmv.VarInitializationBlock, instance.Name + "_" + variable.Name + "[" + i + "]", CmSmv.InitialValue(variable));
                            }
                        }
                    }
                    buffersInit += String.Format(CmSmv.VarInitializationBlock, instance.Name + "_" + CmSmv.Alpha, CmSmv.False);
                    buffersInit += String.Format(CmSmv.VarInitializationBlock, instance.Name + "_" + CmSmv.Beta, CmSmv.False);
                    buffersInit += "\n";
                }
                return buffersInit;
            }
            public static string InternalEventConnections(IEnumerable<Connection> internalBuffers)
            {
                string eventConnections = "-- _internalEventConnections\n";
                MultiMap<string> eventConnectionsMap = _getEventConnectionsMap(internalBuffers);
                foreach (string dst in eventConnectionsMap.Keys)
                {

                    bool dstComponent;
                    string dstSmvVar = CmSmv.ConvertConnectionVariableName(dst, CmSmv.ModuleParameters.Event, out dstComponent);

                    string srcString = "\t("; //+ srcSmvVar + " : " + Smv.True + ";\n";
                    foreach (string src in eventConnectionsMap[dst])
                    {
                        bool srcComponent;
                        string srcSmvVar = CmSmv.ConvertConnectionVariableName(src, CmSmv.ModuleParameters.Event, out srcComponent);
                        if (srcComponent) srcString += srcSmvVar + " | ";
                        else srcString += String.Format("({0} & {1}) | ", srcSmvVar, CmSmv.Alpha);
                    }
                    srcString = srcString.TrimEnd(CmSmv.OrTrimChars) + ") : " + CmSmv.True + ";\n";
                    eventConnections += String.Format(CmSmv.NextCaseBlock + "\n", dstSmvVar, srcString);
                }
                return eventConnections;
            }
            public static string InternalDataConnections(IEnumerable<Connection> internalBuffers, IEnumerable<WithConnection> withConnections)
            {
                string dataConnections = "-- _internalDataConnections\n";
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Data))
                {
                    bool srcComponent;
                    bool dstComponent;
                    string dstSmvVar = CmSmv.ConvertConnectionVariableName(connection.Destination, CmSmv.ModuleParameters.Variable, out dstComponent);
                    string srcSmvVar = CmSmv.ConvertConnectionVariableName(connection.Source, CmSmv.ModuleParameters.Variable, out srcComponent);
                    string srcString = "";
                    if (srcComponent && dstComponent)
                    {
                        //srcString = "\t" + srcSmvVar + " : " + Smv.True + ";\n"; //TODO: make direct connections without double-buffering
                        dataConnections += String.Format(CmSmv.NextVarAssignment + "\n", dstSmvVar, srcSmvVar);
                    }
                    else if (dstComponent)
                    {
                        srcString = FbSmvCommon.VarSamplingRule(connection.Source, withConnections, false);
                        dataConnections += String.Format(CmSmv.NextCaseBlock + "\n", dstSmvVar, srcString);
                    }
                    else if (srcComponent)
                    {
                        IEnumerable<string> samplingEvents = _getSamplingEventNamesForVariable(connection.Destination, withConnections);
                        MultiMap<string> eventConnectionsMap = _getEventConnectionsMap(internalBuffers);

                        string eventSeed = "";
                        foreach (string ev in samplingEvents)
                        {
                            string src = "";
                            foreach (string parentEvent in eventConnectionsMap[ev])
                            {
                                bool dontCare;
                                src += CmSmv.ConvertConnectionVariableName(parentEvent, CmSmv.ModuleParameters.Event, out dontCare) + CmSmv.Or;
                            }
                            eventSeed += String.Format("({0}){1}", src.TrimEnd(CmSmv.OrTrimChars), CmSmv.Or);
                        }
                        srcString = String.Format("\t{0} : {1};\n", eventSeed.TrimEnd(CmSmv.OrTrimChars), srcSmvVar);
                        dataConnections += String.Format(CmSmv.NextCaseBlock + "\n", dstSmvVar, srcString);
                    }
                }
                return dataConnections;
            }
            public static string ResetComponentEventOutputs(IEnumerable<Connection> internalBuffers)
            {
                string resetString = "-- _resetComponentEventOutputs\n";
                foreach (string output in _getInternalEventOutputs(internalBuffers))
                {
                    resetString += String.Format(CmSmv.NextVarAssignment, output, CmSmv.False);
                }
                return resetString;
            }
            public static string DefineOmega(IEnumerable<Connection> internalBuffers)
            {
                string omega = "";
                foreach (string output in _getInternalEventOutputs(internalBuffers))
                {
                    omega += CmSmv.Not + output + CmSmv.Or;
                }
                return String.Format(CmSmv.DefineBlock, CmSmv.Omega, omega.TrimEnd(CmSmv.OrTrimChars));
            }
            public static string InputEventsResetRules(IEnumerable<Event> events)
            {
                string rules = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Input))
                {
                    string resetRule = "\t" + CmSmv.Alpha + " : " + CmSmv.False + ";\n";
                    rules += String.Format(CmSmv.NextCaseBlock, CmSmv.ModuleParameters.Event(ev.Name), resetRule);
                }
                return rules;
            }

            private static MultiMap<string> _getEventConnectionsMap(IEnumerable<Connection> internalBuffers)
            {
                MultiMap<string> eventConnectionsMap = new MultiMap<string>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    eventConnectionsMap.Add(connection.Destination, connection.Source);
                }
                return eventConnectionsMap;
            }
            private static IEnumerable<string> _getSamplingEventNamesForVariable(string varName, IEnumerable<WithConnection> withConnections)
            {
                List<string> output = new List<string>();
                foreach (WithConnection conn in withConnections.Where(c => c.Var == varName))
                {
                    output.Add(conn.Event);
                }
                return output;
            }
            private static IEnumerable<string> _getInternalEventOutputs(IEnumerable<Connection> internalBuffers)
            {
                HashSet<string> internalEventOutputs = new HashSet<string>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    bool srcComponent;
                    string srcSmvVar = CmSmv.ConvertConnectionVariableName(connection.Source, CmSmv.ModuleParameters.Event,
                        out srcComponent);
                    if (!internalEventOutputs.Contains(srcSmvVar))
                    {
                        if (srcComponent) internalEventOutputs.Add(srcSmvVar);
                    }
                }
                return internalEventOutputs;
            }
        }
        internal class BasicFbSmv
        {
            public static string ModuleHeader(IEnumerable<Event> events, IEnumerable<Variable> variables, string fbTypeName)
            {
                string outp = "";
                outp += FbSmvCommon.SmvModuleDeclaration(events, variables, fbTypeName);
                foreach (var variable in variables)
                {
                    if (variable.ArraySize == 0)
                        outp += String.Format(CmSmv.VarDeclarationBlock, variable.Name,
                            CmSmv.DataTypes.GetType(variable.Type));
                    else
                    {
                        for (int i = 0; i < variable.ArraySize; i++)
                        {
                            outp += String.Format(CmSmv.VarDeclarationBlock, variable.Name + "[" + i + "]",
                                CmSmv.DataTypes.GetType(variable.Type));
                        }
                    }
                }
                return outp;
            }
            public static string OsmStatesDeclaration()
            {
                return String.Format("VAR {0} : {{{1}, {2}, {3}}};\n", CmSmv.OsmStateVar, CmSmv.Osm.S0, CmSmv.Osm.S1, CmSmv.Osm.S2);
            }
            public static string EccStatesDeclaration(IEnumerable<ECState> states)
            {
                string eccStates = "";
                foreach (var ecState in states)
                {
                    eccStates += CmSmv.EccState(ecState.Name) + CmSmv.ModuleParameters.Splitter;
                }
                return String.Format("VAR {0} : {{{1}}};\n", CmSmv.EccStateVar,
                    eccStates.TrimEnd(CmSmv.ModuleParameters.Splitter.ToCharArray()));
            }
            public static string EcActionsCounterDeclaration(IEnumerable<ECState> states)
            {
                return String.Format("VAR {0}: 0..{1};\n", CmSmv.EcActionsCounterVar, states.Max(state => state.ActionsCount));
            }
            public static string AlgStepsCounterDeclaration(IEnumerable<TranslatedAlg> translatedAlgorithms)
            {
                string output = "";
                if (translatedAlgorithms.Any()) output += String.Format("VAR {0}: 0..{1};\n", CmSmv.AlgStepsCounterVar, translatedAlgorithms.Max(alg => alg.Lines.Max(line => line.NI)));
                else output += String.Format("VAR {0}: 0..{1};\n", CmSmv.AlgStepsCounterVar, "1");
                return output;
            }
            public static string EcStateChangeBlock(IEnumerable<ECTransition> transitions, IEnumerable<Event> events)
            {
                string ecTransitionsSmv = "";

                foreach (var transition in transitions)
                {
                    ecTransitionsSmv += "\t";
                    ecTransitionsSmv += CmSmv.EccStateVar + "=" + CmSmv.EccState(transition.Source);
                    ecTransitionsSmv += " & ";
                    ecTransitionsSmv += CmSmv.OsmStateVar + "=" + CmSmv.Osm.S1;

                    if (transition.Condition != null && transition.Condition != "1")
                    {
                        ecTransitionsSmv += " & " + _translateEventNames(CmSmv.ClearConditionExpr(transition.Condition), events);
                    }
                    ecTransitionsSmv += " : ";
                    ecTransitionsSmv += CmSmv.EccState(transition.Destination);
                    ecTransitionsSmv += ";\n";
                }
                return String.Format("\n" + CmSmv.NextCaseBlock + "\n", CmSmv.EccStateVar, ecTransitionsSmv);
            }
            public static string EcActionsCounterChangeBlock(IEnumerable<ECState> states)
            {
                string rules = "\t" + CmSmv.OsmStateVar + "=" + CmSmv.Osm.S1 + ": 1;\n";
                string rformat = "\t" + CmSmv.OsmStateVar + "=" + CmSmv.Osm.S2 + " & " + CmSmv.AlgStepsCounterVar +
                                 "=0 & ({0}): ";
                const string modFormatSuffix = "({1}) mod {2};\n";
                const string normalFormatSuffix = "{1};\n";

                string rule1 = "";
                string rule2 = "";
                foreach (ECState state in states)
                {
                    string add = "(";
                    add += CmSmv.EccStateVar + "=" + CmSmv.EccState(state.Name);
                    add += " & ";
                    add += CmSmv.EcActionsCounterVar + " {0} " + (state.ActionsCount > 0 ? state.ActionsCount : 1);
                    add += ") | ";
                    rule1 += String.Format(add, "<");
                    rule2 += String.Format(add, "=");
                }
                rules += String.Format(rformat + modFormatSuffix, rule1.TrimEnd(CmSmv.OrTrimChars), CmSmv.EcActionsCounterVar + " + 1", states.Max(state => state.ActionsCount) + 1);
                rules += String.Format(rformat + normalFormatSuffix, rule2.TrimEnd(CmSmv.OrTrimChars), " 0 ");

                return String.Format(CmSmv.NextCaseBlock, CmSmv.EcActionsCounterVar, rules);
            }
            public static string AlgStepsCounterChangeBlock(IEnumerable<ECState> states, IEnumerable<ECAction> actions, IEnumerable<TranslatedAlg> algorithms)
            {
                //if (!algorithms.Any()) return "";
                string rules = "\t" + CmSmv.OsmStateVar + "=" + CmSmv.Osm.S1 + ": 1;\n";
                string rformat = "\t" + CmSmv.OsmStateVar + "=" + CmSmv.Osm.S2 + " & ({0}):";
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
                        add += CmSmv.EccStateVar + "=" + CmSmv.EccState(state.Name);
                        add += " & ";
                        add += CmSmv.EcActionsCounterVar + " = 1";
                        add += " & ";
                        add += CmSmv.AlgStepsCounterVar + " {0} 1";
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
                rules += String.Format(rformat + modFormatSuffix, rule1.TrimEnd(CmSmv.OrTrimChars), CmSmv.AlgStepsCounterVar + " + 1", maxAlgStepsCount + 1);
                rules += String.Format(rformat + normalFormatSuffix, rule2.TrimEnd(CmSmv.OrTrimChars), " 0 ");

                return String.Format(CmSmv.NextCaseBlock, CmSmv.AlgStepsCounterVar, rules);
            }
            public static string ModuleVariablesInitBlock(IEnumerable<Variable> variables)
            {
                string varsInit = "-- _moduleVariablesInitBlock\n";
                foreach (var variable in variables)
                {
                    varsInit += String.Format(CmSmv.VarInitializationBlock, variable.Name, CmSmv.InitialValue(variable));
                }
                return varsInit;
            }
            public static string EventInputsResetRules(IEnumerable<Event> events, bool eventSignalResetSolve)
            {
                string rules = "";
                string commonResetRule = CmSmv.OsmStateVar + "=" + CmSmv.Osm.S1;
                if (!eventSignalResetSolve) commonResetRule += " & " + CmSmv.ExistsEnabledEcTran;
                string priorityResetRule = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Input))
                {
                    if (priorityResetRule == "")
                        rules += String.Format(CmSmv.NextCaseBlock, CmSmv.ModuleParameters.Event(ev.Name),
                            "\t" + commonResetRule + ": " + CmSmv.False + ";\n");
                    else
                    {
                        string rule = String.Format("\t({0} & ({1})) | ({2}) : {3};\n", CmSmv.Alpha,
                            priorityResetRule.Trim(CmSmv.OrTrimChars), commonResetRule, CmSmv.False);
                        rules += String.Format(CmSmv.NextCaseBlock, CmSmv.ModuleParameters.Event(ev.Name), rule);
                    }

                    priorityResetRule += CmSmv.ModuleParameters.Event(ev.Name) + " | ";
                }
                return rules;
            }
            public static string InputVariablesSampleBasic(IEnumerable<Variable> variables, IEnumerable<WithConnection> withConnections)
            {

                string varChangeBlocks = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Input))
                {
                    varChangeBlocks += String.Format(CmSmv.NextCaseBlock, variable.Name, FbSmvCommon.VarSamplingRule(variable.Name, withConnections, true));
                }
                return varChangeBlocks;
            }
            public static string OutputVariablesChangingRules(IEnumerable<Variable> variables, IEnumerable<ECAction> actions, IEnumerable<AlgorithmLine> lines)
            {
                string varChangeBlocks = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Output || v.Direction == Direction.Internal))
                {
                    string rules = "";
                    IEnumerable<AlgorithmLine> currrentVarLines = lines.Where(line => line.Variable == variable.Name);
                    foreach (AlgorithmLine line in currrentVarLines)
                    {
                        foreach (ECAction action in _findActionsByAlgorithmName(actions, line.AlgorithmName))
                        {
                            string rule = "\t" + CmSmv.OsmStateVar + "=" + CmSmv.Osm.S2;
                            rule += " & ";
                            rule += CmSmv.EccStateVar + "=" + CmSmv.EccState(action.ECState);
                            rule += " & ";
                            rule += CmSmv.EcActionsCounterVar + "=" + action.Number;
                            rule += " & ";
                            rule += CmSmv.AlgStepsCounterVar + "=" + line.NI;
                            if (line.Condition != "1")
                            {
                                rule += " & ";
                                rule += "(" + CmSmv.ClearConditionExpr(line.Condition) + ")";
                            }
                            //string val = line.Value;
                            string val = CmSmv.ClearConditionExpr(line.Value); //TODO: test this. It is an experiment
                            if (String.Compare(val, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
                                val = CmSmv.False;
                            if (String.Compare(val, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
                                val = CmSmv.True;
                            rule += " : (" + val + ");\n";
                            rules += rule;
                        }

                    }
                    varChangeBlocks += String.Format(CmSmv.NextCaseBlock, variable.Name, rules);

                }
                return varChangeBlocks;
            }
            public static string OutputEventsSettingRules(IEnumerable<Event> events, IEnumerable<ECAction> actions)
            {
                string eventChangeString = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Output))
                {
                    string rule = "\t" + CmSmv.OsmStateVar + "=" + CmSmv.Osm.S2 + " & " + CmSmv.AlgStepsCounterVar + "=0" +
                                  " & ({0}) : {1};\n";
                    string outCond = "";
                    bool eventSignalSet = false;
                    foreach (ECAction action in actions.Where(act => act.Output == ev.Name))
                    {
                        outCond += "(" + CmSmv.EccStateVar + "=" + CmSmv.EccState(action.ECState) + " & " +
                                   CmSmv.EcActionsCounterVar + "=" + action.Number + ") | ";
                        eventSignalSet = true;
                    }
                    if (eventSignalSet)
                    {
                        rule = String.Format(rule, outCond.TrimEnd(CmSmv.OrTrimChars), CmSmv.True);
                        eventChangeString += String.Format(CmSmv.NextCaseBlock, CmSmv.ModuleParameters.Event(ev.Name), rule);
                    }
                    else eventChangeString += String.Format(CmSmv.NextCaseBlock, CmSmv.ModuleParameters.Event(ev.Name), "");
                }
                return eventChangeString;
            }
            public static string SetOutputVarBuffers(IEnumerable<Variable> variables, IEnumerable<Event> events, IEnumerable<ECAction> actions, IEnumerable<WithConnection> withConnections)
            {
                string outVarsChangeString = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Output))
                {
                    string rule = CmSmv.OsmStateVar + "=" + CmSmv.Osm.S2 + " & " + CmSmv.AlgStepsCounterVar + "=0" +
                                  " & ({0}) : {1};\n";
                    string outCond = "";

                    List<ECAction> ac = new List<ECAction>();
                    foreach (WithConnection connection in withConnections.Where(conn => conn.Var == variable.Name))
                    {
                        foreach (ECAction action in actions.Where(act => act.Output == connection.Event))
                        {
                            if (!ac.Contains(action)) ac.Add(action);
                        }
                    }

                    foreach (ECAction action in ac)
                    {
                        outCond += "(" + CmSmv.EccStateVar + "=" + CmSmv.EccState(action.ECState) + " & " +
                                   CmSmv.EcActionsCounterVar + "=" + action.Number + ") | ";
                    }
                    rule = String.Format(rule, outCond.TrimEnd(CmSmv.OrTrimChars), variable.Name);
                    outVarsChangeString += String.Format(CmSmv.NextCaseBlock, CmSmv.ModuleParameters.Variable(variable.Name),
                        rule);
                }
                return outVarsChangeString;
            }
            public static string SetServiceSignals()
            {
                string ruleTemplate = "\t({0} & {1}={2} & !{3} | {1}={4} & {5}): {6};\n";
                string betaRule = String.Format(ruleTemplate,
                    CmSmv.Alpha,
                    CmSmv.OsmStateVar,
                    CmSmv.Osm.S0,
                    CmSmv.ExistsInputEvent,
                    CmSmv.Osm.S1,
                    CmSmv.AbsentsEnabledEcTran,
                    CmSmv.True
                    );
                string alphaRule = String.Format(ruleTemplate,
                    CmSmv.Alpha,
                    CmSmv.OsmStateVar,
                    CmSmv.Osm.S0,
                    CmSmv.ExistsInputEvent,
                    CmSmv.Osm.S1,
                    CmSmv.AbsentsEnabledEcTran,
                    CmSmv.False
                    );
                return String.Format(CmSmv.NextCaseBlock, CmSmv.Beta, betaRule) + String.Format(CmSmv.NextCaseBlock, CmSmv.Alpha, alphaRule);
            }
            public static string BasicModuleDefines(IEnumerable<ECState> states, IEnumerable<Event> events, IEnumerable<ECTransition> transitions, bool showUnconditionalTransitions)
            {
                string ecTran = "";
                foreach (ECState state in states)
                {
                    IEnumerable<ECTransition> stateTrans = transitions.Where(t => t.Source == state.Name);
                    if (!stateTrans.Any()) continue;

                    ecTran += "(";
                    ecTran += CmSmv.EccStateVar + "=" + CmSmv.EccState(state.Name);

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
                            transitionRules += _translateEventNames(CmSmv.ClearConditionExpr(transition.Condition), events);
                        }

                        transitionRules += ") | ";
                    }
                    if (transitionRules != null) ecTran += "  & (" + transitionRules.TrimEnd(CmSmv.OrTrimChars) + ")";
                    ecTran += ") | ";
                }

                string existsEnabledECTran = String.Format("DEFINE {0}:= {1};\n", CmSmv.ExistsEnabledEcTran, ecTran.Trim(CmSmv.OrTrimChars));
                string absentsEnabledECTran = "\n";
                return FbSmvCommon.DefineExistsInputEvent(events) + existsEnabledECTran + absentsEnabledECTran;
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
                        strSplit[i] = CmSmv.ModuleParameters.Event(foundEvent.Name);
                    }
                }
                return String.Concat(strSplit);
            }
            private static void _addCounterRules(ref string rule1, ref string rule2, ECState state, int algsCount, ECAction action)
            {
                string add = "(";
                add += CmSmv.EccStateVar + "=" + CmSmv.EccState(state.Name);
                add += " & ";
                add += CmSmv.EcActionsCounterVar + " = " + (action.Number);
                //(state.ActionsCount > 0 ? state.ActionsCount : 1);
                add += " & ";
                add += CmSmv.AlgStepsCounterVar + " {0} " + (algsCount > 0 ? algsCount : 1);
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