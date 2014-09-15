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
            public SmvCodeGenerator(Storage storage, IEnumerable<ExecutionModel> executionModels, Settings settings)
            {
                _storage = storage;
                _executionModels = executionModels;
                _settings = settings;
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
                ExecutionModel executionModel = _executionModels.FirstOrDefault(em => em.FBTypeName == fbType.Name);
                var events = _storage.Events.Where(ev => ev.FBType == fbType.Name);
                var variables = _storage.Variables.Where(ev => ev.FBType == fbType.Name);
                var instances = _storage.Instances.Where(inst => inst.FBType == fbType.Name);
                var withConnections = _storage.WithConnections.Where(conn => conn.FBType == fbType.Name);
                var connections = _storage.Connections.Where(conn => conn.FBType == fbType.Name);
                var instanceParameters = _storage.InstanceParameters.Where(p => p.FBType == fbType.Name);

                IDispatcher dispatcher = executionModel.Dispatcher;

                //smvModule += _moduleHeader(events, variables, fbType.Name) + "\n";
                smvModule += FbSmvCommon.SmvModuleDeclaration(events, variables, fbType.Name);
                smvModule += CompositeFbSmv.FbInstances(instances, _storage.Events, _storage.Variables, connections, _settings) + "\n";
                smvModule += CompositeFbSmv.InternalBuffersDeclaration(instances, connections, _storage.Events, _storage.Variables) + "\n";
                smvModule += Smv.Assign;
                smvModule += CompositeFbSmv.InternalBuffersInitialization(instances, connections, _storage.Events, _storage.Variables, instanceParameters) + "\n";
                //smvModule += _moduleVariablesInitBlock(variables) + "\n";
                //smvModule += _inputVariablesSampleComposite(variables, withConnections) + "\n";
                smvModule += CompositeFbSmv.InternalDataConnections(connections, withConnections, _storage.Variables, instances) + "\n";
                smvModule += CompositeFbSmv.ComponentEventOutputs(connections, _settings.UseProcesses) + "\n";
                //smvModule += _eventInputsResetRules(events) + "\n";
                smvModule += CompositeFbSmv.InputEventsResetRules(events);
                smvModule += "\n-- ---DISPATCHER--- --\n";
                smvModule += "-- *************** --\n";
                smvModule += dispatcher.GetSmvCode(_settings.UseProcesses) + "\n";

                smvModule += CompositeFbSmv.InternalEventConnections(connections, _settings.UseProcesses) + "\n";

                smvModule += FbSmvCommon.DefineExistsInputEvent(events) + "\n";
                smvModule += CompositeFbSmv.DefineOmega(connections) + "\n";

                smvModule += FbSmvCommon.ModuleFooter(_settings) + "\n";
                //smvModule += Smv.AlphaBetaRules;

                return smvModule;
            }

            public string TranslateBasicFB(FBType fbType, bool eventSignalResetSolve = true, bool showUnconditionalTransitions = false)
            {
                string smvModule = "";

                ExecutionModel executionModel = _executionModels.FirstOrDefault(em => em.FBTypeName == fbType.Name);
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

                smvModule += Smv.Assign;
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
                smvModule += BasicFbSmv.OutputVariablesChangingRules(variables, actions, _storage.AlgorithmLines.Where(line => line.FBType == fbType.Name), _settings) + "\n";
                smvModule += BasicFbSmv.SetOutputVarBuffers(variables, events, actions, withConnections) + "\n";
                smvModule += BasicFbSmv.SetServiceSignals(_settings.UseProcesses) + "\n";

                smvModule += BasicFbSmv.EventInputsResetRules(events, executionModel, eventSignalResetSolve, _settings.UseProcesses) + "\n";
                smvModule += BasicFbSmv.OutputEventsSettingRules(events, actions, _settings.UseProcesses) + "\n";

                smvModule += BasicFbSmv.BasicModuleDefines(states, events, transitions, showUnconditionalTransitions) + "\n";

                smvModule += FbSmvCommon.ModuleFooter(_settings) + "\n";
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
            private IEnumerable<ExecutionModel> _executionModels;
            private Settings _settings;
        }
        internal class FbSmvCommon
        {
            public static string ModuleParametersString(IEnumerable<Event> events, IEnumerable<Variable> variables, string preffix = null)
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
            public static string VarSamplingRule(string varName, IEnumerable<WithConnection> withConnections, bool basic, int arrayIndex = -1)
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
            public static string DefineExistsInputEvent(IEnumerable<Event> events)
            {
                string inputEvents = events.Where(ev => ev.Direction == Direction.Input).Aggregate("", (current, ev) => current + (Smv.ModuleParameters.Event(ev.Name) + " | "));
                return String.Format(Smv.DefineBlock, Smv.ExistsInputEvent, inputEvents.Trim(Smv.OrTrimChars));
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

        }
        internal class CompositeFbSmv
        {
            public static string FbInstances(IEnumerable<FBInstance> instances, IEnumerable<Event> nonFilteredEvents, IEnumerable<Variable> nonFilteredVariables, IEnumerable<Connection> connections, Settings settings)
            {
                string inst = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType);
                    var instanceVars = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType);

                    string instanceParametersString = InstanceParametersString(instanceEvents, instanceVars, connections, instance.Name);

                    string instImplementation = instance.InstanceType + " (" + instanceParametersString + ")";

                    inst += String.Format(Smv.VarDeclarationBlock, instance.Name, (settings.UseProcesses ? "process " : "") + instImplementation);
                }
                return inst;
            }
            public static string InternalBuffersDeclaration(IEnumerable<FBInstance> instances, IEnumerable<Connection> connections, IEnumerable<Event> nonFilteredEvents, IEnumerable<Variable> nonFilteredVariables)
            {
                string buffers = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal);
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);


                    foreach (Event ev in instanceEvents)
                    {
                        //Connection inputConnection;
                        //if (! _isInputFromComponent(ev, connections, instance.Name, out inputConnection))
                        //{
                            buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + ev.Name, Smv.DataTypes.BoolType);
                        //}
                    }
                    foreach (Variable variable in instanceVariables)
                    {
                        Connection inputConnection;
                        if (!_isInputFromComponent(variable, connections, instance.Name, out inputConnection))
                        {
                            if (variable.ArraySize == 0)
                                buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + variable.Name, variable.SmvType);
                            else
                            {
                                for (int i = 0; i < variable.ArraySize; i++)
                                {
                                    buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + variable.Name + Smv.ArrayIndex(i), variable.SmvType);
                                }
                            }
                        }
                    }
                    buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + Smv.Alpha, Smv.DataTypes.BoolType);
                    buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + Smv.Beta, Smv.DataTypes.BoolType);
                    buffers += "\n";
                }
                return buffers;
            }
            private static bool _isInputFromComponent(FBInterface instanceParameter, IEnumerable<Connection> connections, string instanceName, out Connection inputConnection)
            {
                inputConnection = null;
                bool srcComponent = false;

                if (instanceParameter.Direction == Direction.Input)
                {
                    inputConnection = connections.FirstOrDefault(conn => conn.Destination == instanceName + "." + instanceParameter.Name);
                    if (inputConnection != null) Smv.ConvertConnectionVariableName(inputConnection.Source, Smv.ModuleParameters.Event, out srcComponent);
                }

                return srcComponent;
            }

            private static string _getInstanceParameterString(FBInterface instanceParameter, IEnumerable<Connection> connections, string instanceName)
            {
                Connection inputConnection;
                if (_isInputFromComponent(instanceParameter, connections, instanceName, out inputConnection))
                {
                    return (inputConnection.Source.Replace('.', '_'));
                }
                else
                {
                    return (instanceName + "_" + instanceParameter.Name);
                }
            }
            public static string InstanceParametersString(IEnumerable<Event> events, IEnumerable<Variable> variables, IEnumerable<Connection> connections, string instanceName)
            {
                string moduleParameters = "";
                foreach (Event ev in events)
                {
                    //moduleParameters += _getInstanceParameterString(ev, connections, instanceName) + Smv.ModuleParameters.Splitter;
                    moduleParameters += (instanceName + "_" + ev.Name) + Smv.ModuleParameters.Splitter;
                }
                foreach (Variable variable in variables)
                {
                    if (variable.Direction != Direction.Internal)
                    {
                        moduleParameters += _getInstanceParameterString(variable, connections, instanceName) + Smv.ModuleParameters.Splitter;
                    }
                }

                moduleParameters += instanceName + "_" + Smv.Alpha + Smv.ModuleParameters.Splitter;
                moduleParameters += instanceName + "_" + Smv.Beta + Smv.ModuleParameters.Splitter;

                return moduleParameters.TrimEnd(Smv.ModuleParameters.Splitter.ToCharArray());
            }

            public static string InternalBuffersInitialization(IEnumerable<FBInstance> instances, IEnumerable<Connection> connections, IEnumerable<Event> nonFilteredEvents, IEnumerable<Variable> nonFilteredVariables, IEnumerable<InstanceParameter> instanceParameters)
            {
                string buffersInit = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal);
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);
                    foreach (Event ev in instanceEvents)
                    {
                        //Connection inputConnection;
                        //if (!_isInputFromComponent(ev, connections, instance.Name, out inputConnection))
                        //{
                            buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + ev.Name, Smv.False);
                        //}
                    }
                    foreach (Variable variable in instanceVariables)
                    {
                        Connection inputConnection;
                        if (_isInputFromComponent(variable, connections, instance.Name, out inputConnection)) continue;
                        
                        // if(! _isInputFromComponent)
                        InstanceParameter instanceParameter = instanceParameters.FirstOrDefault(p => p.InstanceName == instance.Name && p.Name == variable.Name);
                        if (variable.ArraySize == 0)
                        {
                            string value = instanceParameter == null ? Smv.InitialValue(variable) : instanceParameter.Value;
                            buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + variable.Name, value);
                        }
                        else
                        {
                            string[] values;

                            if (instanceParameter == null)
                            {
                                values = new string[variable.ArraySize];
                                for (int i = 0; i < variable.ArraySize; i++) values[i] = Smv.InitialValue(variable);
                            }
                            else
                            {
                                char[] trimChars = {'[',']'};
                                values = instanceParameter.Value.Trim(trimChars).Split(',');
                                if (values.Count() != variable.ArraySize) throw new Exception("Invalid array value " + instanceParameter.Value);
                            }
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + variable.Name + Smv.ArrayIndex(i), values[i]);
                            }
                        }
                    }
                    buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Alpha, Smv.False);
                    buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Beta, Smv.False);
                    buffersInit += "\n";
                }
                return buffersInit;
            }
            public static string InternalEventConnections(IEnumerable<Connection> internalBuffers, bool useProcesses)
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

                    if (useProcesses)
                    {
                        srcString = srcString.TrimEnd(Smv.OrTrimChars) + ") : " + Smv.True + ";\n";
                        eventConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar, srcString);
                    }
                    else
                    {
                        string reset_string;
                        if (dstComponent)
                        {
                            string[] connectionDst = Smv.SplitConnectionVariableName(dst);
                            reset_string = String.Format("\t({0}.{1}_reset) : {2};\n", connectionDst[0], Smv.ModuleParameters.Event(connectionDst[1]), Smv.False);
                        }
                        else
                        {
                            reset_string = String.Format("\t{0} : {1};\n", Smv.True, Smv.False);
                        }
                        srcString = srcString.TrimEnd(Smv.OrTrimChars) + ") : " + Smv.True + ";\n";
                        eventConnections += String.Format(Smv.EmptyNextCaseBlock + "\n", dstSmvVar, srcString + reset_string);
                    }
                    
                }
                return eventConnections;
            }
            public static string InternalDataConnections(IEnumerable<Connection> internalBuffers, IEnumerable<WithConnection> withConnections, IEnumerable<Variable> allVariables, IEnumerable<FBInstance> instances)
            {
                string dataConnections = "-- _internalDataConnections\n";
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Data))
                {
                    bool srcComponent;
                    bool dstComponent;
                    string dstSmvVar = Smv.ConvertConnectionVariableName(connection.Destination, Smv.ModuleParameters.Variable, out dstComponent);
                    string srcSmvVar = Smv.ConvertConnectionVariableName(connection.Source, Smv.ModuleParameters.Variable, out srcComponent);
                    string srcString = "";
                    /*if (srcComponent && dstComponent)
                    {
                        //srcString = "\t" + srcSmvVar + " : " + Smv.True + ";\n"; //TODO: make direct connections without double-buffering
                        var srcVar = _findVariable(connection.Source, allVariables, instances);

                        if (srcVar.ArraySize == 0)
                            dataConnections += String.Format(Smv.NextVarAssignment + "\n", dstSmvVar, srcSmvVar);
                        else
                        {
                            for (int i = 0; i < srcVar.ArraySize; i++)
                            {
                                dataConnections += String.Format(Smv.NextVarAssignment + "\n", dstSmvVar + Smv.ArrayIndex(i), srcSmvVar + Smv.ArrayIndex(i));
                            }
                        }
                    }
                    else */
                    if (!srcComponent && dstComponent)
                    {
                        var dstVar = _findVariable(connection.Destination, allVariables, instances);
                        if (dstVar.ArraySize == 0)
                        {
                            srcString = FbSmvCommon.VarSamplingRule(connection.Source, withConnections, false);
                            dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar, srcString);
                        } 
                        else
                        {
                            for (int i = 0; i < dstVar.ArraySize; i++)
                            {
                                srcString = FbSmvCommon.VarSamplingRule(connection.Source, withConnections, false, i);
                                dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar + Smv.ArrayIndex(i), srcString);
                            }
                        }
                    }
                    else if (srcComponent && !dstComponent)
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

                        var srcVar = _findVariable(connection.Source, allVariables, instances);
                        if (srcVar.ArraySize == 0)
                        {
                            srcString = String.Format("\t{0} : {1};\n", eventSeed.TrimEnd(Smv.OrTrimChars), srcSmvVar);
                            dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar, srcString);
                        }
                        else
                        {
                            for (int i = 0; i < srcVar.ArraySize; i++)
                            {
                                srcString = String.Format("\t{0} : {1};\n", eventSeed.TrimEnd(Smv.OrTrimChars), srcSmvVar + Smv.ArrayIndex(i));
                                dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar + Smv.ArrayIndex(i), srcString);
                            }
                        }
                    }
                }
                return dataConnections;
            }

            private static Variable _findVariable(string connectionPoint, IEnumerable<Variable> allVariables, IEnumerable<FBInstance> instances)
            {
                string[] nameSplit = Smv.SplitConnectionVariableName(connectionPoint);
                FBInstance fbInst = instances.FirstOrDefault(inst => inst.Name == nameSplit[0]);
                Variable foundVar = allVariables.FirstOrDefault(v => v.FBType == fbInst.InstanceType && v.Name == nameSplit[1]);
                return foundVar;
            }

            public static string ComponentEventOutputs(IEnumerable<Connection> connections, bool useProcesses)
            {
                string outputsString = "-- ComponentEventOutputs\n";
                if (useProcesses)
                {
                    foreach (string output in _getInternalEventOutputStrings(connections))
                    {
                        outputsString += String.Format(Smv.NextVarAssignment, output, Smv.False);
                    }
                }
                else
                {
                    foreach (Connection outputConnection in _getInternalEventOutputs(connections))
                    {
                        string[] outpSplit = Smv.SplitConnectionVariableName(outputConnection.Source);
                        bool alreadyChecked;
                        string varName = Smv.ConvertConnectionVariableName(outputConnection.Source, Smv.ModuleParameters.Event, out alreadyChecked);

                        string setRule = String.Format("\t{0}.{1}_set : {2};\n", outpSplit[0], Smv.ModuleParameters.Event(outpSplit[1]), Smv.True);
                        string resetRule = String.Format("\t{0} : {1};\n", Smv.True, Smv.False);
                        outputsString += String.Format(Smv.EmptyNextCaseBlock, varName, setRule + resetRule);
                    }
                }
                return outputsString;
            }
            public static string DefineOmega(IEnumerable<Connection> internalBuffers)
            {
                string omega = "";
                foreach (string output in _getInternalEventOutputStrings(internalBuffers))
                {
                    omega += Smv.Not + output + Smv.Or;
                }
                return String.Format(Smv.DefineBlock, Smv.Omega, omega.TrimEnd(Smv.OrTrimChars));
            }
            public static string InputEventsResetRules(IEnumerable<Event> events)
            {
                string rules = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Input))
                {
                    string resetRule = "\t" + Smv.Alpha + " : " + Smv.False + ";\n";
                    rules += String.Format(Smv.NextCaseBlock, Smv.ModuleParameters.Event(ev.Name), resetRule);
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
            private static IEnumerable<string> _getInternalEventOutputStrings(IEnumerable<Connection> internalBuffers)
            {
                HashSet<string> internalEventOutputs = new HashSet<string>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    bool srcComponent;
                    string srcSmvVar = Smv.ConvertConnectionVariableName(connection.Source, Smv.ModuleParameters.Event, out srcComponent);
                    if (!internalEventOutputs.Contains(srcSmvVar))
                    {
                        if (srcComponent) internalEventOutputs.Add(srcSmvVar);
                    }
                }
                return internalEventOutputs;
            }

            private static IEnumerable<Connection> _getInternalEventOutputs(IEnumerable<Connection> internalBuffers)
            {
                HashSet<Connection> internalEventOutputs = new HashSet<Connection>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    bool srcComponent;
                    Smv.ConvertConnectionVariableName(connection.Source, Smv.ModuleParameters.Event, out srcComponent);
                    if (!internalEventOutputs.Contains(connection))
                    {
                        if (srcComponent) internalEventOutputs.Add(connection);
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
                        outp += String.Format(Smv.VarDeclarationBlock, variable.Name, variable.SmvType);
                    else
                    {
                        for (int i = 0; i < variable.ArraySize; i++)
                        {
                            outp += String.Format(Smv.VarDeclarationBlock, variable.Name + Smv.ArrayIndex(i), variable.SmvType);
                        }
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
                return varChangeBlocks;
            }
            public static string OutputVariablesChangingRules(IEnumerable<Variable> variables, IEnumerable<ECAction> actions, IEnumerable<AlgorithmLine> lines, Settings settings)
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

                            if (variable.SmvType.GetType() != typeof(Smv.DataTypes.BoolSmvType) && settings.ModularArithmetics)
                            {
                                //string rangeStr = variable.SmvType.Split()
                                Smv.DataTypes.RangeSmvType varType = (Smv.DataTypes.RangeSmvType)variable.SmvType;

                                int modulo = varType.RangeEnd-varType.RangeBegin;
                                string correction;
                                if (varType.RangeBegin > 0) correction = " + " + varType.RangeBegin;
                                else if (varType.RangeBegin < 0) correction = " - " + (Math.Abs(varType.RangeBegin));
                                else correction = "";

                                rule += " : (" + val + ") mod " + modulo + correction +";\n";
                            }
                            else
                            {
                                rule += " : (" + val + ");\n";
                            }
                            rules += rule;
                        }

                    }
                    varChangeBlocks += String.Format(Smv.NextCaseBlock, variable.Name, rules);

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
            public static string SetOutputVarBuffers(IEnumerable<Variable> variables, IEnumerable<Event> events, IEnumerable<ECAction> actions, IEnumerable<WithConnection> withConnections)
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
                    return String.Format(Smv.DefineBlock, "alpha_beta", rule);
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