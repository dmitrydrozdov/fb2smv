using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2SMV.FBCollections;
using FB2SMV.ServiceClasses;

namespace FB2SMV
{
    namespace Core
    {
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

            public static string ComponentDataOutputNextStatements(IEnumerable<Variable> allVariables, IEnumerable<FBInstance> instances)
            {
                string outStr = "";
                foreach (FBInstance instance in instances)
                {
                    var dataOutputs = allVariables.Where(v => v.FBType == instance.InstanceType && v.Direction == Direction.Output);
                    foreach (Variable variable in dataOutputs)
                    {
                        if (variable.ArraySize == 0)
                        {
                            string varName = instance.Name + "_" + variable.Name;
                            outStr += String.Format(Smv.NextVarAssignment, varName, varName);
                        }
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                string varName = String.Format("{0}_{1}[{2}]", instance.Name, variable.Name, i);
                                outStr += String.Format(Smv.NextVarAssignment, varName, varName);
                            }
                        }
                    }
                }
                return outStr;
            }
            public static string NonConnectedInstanceOutputEvents(IEnumerable<Event> allEvents, IEnumerable<FBInstance> instances, IEnumerable<Connection> connections)
            {
                string outStr = "";
                foreach (FBInstance instance in instances)
                {
                    var nonConnectedEvents = allEvents.Where(ev => ev.FBType == instance.InstanceType && _eventIsNonConnected(instance.Name, ev, connections));
                    foreach (Event ev in nonConnectedEvents)
                    {
                        string bufName = instance.Name + "_" + ev.Name;
                        outStr += String.Format(Smv.NextVarAssignment, bufName, bufName);
                    }
                }
                return outStr;
            }

            private static bool _eventIsNonConnected(string instanceName, Event ev, IEnumerable<Connection> connections)
            {
                foreach (Connection conn in connections)
                {
                    if (conn.Source.InstanceName == instanceName && conn.Source.Variable == ev.Name 
                       || conn.Destination.InstanceName == instanceName && conn.Destination.Variable == ev.Name)
                        return false;
                }
                return true;
            }

            public static string InternalBuffersDeclaration(IEnumerable<FBInstance> instances, IEnumerable<Connection> connections, IEnumerable<Event> nonFilteredEvents, IEnumerable<Variable> nonFilteredVariables)
            {
                string buffers = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal && !v.IsConstant);
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);

                    foreach (Event ev in instanceEvents)
                    {
                        //Connection inputConnection;
                        //if (! _isInputFromComponent(ev, connections, instance.Name, out inputConnection))
                        //{
                        buffers += String.Format(Smv.VarDeclarationBlock, instance.Name + "_" + ev.Name, EventInstance.SmvType(ev));
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

            internal static string NonConnectedEvents(IEnumerable<Connection> connections, List<Event> events, IEnumerable<FBInstance> instances, ShowMessageDelegate showMessage)
            {
                 
                string ret = "";
                
                string HandleNotConnectedEvent(EventInstance nonConnectedEvent)
                {
                    showMessage(String.Format("Warning! Event {0} in {1}:{2} is not connected!",
                        nonConnectedEvent.Event.Name, nonConnectedEvent.Instance.Name , nonConnectedEvent.Instance.FBType));

                    ret += String.Format(Smv.NextVarAssignment, nonConnectedEvent.Value(), nonConnectedEvent.Value());
                    if (nonConnectedEvent.Event.Timed)
                    {
                        ret += String.Format(Smv.NextVarAssignment, nonConnectedEvent.TSLast(), nonConnectedEvent.TSLast());
                        ret += String.Format(Smv.NextVarAssignment, nonConnectedEvent.TSBorn(), nonConnectedEvent.TSBorn());     
                    }
                   
                    return ret;
                }

                foreach (FBInstance instance in instances)
                {
                    var instanceInputEvents = events.Where(ev => ev.FBType == instance.InstanceType && ev.Direction == Direction.Input);
                    var instanceOutputEvents = events.Where(ev => ev.FBType == instance.InstanceType && ev.Direction == Direction.Output);

                    foreach (Event inputEvent in instanceInputEvents)
                    {
                        Connection connection = connections.FirstOrDefault(conn => conn.Destination.InstanceName == instance.Name
                                                                                   && conn.Destination.Variable == inputEvent.Name);
                        if (connection == null)
                        {
                            ret = HandleNotConnectedEvent(new EventInstance(inputEvent, instance));
                        }
                    }
                    
                    foreach (Event outputEvent in instanceOutputEvents)
                    {
                        Connection connection = connections.FirstOrDefault(conn => conn.Source.InstanceName == instance.Name 
                                                                                   && conn.Source.Variable == outputEvent.Name);
                        if (connection == null)
                        {
                            ret = HandleNotConnectedEvent(new EventInstance(outputEvent, instance));
                        }
                    }
                }
                return ret;
               
            }

            private static bool _isInputFromComponent(FBInterface instanceParameter, IEnumerable<Connection> connections, string instanceName, out Connection inputConnection)
            {
                inputConnection = null;
                bool srcComponent = false;

                if (instanceParameter.Direction == Direction.Input)
                {
                    inputConnection = connections.FirstOrDefault(conn => conn.Destination.InstanceName == instanceName 
                                                                        && conn.Destination.Variable == instanceParameter.Name);
                    if (inputConnection != null) srcComponent = inputConnection.Source.IsComponentVar();
                }

                return srcComponent;
            }

            private static string _getInstanceParameterString(FBInterface instanceParameter, IEnumerable<Connection> connections, string instanceName)
            {
                if (_isInputFromComponent(instanceParameter, connections, instanceName, out var inputConnection))
                {
                    return inputConnection.Source.InstanceName + "_" + inputConnection.Source.Variable;
                }

                return instanceName + "_" + instanceParameter.Name;
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
                    if (variable.Direction != Direction.Internal && !variable.IsConstant)
                    {
                        moduleParameters += _getInstanceParameterString(variable, connections, instanceName) + Smv.ModuleParameters.Splitter;
                    }
                }

                moduleParameters += "TGlobal" + Smv.ModuleParameters.Splitter;
                moduleParameters += instanceName + "_" + Smv.Alpha + Smv.ModuleParameters.Splitter;
                moduleParameters += instanceName + "_" + Smv.Beta + Smv.ModuleParameters.Splitter;

                return moduleParameters.TrimEnd(Smv.ModuleParameters.Splitter.ToCharArray());
            }

            /*private static string _clearZeroValues(string input, Type varType) //Fix the problem with 0.0 initial values
            {
                if (input == "0.0") return "0";
                if (varType == typeof (Smv.DataTypes.BoolSmvType))
                {
                    if (input == "0") return Smv.False;
                    if (input == "1") return Smv.True;
                }
                return input;
            }*/
            private static bool _nonInitializableVar(Variable variable)
            {
                var delayBlocks = new string[] { "E_DELAY", "E_CYCLE" };
                var delayDataIOs = new string[] { "Di", "Do", "Dt" };

                if (delayBlocks.Contains(variable.FBType) && delayDataIOs.Contains(variable.Name)) return true;
                else return false;
            }
            public static string InternalBuffersInitialization(IEnumerable<FBInstance> instances, IEnumerable<Connection> connections, IEnumerable<Variable> nonFilteredVariables, IEnumerable<InstanceParameter> instanceParameters, bool mainModule = false)
            {
                string buffersInit = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables = nonFilteredVariables.Where(v => v.FBType == instance.InstanceType && v.Direction != Direction.Internal && !v.IsConstant);
                    foreach (Variable variable in instanceVariables)
                    {
                        Connection inputConnection;
                        if (_nonInitializableVar(variable)) continue; // do not initialize SMV variables for FB_DELAY and E_CYCLE data IOs
                        if (_isInputFromComponent(variable, connections, instance.Name, out inputConnection)) continue;

                        // if(! _isInputFromComponent)
                        InstanceParameter instanceParameter = instanceParameters.FirstOrDefault(p => p.InstanceName == instance.Name && p.Name == variable.Name);
                        if (variable.ArraySize == 0)
                        {
                            string value = instanceParameter == null ? Smv.InitialValue(variable) : Smv.ClearInitialValue(instanceParameter.Value, variable);
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
                                char[] trimChars = { '[', ']' };
                                values = instanceParameter.Value.Trim(trimChars).Split(',');
                                if (values.Count() != variable.ArraySize) throw new Exception("Invalid array value " + instanceParameter.Value);
                            }
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + variable.Name + Smv.ArrayIndex(i), Smv.ClearInitialValue(values[i], variable));
                            }
                        }
                    }
                    if (!mainModule)
                    {
                        buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Alpha, Smv.False);
                        buffersInit += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Beta, Smv.False);
                    }
                    buffersInit += "\n";
                }
                return buffersInit;
            }
            public static string InternalEventConnections(IEnumerable<Connection> internalBuffers, Storage _storage, bool useProcesses)
            {
                string eventConnections = "-- _internalEventConnections\n";
                var eventConnectionsMap = _getEventConnectionsMap(internalBuffers, _storage);
                var definesList = new List<string>();
                foreach (EventInstance dst in eventConnectionsMap.Keys)
                {

                    if (dst.IsComponentVar())
                    {
                        string valueString = "";
                        string tsbornString = "";
                        string tsLastString = "";
                        foreach (EventInstance src in eventConnectionsMap[dst])
                        {
                            string srcString = "\t";
                            if (src.IsComponentVar()) srcString += src.Value();
                            else srcString += $"({src.Value()} & {Smv.Alpha})";
                            valueString  += $"{srcString} : {src.Value()};\n";
                            tsbornString += $"{srcString} : {src.TSBorn()};\n";
                            tsLastString += $"{srcString} : {src.TSLast()};\n";
                        }
                    
                        if (useProcesses)
                        {
                            eventConnections += String.Format(Smv.NextCaseBlock + "\n", dst.Value(), valueString);
                        }
                        else
                        {
                            string reset_string;
                            reset_string = String.Format("\t({0}.{1}_reset) : {2};\n", dst.Instance.Name, dst.ParameterName(), Smv.False);
                          
                            eventConnections += String.Format(Smv.NextCaseBlock + "\n", dst.Value(), valueString + reset_string);
                            if (dst.Event.Timed)
                            {
                                eventConnections += String.Format(Smv.NextCaseBlock + "\n", dst.TSBorn(), tsbornString);
                                eventConnections += String.Format(Smv.NextCaseBlock + "\n", dst.TSLast(), tsLastString);   
                            }   
                        }
                    }
                    else
                    {
                        string srcString = "\t";
                        foreach (EventInstance src in eventConnectionsMap[dst])
                        {   
                            if (src.IsComponentVar()) srcString += src.Value();
                            else srcString += $"({src.Value()} & {Smv.Alpha})";
                            srcString += " | ";
                        }
                        srcString = srcString.TrimEnd(Smv.OrTrimChars);

                        //reset_string = String.Format("\t{0} : {1};\n", Smv.True, Smv.False);
                        //srcString = srcString.TrimEnd(Smv.OrTrimChars) + ") : " + Smv.True + ";\n";
                        //eventConnections += String.Format(Smv.EmptyNextCaseBlock + "\n", dstSmvVar, srcString + reset_string);
                        definesList.Add(String.Format(Smv.DefineBlock, dst.ParameterName() + "_set", srcString));
                    }
                    
                    
                }

                foreach (string def in definesList)
                {
                    eventConnections += def;
                }
                return eventConnections;
            }
            public static string InternalDataConnections(
                IEnumerable<Connection> internalBuffers, 
                IEnumerable<WithConnection> withConnections,
                Storage _storage)
            {
                string dataConnections = "-- _internalDataConnections\n";
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Data))
                {
                    bool srcComponent = connection.Source.IsComponentVar();
                    bool dstComponent = connection.Destination.IsComponentVar();
                    string dstSmvVar = Smv.ConvertConnectionVariableName(connection.Destination, Smv.ModuleParameters.Variable);
                    string srcSmvVar = Smv.ConvertConnectionVariableName(connection.Source, Smv.ModuleParameters.Variable);
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
                        Variable dstVar = _findVariable(connection.Destination, connection.FBType, _storage);
                        if (dstVar.ArraySize == 0)
                        {
                            srcString = FbSmvCommon.VarSamplingRule(connection.Source.Variable, withConnections, _storage.Events, false);
                            dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar, srcString);
                        }
                        else
                        {
                            for (int i = 0; i < dstVar.ArraySize; i++)
                            {
                                srcString = FbSmvCommon.VarSamplingRule(connection.Source.Variable, withConnections, _storage.Events, false, i);
                                dataConnections += String.Format(Smv.NextCaseBlock + "\n", dstSmvVar + Smv.ArrayIndex(i), srcString);
                            }
                        }
                    }
                    else if (srcComponent && !dstComponent)
                    {
                        var samplingEvents = _getSamplingEventNamesForVariable(connection.Destination.Variable, withConnections, _storage.Events);
                        var eventConnectionsMap = _getEventConnectionsMap(internalBuffers, _storage);

                        string eventSeed = "";
                        foreach (Event ev in samplingEvents)
                        {
                            string src = "";
                            foreach (var parentEvent in eventConnectionsMap[new EventInstance(ev, null)])
                            {
                                src += parentEvent.Value() + Smv.Or;
                            }
                            eventSeed += String.Format("({0}){1}", src.TrimEnd(Smv.OrTrimChars), Smv.Or);
                        }

                        Variable srcVar = _findVariable(connection.Source, connection.FBType, _storage);
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

            private static Variable _findVariable(ConnectionNode connectionPoint, string currentFB, Storage _storage)  
            {
                FBInstance fbInst = _storage.Instances.FirstOrDefault(inst => inst.Name == connectionPoint.InstanceName && inst.FBType == currentFB);
                var fbType = fbInst.InstanceType ?? currentFB;
                Variable foundVar = _storage.Variables.FirstOrDefault(v => v.FBType == fbType && v.Name == connectionPoint.Variable);
                return foundVar;
            }

            private static EventInstance _findEvent(ConnectionNode connectionPoint, string currentFB, Storage _storage)
            {
                FBInstance fbInst = _storage.Instances.FirstOrDefault(inst => inst.Name == connectionPoint.InstanceName && inst.FBType == currentFB);
                var fbType = fbInst?.InstanceType ?? currentFB;
                Event foundEvent = _storage.Events.FirstOrDefault(e => e.FBType == fbType && e.Name == connectionPoint.Variable);
                return new EventInstance(foundEvent, fbInst);
            }

            public static string ComponentEventOutputs(
                IEnumerable<Connection> connections, 
                Storage _storage,
                bool useProcesses)
            {
                string outputsString = "-- ComponentEventOutputs\n";
                if (useProcesses)
                {
                    foreach (string output in _getInternalEventOutputStrings(connections, _storage))
                    {
                        outputsString += String.Format(Smv.NextVarAssignment, output, Smv.False);
                    }
                }
                else
                {
                    foreach (var output in _getInternalEventOutputs(connections, _storage))
                    {
                        string setRule = String.Format("\t{0}.{1}_set : {2};\n", output.Instance.Name, output.ParameterName(), Smv.True);
                        string resetRule = String.Format("\t{0} : {1};\n", Smv.True, Smv.False);
                        outputsString += String.Format(Smv.EmptyNextCaseBlock, output.Value(), setRule + resetRule);

                        if (output.Event.Timed)
                        {
                            string setLastRule = String.Format("\t{0}.{1}_set : {2};\n", output.Instance.Name, output.ParameterName(), "systemclock");
                            string resetLastRule = String.Format("\t{0} : {1};\n", Smv.True, output.TSLast());
                            outputsString += String.Format(Smv.EmptyNextCaseBlock, output.TSLast(), setLastRule + resetLastRule);
                        
                            string setBornRule = String.Format("\t{0}.{1}_set : {0}.INVOKEDBY.ts_born;\n", output.Instance.Name, output.ParameterName());
                            string resetBornRule = String.Format("\t{0} : {1};\n", Smv.True, output.TSBorn());
                            outputsString += String.Format(Smv.EmptyNextCaseBlock, output.TSBorn(), setBornRule + resetBornRule);

                        }   
                    }
                }
                return outputsString;
            }
            public static string DefineOmega(IEnumerable<Connection> internalBuffers, Storage storage)
            {
                string omega = "";
                foreach (string output in _getInternalEventOutputStrings(internalBuffers, storage))
                {
                    omega += output + Smv.Or;
                }
                omega = Smv.Not + "(" + omega.TrimEnd(Smv.OrTrimChars) + ")";
                return String.Format(Smv.DefineBlock, Smv.Omega, omega);
            }
            public static string DefinePhi(IEnumerable<FBInstance> instances, IEnumerable<Event> nonFilteredEvents)
            {
                string phi = "(" + Smv.Not + Smv.ExistsInputEvent + ")" + Smv.And;
                string phiEvents = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceEvents = nonFilteredEvents.Where(ev => ev.FBType == instance.InstanceType && ev.Direction != Direction.Internal);
                    foreach (Event ev in instanceEvents)
                    {
                        phiEvents += new EventInstance(ev, instance).Value() + Smv.Or;
                    }
                }
                phi += String.Format("({0}({1}))", Smv.Not, phiEvents.Trim(Smv.OrTrimChars));
                return String.Format(Smv.DefineBlock, Smv.Phi, phi);
            }
            public static string InputEventsResetRules(IEnumerable<Event> events, bool useProcesses)
            {
                string rules = "";
                foreach (Event ev in events.Where(ev => ev.Direction == Direction.Input))
                {
                    EventInstance eventInstance = new EventInstance(ev, null);   
                    if (useProcesses)
                    {
                        string resetRule = "\t" + Smv.Alpha + " : " + Smv.False + ";\n";
                        rules += String.Format(Smv.NextCaseBlock, eventInstance.Value(), resetRule);
                    }
                    else
                    {
                        rules += String.Format(Smv.DefineBlock, eventInstance.SmvName() + "_reset", Smv.Alpha);
                    }
                }
                return rules;
            }

            private static MultiMap<EventInstance, EventInstance> _getEventConnectionsMap(IEnumerable<Connection> internalBuffers, Storage _storage)
            {
                var eventConnectionsMap = new MultiMap<EventInstance, EventInstance>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    
                    var dst = _findEvent(connection.Destination, connection.FBType, _storage);
                    var src = _findEvent(connection.Source, connection.FBType, _storage);
                    eventConnectionsMap.Add(dst, src);
                }
                return eventConnectionsMap;
            }
            private static IEnumerable<Event> _getSamplingEventNamesForVariable(
                string varName,
                IEnumerable<WithConnection> withConnections,
                IEnumerable<Event> events)
            {
                return withConnections.Where(c => c.Var == varName)
                    .Select(conn => events.FirstOrDefault(e => e.Name == conn.Event && e.FBType == conn.FBType))
                    .ToList();
            }
            private static IEnumerable<string> _getInternalEventOutputStrings(IEnumerable<Connection> internalBuffers,
                Storage storage)
            {
                HashSet<string> internalEventOutputs = new HashSet<string>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    EventInstance srcEvent = _findEvent(connection.Source, connection.FBType, storage);
                    string srcSmvVar = srcEvent.Value();
                    if (!internalEventOutputs.Contains(srcSmvVar) && srcEvent.IsComponentVar())
                    {
                        internalEventOutputs.Add(srcSmvVar);
                    }
                }
                return internalEventOutputs;
            }

            private static IEnumerable<EventInstance> _getInternalEventOutputs(IEnumerable<Connection> internalBuffers, Storage _storage)
            {
                var internalEventOutputs = new HashSet<EventInstance>();
                foreach (Connection connection in internalBuffers.Where(conn => conn.Type == ConnectionType.Event))
                {
                    var eventInstance = _findEvent(connection.Source, connection.FBType, _storage);
                    if (!internalEventOutputs.Contains(eventInstance) && eventInstance.IsComponentVar())
                    {
                        internalEventOutputs.Add(eventInstance);
                    }
                }
                return internalEventOutputs;
            }

            public static string NonConnectedInputs(IEnumerable<Connection> connections, IEnumerable<Variable> allVariables, IEnumerable<FBInstance> instances)
            {
                string ret = "";
                foreach (FBInstance instance in instances)
                {
                    var instanceVariables = allVariables.Where(v => v.FBType == instance.InstanceType && v.Direction == Direction.Input && !v.IsConstant);

                    foreach (Variable variable in instanceVariables)
                    {
                        if (_nonInitializableVar(variable)) continue; // do not add "next" statement for FB_DELAY and E_CYCLE data IOs
                        Connection connection = connections.FirstOrDefault(conn => conn.Destination.InstanceName == instance.Name 
                                                                                   && conn.Destination.Variable == variable.Name);
                        if (connection != null) continue;

                        if (variable.ArraySize == 0)
                        {
                            ret += String.Format(Smv.NextVarAssignment, instance.Name + "_" + variable.Name, instance.Name + "_" + variable.Name);
                        }
                        else
                        {
                            for (int i = 0; i < variable.ArraySize; i++)
                            {
                                ret += String.Format(Smv.NextVarAssignment, instance.Name + "_" + variable.Name + Smv.ArrayIndex(i), instance.Name + "_" + variable.Name + Smv.ArrayIndex(i));
                            }
                        }
                    }
                }
                return ret;
            }

            public static string InitNonTimedEvents(IEnumerable<Event> events)
            {
                string result = "";
                foreach (Event ev in events.Where(ev => !ev.Timed))
                {
                    result += String.Format(Smv.VarInitializationBlock, EventInstance.ParameterName(ev), Smv.False);
                }


                return result;
            }
        }

    }
}