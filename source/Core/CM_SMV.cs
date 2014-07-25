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
        public static class Smv
        {
            public static string True = "TRUE";
            public static string False = "FALSE";
            public static string ModuleDef = "MODULE {0}({1})\n";
            public static string VarDeclarationBlock = "VAR {0} : {1};\n";
            public static string VarInitializationBlock = "init({0}):= {1};\n";
            public static string DefineBlock = "DEFINE {0}:= {1};\n";
            public static string OsmStateVar = "S_smv";
            public static string EccStateVar = "Q_smv";
            public static string EcActionsCounterVar = "NA";
            public static string AlgStepsCounterVar = "NI";
            public static string NextCaseBlock = "next({0}):= case\n{1}\t" + True + " : {0};\nesac;\n";
            public static string ExistsInputEvent = "ExistsInputEvent";
            public static string ExistsEnabledEcTran = "ExistsEnabledECTran";
            public static string AbsentsEnabledEcTran = "(!ExistsEnabledECTran)";
            public static string Alpha = "alpha";
            public static string Beta = "beta";
            public static string Omega = "omega";

            public static string NormalVarAssignment = "{0} := {1};\n";
            public static string NextVarAssignment = "next({0}) := {1};\n";
            public static char ConnectionNameSeparator = '.';

            //public static string CommonAlphaBetaRules = String.Format(NextCaseBlock, Alpha, "\t" + Beta + " : " + True) + String.Format(NextCaseBlock, Beta, "\t" + Beta + " : " + False);

            public static string And = " & ";
            public static string Or = " | ";
            public static string Not = "!";


            public static string OsmStateChangeBlock
            {
                get
                {
                    string rules = "\t" + Smv.Alpha + " & " + OsmStateVar + "=" + Osm.S0 + " & " + ExistsInputEvent +
                                   ": " + Osm.S1 + ";\n";
                    rules += "\t" + OsmStateVar + "=" + Osm.S1 + " & " + ExistsEnabledEcTran + ": " + Osm.S2 + ";\n";
                    rules += "\t" + OsmStateVar + "=" + Osm.S2 + " & " + EcActionsCounterVar + "=0 : " + Osm.S1 + ";\n";
                    rules += "\t" + OsmStateVar + "=" + Osm.S1 + " & " + AbsentsEnabledEcTran + ": " + Osm.S0 + ";\n";
                    return String.Format(NextCaseBlock, OsmStateVar, rules);
                }
            }

            public static string OsmState(string name)
            {
                return name + "_osm";
            }

            public static string EccState(string name)
            {
                return name + "_ecc";
            }

            public static string InitialValue(Variable variable)
            {
                if (variable.InitialValue != null) return variable.InitialValue;
                if (String.Compare(variable.Type, "BOOL", StringComparison.InvariantCultureIgnoreCase) == 0)
                    return "FALSE";
                return "0";
            }

            public static string ClearConditionExpr(string cond)
            {
                Regex rAnd = new Regex(@"((?<=\w)&(?=\w))|((?<=\W)(AND|and|And)(?=\W))");
                Regex rOr = new Regex(@"(?<=\w)\|(?=\w)|((?<=\W)(OR|or|Or)(?=\W))");
                Regex rNot = new Regex(@"(?<=\W)(NOT|not|Not)");
                //cond = cond.Replace("AND", Smv.And);
                string cleared = rAnd.Replace(cond, " " + And + " ");
                cleared = rOr.Replace(cleared, " " + Or + " ");
                cleared = rNot.Replace(cleared, " " + Not);
                return cleared;
            }

            public delegate string ProccessingFunc(string name);

            public static string ConvertConnectionVariableName(string name, ProccessingFunc moduleParamNameConversionFunc, out bool componentVar)
            {
                string[] splitArr = name.Split(Smv.ConnectionNameSeparator);
                string converted = "";
                if (splitArr.Count() == 0) throw new Exception("No connection var name");
                else if (splitArr.Count() == 1)
                {
                    converted = moduleParamNameConversionFunc(splitArr[0]);
                    componentVar = false;
                }
                else if (splitArr.Count() == 2)
                {
                    converted = splitArr[0] + "_" + splitArr[1];
                    componentVar = true;
                }
                else throw new Exception("Unknown var name: " + name);
                return converted;
            }

            public static class DataTypes
            {
                public static string BoolType = "boolean";
                public static string NormalRangeType = "0..99";

                public static string GetType(string varType)
                {
                    if (String.Compare(varType, "BOOL", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return BoolType;
                    if (String.Compare(varType, "INT", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return NormalRangeType;
                    if (String.Compare(varType, "UINT", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return NormalRangeType;
                    return null;
                }
            }

            public static class ModuleParameters
            {
                public static string Splitter = ", ";
                public static string EventPreffix = "event_";
                public static string VariablePreffix = "";
                public static string EventSuffix = "";
                public static string VariableSuffix = "_";

                public static string Event(string name)
                {
                    return EventPreffix + name + EventSuffix;
                }

                public static string Variable(string name)
                {
                    return VariablePreffix + name + VariableSuffix;
                }
            }

            public static class Osm
            {
                public static string S0
                {
                    get { return OsmState("s0"); }
                }

                public static string S1
                {
                    get { return OsmState("s1"); }
                }

                public static string S2
                {
                    get { return OsmState("s2"); }
                }
            }

            public static char[] OrTrimChars = {' ', '|'};
            //public static string
        }

        public class CM_SMV
        {
            public CM_SMV(Storage storage)
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

                //smvModule += _moduleHeader(events, variables, fbType.Name) + "\n";
                smvModule += _smvModuleDeclaration(events, variables, fbType.Name);
                smvModule += _fbInstances(instances) + "\n";
                smvModule += _internalBuffersDeclaration(instances) + "\n";
                smvModule += "\nASSIGN\n";
                smvModule += _internalBuffersInitialization(instances) + "\n";
                //smvModule += _moduleVariablesInitBlock(variables) + "\n";
                //smvModule += _inputVariablesSampleComposite(variables, withConnections) + "\n";
                smvModule += _internalEventConnections(internalBuffers) + "\n";
                smvModule += _internalDataConnections(internalBuffers, withConnections) + "\n";
                smvModule += _resetComponentEventOutputs(internalBuffers) + "\n";
                //smvModule += _eventInputsResetRules(events) + "\n";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Input))
                {
                    string resetRule = "\t" + Smv.Alpha + " : " + Smv.False + ";\n";
                    smvModule += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name), resetRule);
                }
                smvModule += "\n-- ---SCHEDULER--- --\n";
                smvModule += "-- *************** --\n";
                smvModule += _cyclicSchedulingRules(instances) + "\n";


                smvModule += _defineExistsInputEvent(events) + "\n";
                smvModule += _defineOmega(internalBuffers) + "\n";
                smvModule += _moduleFooter() + "\n";
                //smvModule += Smv.AlphaBetaRules;

                return smvModule;
            }

            public string TranslateBasicFB(FBType fbType, bool eventSignalResetSolve = true, bool showUnconditionalTransitions = false)
            {
                string smvModule = "";

                var events = _storage.Events.Where(ev => ev.FBType == fbType.Name);
                var variables = _storage.Variables.Where(ev => ev.FBType == fbType.Name);
                var states = _storage.EcStates.Where(ev => ev.FBType == fbType.Name);
                var algorithms =
                    _storage.Algorithms.Where(alg => alg.FBType == fbType.Name && alg.Language == AlgorithmLanguages.ST);
                var smvAlgs = _translateAlgorithms(algorithms);
                var actions = _storage.EcActions.Where(act => act.FBType == fbType.Name);
                var withConnections = _storage.WithConnections.Where(conn => conn.FBType == fbType.Name);
                var transitions = _storage.EcTransitions.Where(tr => tr.FBType == fbType.Name);

                smvModule += _moduleHeader(events, variables, fbType.Name);

                smvModule += String.Format("VAR {0} : {{{1}, {2}, {3}}};\n", Smv.OsmStateVar, Smv.Osm.S0, Smv.Osm.S1,
                    Smv.Osm.S2);
                smvModule += _eccStatesDeclaration(states) + "\n";
                smvModule += String.Format("VAR {0}: 0..{1};\n", Smv.EcActionsCounterVar, states.Max(state => state.ActionsCount));
                if (smvAlgs.Any()) smvModule += String.Format("VAR {0}: 0..{1};\n", Smv.AlgStepsCounterVar,  smvAlgs.Max(alg => alg.Lines.Max(line => line.NI)));
                else smvModule += String.Format("VAR {0}: 0..{1};\n", Smv.AlgStepsCounterVar, "1");

                smvModule += "\nASSIGN\n";
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.EccStateVar,
                    Smv.EccState(states.First(s => true).Name));
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.OsmStateVar, Smv.Osm.S0);
                smvModule += _moduleVariablesInitBlock(variables) + "\n";
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.EcActionsCounterVar, "0");
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.AlgStepsCounterVar, "0");

                smvModule += "\n" + _ecStateChangeBlock(transitions, events) + "\n";
                smvModule += Smv.OsmStateChangeBlock + "\n";
                smvModule += _ecActionsCounterChangeBlock(states) + "\n";
                /*if (smvAlgs.Any()) */smvModule += _algStepsCounterChangeBlock(states, actions, smvAlgs) + "\n";
                smvModule += _eventInputsResetRules(events, eventSignalResetSolve) + "\n";
                smvModule += _inputVariablesSampleBasic(variables, withConnections) + "\n";
                smvModule +=
                    _outputVariablesChangingRules(variables, actions,
                        _storage.AlgorithmLines.Where(line => line.FBType == fbType.Name)) + "\n";
                smvModule += _outputEventsSettingRules(events, actions) + "\n";
                smvModule += _setOutputVarBuffers(variables, events, actions, withConnections) + "\n";
                smvModule += _setServiceSignals() + "\n";
                smvModule += _basicModuleDefines(states, events, transitions, showUnconditionalTransitions) + "\n";
                smvModule += _moduleFooter() + "\n";
                return smvModule;
            }

            private string _cyclicSchedulingRules(IEnumerable<FBInstance> instances, bool solveSchedulingProblem = true)
            {
                string scheduler = "";

                //string prevAlpha = null;
                string prevBeta = Smv.Alpha;
                bool firstBlock = true;
                foreach (FBInstance instance in instances)
                {
                    string alphaVar = instance.Name + "_" + Smv.Alpha;
                    string betaVar = instance.Name + "_" + Smv.Beta;

                    if (solveSchedulingProblem){
                        scheduler += String.Format(Smv.NextCaseBlock, alphaVar, "\t" + prevBeta + Smv.And + Smv.Omega + (firstBlock ? Smv.And + Smv.Not + Smv.ExistsInputEvent : "") + " : " + Smv.True + ";\n");
                        firstBlock = false;
                    }
                    else
                    {
                        scheduler += String.Format(Smv.NextCaseBlock, alphaVar, "\t" + prevBeta + Smv.And + Smv.Omega + Smv.And + Smv.Not + Smv.ExistsInputEvent + " : " + Smv.True + ";\n");
                    }
                    scheduler += String.Format(Smv.NextCaseBlock, betaVar, "\t" + betaVar + Smv.And + Smv.Omega + " : " + Smv.False + ";\n");
                    prevBeta = betaVar;
                }

                scheduler += String.Format(Smv.NextCaseBlock, Smv.Alpha,
                    "\t" + Smv.Alpha + Smv.And + Smv.Omega + Smv.And + (Smv.Not + Smv.ExistsInputEvent) + " : " +
                    Smv.False + ";\n");
                scheduler += String.Format(Smv.NextCaseBlock, Smv.Beta,
                    "\t" + prevBeta + Smv.And + Smv.Omega + " : " + Smv.True + ";\n");
                return scheduler;
            }

            private string _resetComponentEventOutputs(IEnumerable<Connection> internalBuffers)
            {
                string resetString = "-- _resetComponentEventOutputs\n";
                foreach (string output in _getInternalEventOutputs(internalBuffers))
                {
                    resetString += String.Format(Smv.NextVarAssignment, output, Smv.False);
                }
                return resetString;
            }

            private IEnumerable<string> _getInternalEventOutputs(IEnumerable<Connection> internalBuffers)
            {
                HashSet<string> internalEventOutputs = new HashSet<string>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    bool srcComponent;
                    string srcSmvVar = Smv.ConvertConnectionVariableName(connection.Source, Smv.ModuleParameters.Event,
                        out srcComponent);
                    if (!internalEventOutputs.Contains(srcSmvVar))
                    {
                        if (srcComponent) internalEventOutputs.Add(srcSmvVar);
                    }
                }
                return internalEventOutputs;
            }

            private string _internalBuffersDeclaration(IEnumerable<FBInstance> instances)
            {
                string buffers = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables =
                        _storage.Variables.Where(
                            v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal);
                    var instanceEvents =
                        _storage.Events.Where(
                            ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);
                    foreach (Event ev in instanceEvents)
                    {
                        buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + ev.Name,
                            Smv.DataTypes.BoolType);
                    }
                    foreach (Variable variable in instanceVariables)
                    {
                        if (variable.ArraySize == 0)
                            buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + variable.Name,
                                Smv.DataTypes.GetType(variable.Type));
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                buffers += String.Format(Smv.VarDeclarationBlock,
                                    instance.Name + "_" + variable.Name + "[" + i + "]",
                                    Smv.DataTypes.GetType(variable.Type));
                            }
                        }
                    }
                    buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + Smv.Alpha,
                        Smv.DataTypes.BoolType);
                    buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + Smv.Beta,
                        Smv.DataTypes.BoolType);
                    buffers += "\n";
                }
                return buffers;
            }

            private string _internalBuffersInitialization(IEnumerable<FBInstance> instances)
            {
                string buffersInit = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables =
                        _storage.Variables.Where(
                            v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal);
                    var instanceEvents =
                        _storage.Events.Where(
                            ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);
                    foreach (Event ev in instanceEvents)
                    {
                        buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + ev.Name,
                            Smv.False);
                    }
                    foreach (Variable variable in instanceVariables)
                    {
                        if (variable.ArraySize == 0)
                            buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + variable.Name,
                                Smv.InitialValue(variable));
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                buffersInit += String.Format(Smv.VarInitializationBlock,
                                    instance.Name + "_" + variable.Name + "[" + i + "]", Smv.InitialValue(variable));
                            }
                        }
                    }
                    buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Alpha, Smv.False);
                    buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Beta, Smv.False);
                    buffersInit += "\n";
                }
                return buffersInit;
            }

            private string _fbInstances(IEnumerable<FBInstance> instances)
            {
                string inst = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceEvents = _storage.Events.Where(ev => ev.FBType == instance.InstanceType);
                    var instanceVars = _storage.Variables.Where(v => v.FBType == instance.InstanceType);
                    string instImplementation = instance.InstanceType + " (" +
                                                _moduleParametersString(instanceEvents, instanceVars, instance.Name) +
                                                ")";
                    inst += String.Format(Smv.VarDeclarationBlock, instance.Name, "process " + instImplementation);
                }
                return inst;
            }

            private MultiMap<string> _getEventConnectionsMap(IEnumerable<Connection> internalBuffers)
            {
                MultiMap<string> eventConnectionsMap = new MultiMap<string>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    eventConnectionsMap.Add(connection.Destination, connection.Source);
                }
                return eventConnectionsMap;
            }

            private string _internalEventConnections(IEnumerable<Connection> internalBuffers)
            {
                string eventConnections = "-- _internalEventConnections\n";
                MultiMap<string> eventConnectionsMap = _getEventConnectionsMap(internalBuffers);
                foreach (string dst in eventConnectionsMap.Keys)
                {

                    bool dstComponent;
                    string dstSmvVar = Smv.ConvertConnectionVariableName(dst, Smv.ModuleParameters.Event, out dstComponent);

                    string srcString = "\t("; //+ srcSmvVar + " : " + Smv.True + ";\n";
                    foreach (string src in eventConnectionsMap[dst])
                    {
                        bool srcComponent;
                        string srcSmvVar = Smv.ConvertConnectionVariableName(src, Smv.ModuleParameters.Event, out srcComponent);
                        if (srcComponent) srcString += srcSmvVar + " | ";
                        else srcString += String.Format("({0} & {1}) | ", srcSmvVar, Smv.Alpha);
                    }
                    srcString = srcString.TrimEnd(Smv.OrTrimChars) + ") : " + Smv.True + ";\n";
                    eventConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar, srcString);
                }
                return eventConnections;
            }

            private IEnumerable<string> _getSamplingEventNamesForVariable(string varName, IEnumerable<WithConnection> withConnections)
            {
                List<string> output = new List<string>();
                foreach (WithConnection conn in withConnections.Where(c => c.Var == varName))
                {
                    output.Add(conn.Event);
                }
                return output;
            }

            private string _internalDataConnections(IEnumerable<Connection> internalBuffers, IEnumerable<WithConnection> withConnections)
            {
                string dataConnections = "-- _internalDataConnections\n";
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Data))
                {
                    bool srcComponent;
                    bool dstComponent;
                    string dstSmvVar = Smv.ConvertConnectionVariableName(connection.Destination, Smv.ModuleParameters.Variable, out dstComponent);
                    string srcSmvVar = Smv.ConvertConnectionVariableName(connection.Source, Smv.ModuleParameters.Variable, out srcComponent);
                    string srcString = "";
                    if (srcComponent && dstComponent) {
                        //srcString = "\t" + srcSmvVar + " : " + Smv.True + ";\n"; //TODO: make direct connections without double-buffering
                        dataConnections += String.Format(Smv.NextVarAssignment + "\n", dstSmvVar, srcSmvVar);
                    }
                    else if(dstComponent)
                    {
                        srcString = _varSamplingRule(connection.Source, withConnections, false);
                        dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar, srcString);
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
                                src += Smv.ConvertConnectionVariableName(parentEvent, Smv.ModuleParameters.Event, out dontCare) + Smv.Or;
                            }
                            eventSeed += String.Format("({0}){1}", src.TrimEnd(Smv.OrTrimChars), Smv.Or);
                        }
                        srcString = String.Format("\t{0} : {1};\n", eventSeed.TrimEnd(Smv.OrTrimChars), srcSmvVar);
                        dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar, srcString);
                    }
                }
                return dataConnections;
            }

            private string _defineOmega(IEnumerable<Connection> internalBuffers)
            {
                string omega = "";
                foreach (string output in _getInternalEventOutputs(internalBuffers))
                {
                    omega += Smv.Not + output + Smv.Or;
                }
                return String.Format(Smv.DefineBlock, Smv.Omega, omega.TrimEnd(Smv.OrTrimChars));
            }

            private string _moduleHeader(IEnumerable<Event> events, IEnumerable<Variable> variables, string fbTypeName)
            {
                string outp = "";
                outp += _smvModuleDeclaration(events, variables, fbTypeName);
                foreach (var variable in variables)
                {
                    if (variable.ArraySize == 0)
                        outp += String.Format(Smv.VarDeclarationBlock, variable.Name,
                            Smv.DataTypes.GetType(variable.Type));
                    else
                    {
                        for (int i = 0; i < variable.ArraySize; i++)
                        {
                            outp += String.Format(Smv.VarDeclarationBlock, variable.Name + "[" + i + "]",
                                Smv.DataTypes.GetType(variable.Type));
                        }
                    }
                }
                return outp;
            }

            private string _translateEventNames(string str, IEnumerable<Event> events)
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

            private IEnumerable<TranslatedAlg> _translateAlgorithms(IEnumerable<Algorithm> algorithms)
                //TODO: put translated algorithms to storage
            {
                var smvAlgs = new List<TranslatedAlg>();
                foreach (Algorithm alg in algorithms)
                {
                    TranslatedAlg translated = new TranslatedAlg(alg.Name, Translator.Translate(alg.Text));
                    if (translated.Lines.Any()) smvAlgs.Add(translated);
                }
                return smvAlgs;
            }

            private string _moduleVariablesInitBlock(IEnumerable<Variable> variables)
            {
                string varsInit = "-- _moduleVariablesInitBlock\n";
                foreach (var variable in variables)
                {
                    varsInit += String.Format(Smv.VarInitializationBlock, variable.Name, Smv.InitialValue(variable));
                }
                return varsInit;
            }

            private string _smvModuleDeclaration(IEnumerable<Event> events, IEnumerable<Variable> variables,
                string fbTypeName)
            {

                return String.Format(Smv.ModuleDef, fbTypeName, _moduleParametersString(events, variables));
            }

            private string _moduleParametersString(IEnumerable<Event> events, IEnumerable<Variable> variables,
                string preffix = null)
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
                    if (variable.Direction != Direction.Internal)
                    {
                        if (preffix == null)
                            moduleParameters += (Smv.ModuleParameters.Variable(variable.Name) +
                                                 Smv.ModuleParameters.Splitter);
                        else moduleParameters += (preffix + "_" + variable.Name + Smv.ModuleParameters.Splitter);
                    }
                }
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

            private string _eccStatesDeclaration(IEnumerable<ECState> states)
            {
                string eccStates = "";
                foreach (var ecState in states)
                {
                    eccStates += Smv.EccState(ecState.Name) + Smv.ModuleParameters.Splitter;
                }
                return String.Format("VAR {0} : {{{1}}};\n", Smv.EccStateVar,
                    eccStates.TrimEnd(Smv.ModuleParameters.Splitter.ToCharArray()));
            }

            private string _ecStateChangeBlock(IEnumerable<ECTransition> transitions, IEnumerable<Event> events)
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
                        ecTransitionsSmv += " & " +
                                            _translateEventNames(Smv.ClearConditionExpr(transition.Condition), events);
                    }
                    ecTransitionsSmv += " : ";
                    ecTransitionsSmv += Smv.EccState(transition.Destination);
                    ecTransitionsSmv += ";\n";
                }
                return String.Format(Smv.NextCaseBlock, Smv.EccStateVar, ecTransitionsSmv);
            }

            private string _ecActionsCounterChangeBlock(IEnumerable<ECState> states)
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

            private void _addCounterRules(ref string rule1, ref string rule2, ECState state, int algsCount, ECAction action) 
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

            private string _algStepsCounterChangeBlock(IEnumerable<ECState> states, IEnumerable<ECAction> actions, IEnumerable<TranslatedAlg> algorithms)
            {
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

            private string _eventInputsResetRules(IEnumerable<Event> events, bool eventSignalResetSolve)
            {
                string rules = "";
                string commonResetRule = Smv.OsmStateVar + "=" + Smv.Osm.S1;
                if (!eventSignalResetSolve) commonResetRule += " & " + Smv.ExistsEnabledEcTran;
                string priorityResetRule = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Input))
                {
                    if (priorityResetRule == "")
                        rules += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name),
                            "\t" + commonResetRule + ": " + Smv.False + ";\n");
                    else
                    {
                        string rule = String.Format("\t({0} & ({1})) | ({2}) : {3};\n", Smv.Alpha,
                            priorityResetRule.Trim(Smv.OrTrimChars), commonResetRule, Smv.False);
                        rules += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name), rule);
                    }

                    priorityResetRule += Smv.ModuleParameters.Event(ev.Name) + " | ";
                }
                return rules;
            }

            private string _inputVariablesSampleBasic(IEnumerable<Variable> variables,
                IEnumerable<WithConnection> withConnections)
            {

                string varChangeBlocks = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Input))
                {
                    varChangeBlocks += String.Format(Smv.NextCaseBlock, variable.Name,
                        _varSamplingRule(variable.Name, withConnections, true));
                }
                return varChangeBlocks;
            }

            private string _varSamplingRule(string varName, IEnumerable<WithConnection> withConnections, bool basic)
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
                rule += " : " + Smv.ModuleParameters.Variable(varName) + " ;\n";
                return rule;
            }

            private string _inputVariablesSampleComposite(IEnumerable<Variable> variables,
                IEnumerable<WithConnection> withConnections)
            {

                string varChangeBlocks = "";
                foreach (Variable variable in variables.Where(v => v.Direction == Direction.Input))
                {
                    varChangeBlocks += String.Format(Smv.NormalVarAssignment, variable.Name,
                        Smv.ModuleParameters.Variable(variable.Name));
                }
                return varChangeBlocks;
            }

            private string _outputVariablesChangingRules(IEnumerable<Variable> variables, IEnumerable<ECAction> actions,
                IEnumerable<AlgorithmLine> lines)
            {
                string varChangeBlocks = "";
                foreach (
                    Variable variable in
                        variables.Where(v => v.Direction == Direction.Output || v.Direction == Direction.Internal))
                {
                    string rules = "";
                    IEnumerable<AlgorithmLine> currrentVarLines = lines.Where(line => line.Variable == variable.Name);
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
                            string val = Smv.ClearConditionExpr(line.Value); //TODO: test this. It is an experiment
                            if (String.Compare(val, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
                                val = Smv.False;
                            if (String.Compare(val, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
                                val = Smv.True;
                            rule += " : (" + val + ");\n";
                            rules += rule;
                        }

                    }
                    varChangeBlocks += String.Format(Smv.NextCaseBlock, variable.Name, rules);

                }
                return varChangeBlocks;
            }

            private string _outputEventsSettingRules(IEnumerable<Event> events, IEnumerable<ECAction> actions)
            {
                string eventChangeString = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Output))
                {
                    string rule = "\t" + Smv.OsmStateVar + "=" + Smv.Osm.S2 + " & " + Smv.AlgStepsCounterVar + "=0" +
                                  " & ({0}) : {1};\n";
                    string outCond = "";
                    bool eventSignalSet = false;
                    foreach (ECAction action in actions.Where(act => act.Output == ev.Name))
                    {
                        outCond += "(" + Smv.EccStateVar + "=" + Smv.EccState(action.ECState) + " & " +
                                   Smv.EcActionsCounterVar + "=" + action.Number + ") | ";
                        eventSignalSet = true;
                    }
                    if (eventSignalSet)
                    {
                        rule = String.Format(rule, outCond.TrimEnd(Smv.OrTrimChars), Smv.True);
                        eventChangeString += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name), rule);
                    }
                    else eventChangeString += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name), "");
                }
                return eventChangeString;
            }

            private string _setOutputVarBuffers(IEnumerable<Variable> variables, IEnumerable<Event> events,
                IEnumerable<ECAction> actions, IEnumerable<WithConnection> withConnections)
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
                        foreach (ECAction action in actions.Where(act => act.Output == connection.Event))
                        {
                            if (!ac.Contains(action)) ac.Add(action);
                        }
                    }

                    foreach (ECAction action in ac)
                    {
                        outCond += "(" + Smv.EccStateVar + "=" + Smv.EccState(action.ECState) + " & " +
                                   Smv.EcActionsCounterVar + "=" + action.Number + ") | ";
                    }
                    rule = String.Format(rule, outCond.TrimEnd(Smv.OrTrimChars), variable.Name);
                    outVarsChangeString += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Variable(variable.Name),
                        rule);
                }
                return outVarsChangeString;
            }

            private string _setServiceSignals()
            {
                string ruleTemplate = "\t({0} & {1}={2} & !{3} | {1}={4} & {5}): {6};\n";
                string betaRule = String.Format(ruleTemplate,
                    Smv.Alpha,
                    Smv.OsmStateVar,
                    Smv.Osm.S0,
                    Smv.ExistsInputEvent,
                    Smv.Osm.S1,
                    Smv.AbsentsEnabledEcTran,
                    Smv.True
                    );
                string alphaRule = String.Format(ruleTemplate,
                    Smv.Alpha,
                    Smv.OsmStateVar,
                    Smv.Osm.S0,
                    Smv.ExistsInputEvent,
                    Smv.Osm.S1,
                    Smv.AbsentsEnabledEcTran,
                    Smv.False
                    );
                return String.Format(Smv.NextCaseBlock, Smv.Beta, betaRule) +
                       String.Format(Smv.NextCaseBlock, Smv.Alpha, alphaRule);
            }

            private string _defineExistsInputEvent(IEnumerable<Event> events)
            {
                string inputEvents = events.Where(ev => ev.Direction == Direction.Input)
                    .Aggregate("", (current, ev) => current + (Smv.ModuleParameters.Event(ev.Name) + " | "));
                return String.Format(Smv.DefineBlock, Smv.ExistsInputEvent, inputEvents.Trim(Smv.OrTrimChars));
            }

            private string _basicModuleDefines(IEnumerable<ECState> states, IEnumerable<Event> events,
                IEnumerable<ECTransition> transitions, bool showUnconditionalTransitions)
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

                string existsEnabledECTran = String.Format("DEFINE {0}:= {1};\n", Smv.ExistsEnabledEcTran,
                    ecTran.Trim(Smv.OrTrimChars));
                string absentsEnabledECTran = "\n";
                return _defineExistsInputEvent(events) + existsEnabledECTran + absentsEnabledECTran;

            }

            private string _moduleFooter()
            {
                return "\nFAIRNESS running\n";
            }

            private IEnumerable<ECAction> _findActionsByAlgorithmName(IEnumerable<ECAction> actions,
                string algorithmName)
            {
                return actions.Where(act => act.Algorithm == algorithmName);
            }

            public List<string> BasicBlocks = new List<string>();
            private Storage _storage;
        }
    }
}