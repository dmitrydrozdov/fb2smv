﻿using System;
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
            private ShowMessageDelegate _showMessage = null;
            public SmvCodeGenerator(Storage storage, IEnumerable<ExecutionModel> executionModels, Settings settings, ShowMessageDelegate showMessage)
            {
                _storage = storage;
                _executionModels = executionModels;
                _settings = settings;
                _showMessage = showMessage;
            }

            public int fbTypeCompare(FBType a, FBType b)
            {
                if(a.Type < b.Type) return -1;
                else if (a.Type > b.Type) return 1;
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
                else if(fbType.Type == FBClass.Composite) return TranslateCompositeFB(fbType);
                else if (fbType.Type == FBClass.Library) return TranslateLibraryFBType(fbType);
                else throw new Exception("Unsupported FB class! " + fbType.Type);
            }

            public string TranslateLibraryFBType(FBType fbType)
            {
                if(fbType.Name == LibraryTypes.E_SPLIT)
                {
                    return LibraryFBTypes.ESplitFBModule(_storage, _settings);
                }
                if (fbType.Name == LibraryTypes.E_DELAY)
                {
                    return LibraryFBTypes.EDelayFBModule(_storage, _settings);
                }
                else if (fbType.Name == LibraryTypes.E_CYCLE)
                {
                    return LibraryFBTypes.ECycleFBModule(_storage, _settings);
                }
                _showMessage(String.Format("Warning! Library type {0} is not supported for current execution model. Dummy FB module was generated.", fbType.Name));
                return emptyFbModule(fbType.Name);
            }

            private string emptyFbModule(string moduleName)
            {
                string smvModule = "";
                smvModule += FbSmvCommon.SmvModuleDeclaration(new List<Event>(), new List<Variable>(), moduleName);
                smvModule += FbSmvCommon.ModuleFooter(_settings) + "\n";

                return smvModule;
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

                if (_settings.UseProcesses)
                {
                    smvModule += CompositeFbSmv.NonConnectedInstanceOutputEvents(_storage.Events, instances, connections);
                    smvModule += CompositeFbSmv.ComponentDataOutputNextStatements(_storage.Variables, instances);
                }
                //smvModule += _moduleVariablesInitBlock(variables) + "\n";
                //smvModule += _inputVariablesSampleComposite(variables, withConnections) + "\n";
                smvModule += CompositeFbSmv.NonConnectedEventInputs(connections, _storage.Events, instances, _showMessage);
                smvModule += CompositeFbSmv.NonConnectedInputs(connections, _storage.Variables, instances);
                smvModule += CompositeFbSmv.InternalDataConnections(connections, withConnections, _storage.Variables, instances) + "\n";
                smvModule += CompositeFbSmv.ComponentEventOutputs(connections, _settings.UseProcesses) + "\n";
                //smvModule += _eventInputsResetRules(events) + "\n";
                smvModule += "\n-- ---DISPATCHER--- --\n";
                smvModule += "-- *************** --\n";
                smvModule += dispatcher.GetSmvCode(_settings.UseProcesses) + "\n";

                smvModule += CompositeFbSmv.InternalEventConnections(connections, _settings.UseProcesses) + "\n";
                smvModule += CompositeFbSmv.InputEventsResetRules(events, _settings.UseProcesses);
                smvModule += FbSmvCommon.DefineExistsInputEvent(events) + "\n";
                smvModule += CompositeFbSmv.DefineOmega(connections) + "\n";
                smvModule += CompositeFbSmv.DefinePhi(instances, _storage.Events) + "\n"; //phi variable for timed models

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
                var ndt = _storage.Ndts.FirstOrDefault(ev => ev.FBType == fbType.Name);

                smvModule += BasicFbSmv.ModuleHeader(events, variables, fbType.Name);

                smvModule += BasicFbSmv.OsmStatesDeclaration();
                smvModule += BasicFbSmv.EccStatesDeclaration(states) + "\n";
                smvModule += BasicFbSmv.EcActionsCounterDeclaration(states);
                smvModule += BasicFbSmv.AlgStepsCounterDeclaration(smvAlgs);

                if (ndt != null && ndt.Name == "NDT")
                    smvModule += BasicFbSmv.NdtDeclaration(ndt);

                smvModule += Smv.Assign;
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.EccStateVar, Smv.EccState(states.First(s => true).Name));
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.OsmStateVar, Smv.Osm.S0);
                smvModule += BasicFbSmv.ModuleVariablesInitBlock(variables) + "\n";
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.EcActionsCounterVar, "0");
                smvModule += String.Format(Smv.VarInitializationBlock, Smv.AlgStepsCounterVar, "0");

                if (ndt != null && ndt.Name == "NDT")
                    smvModule += BasicFbSmv.NdtInitBlock(ndt);

                if (ndt != null && ndt.Name == "NDT")
                    smvModule += BasicFbSmv.NdtChangeBlock(transitions);

                smvModule += BasicFbSmv.EcStateChangeBlock(transitions, events);
                smvModule += Smv.OsmStateChangeBlock + "\n";
                smvModule += BasicFbSmv.EcActionsCounterChangeBlock(states) + "\n";
                smvModule += BasicFbSmv.AlgStepsCounterChangeBlock(states, actions, smvAlgs) + "\n";

                smvModule += BasicFbSmv.InputVariablesSampleBasic(variables, withConnections) + "\n";
                smvModule += BasicFbSmv.OutputVariablesChangingRules(variables, actions, _storage.AlgorithmLines.Where(line => line.FBType == fbType.Name), _settings) + "\n";
                smvModule += BasicFbSmv.SetOutputVarBuffers(variables, events, actions, withConnections, _showMessage) + "\n";
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

            public string GetTimeScheduler()
            {
                if (_storage.TimersCount == 0) return ""; //return empty string if there is no timers
                string smvType = _storage.TimeSMVType;
                int timeLocalRangeTop = 100;
                string[] timeRangeSplit = smvType.Split(new string[] {".."}, StringSplitOptions.RemoveEmptyEntries);
                if(timeRangeSplit.Count() > 1)
                {
                    timeLocalRangeTop = Convert.ToInt32(timeRangeSplit[1]);
                }
                TimeScheduler ts = new TimeScheduler(_storage.TimersCount, smvType, timeLocalRangeTop);
                return ts.GetSMVCode(_storage.Tmax);
            }

            public string GenerateMain()
            {
                string mainModule = "";
                FBType topLevelFbType = _storage.Types.FirstOrDefault(fbType => fbType.IsRoot);
                if (topLevelFbType == null) throw new ArgumentNullException("Can't find root FB type");

                List<FBInstance> instanceList = new List<FBInstance>();
                List<Connection> connections = new List<Connection>();
                List<InstanceParameter> instanceParameters = new List<InstanceParameter>();
                FBInstance instance = new FBInstance(topLevelFbType.Name + "_inst", topLevelFbType.Name, "Top-level FB instance", "main");
                instanceList.Add(instance);

                mainModule += String.Format(Smv.ModuleDef, "main", "");
                mainModule += CompositeFbSmv.FbInstances(instanceList, _storage.Events, _storage.Variables, connections, _settings) + "\n";
                mainModule += CompositeFbSmv.InternalBuffersDeclaration(instanceList, connections, _storage.Events, _storage.Variables) + "\n";
                if (_settings.GenerateDummyProperty)
                {
                    mainModule += String.Format(Smv.VarDeclarationBlock, "false_var", Smv.DataTypes.BoolType);
                }
                mainModule += Smv.Assign;
                if (_settings.GenerateDummyProperty)
                {
                    mainModule += String.Format(Smv.VarInitializationBlock, "false_var", Smv.False);
                    mainModule += String.Format(Smv.NextVarAssignment, "false_var", Smv.False);
                }
                mainModule += CompositeFbSmv.InternalBuffersInitialization(instanceList, connections, _storage.Events, _storage.Variables, instanceParameters, true) + "\n";
                
                mainModule += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Alpha, Smv.True);
                mainModule += String.Format(Smv.VarInitializationBlock, instance.Name + "_" + Smv.Beta, Smv.False);
                    

                //Main module next blocks
                //**********************
                foreach (Variable variable in _storage.Variables.Where(v=>v.FBType == topLevelFbType.Name && v.Direction == Direction.Input && !v.IsConstant))
                {
                    string smvVariable = instance.Name + "_" + variable.Name;
                    if (variable.ArraySize == 0)
                    {
                        mainModule += String.Format(Smv.NextVarAssignment, smvVariable, smvVariable);
                    }
                    else
                    {
                        for (int i = 0; i < variable.ArraySize; i++)
                        {
                            mainModule += String.Format(Smv.NextVarAssignment, smvVariable + Smv.ArrayIndex(i), smvVariable + Smv.ArrayIndex(i));
                        }
                    }
                }
                foreach (Event ev in _storage.Events.Where(ev => ev.FBType == topLevelFbType.Name))
                {
                    string smvVariable = instance.Name + "_" + ev.Name;
                    string nextRule = "";
                    if (ev.Direction == Direction.Output)
                        nextRule = instance.Name + "." + "event_" + ev.Name + "_set : " + Smv.True + ";\n";
                    else
                        nextRule = instance.Name + "." + "event_"+ev.Name+"_reset : "+Smv.False + ";\n";
                    mainModule += String.Format(Smv.NextCaseBlock, smvVariable, nextRule);
                }

                string alphaRule = "\t" + instance.Name + "_" + Smv.Beta + " : " + Smv.True + ";\n" +
                                    "\t" + instance.Name + ".alpha_reset : " + Smv.False + ";\n";
                string betaRule = "\t" + instance.Name + "_" + Smv.Beta + " : " + Smv.False + ";\n" +
                                    "\t" + instance.Name + ".beta_set : " + Smv.True + ";\n";
                mainModule += String.Format(Smv.NextCaseBlock, instance.Name + "_" + Smv.Alpha, alphaRule);
                mainModule += String.Format(Smv.NextCaseBlock, instance.Name + "_" + Smv.Beta, betaRule);
                //**********************

                //mainModule += FbSmvCommon.ModuleFooter(_settings) + "\n";

                if (_settings.GenerateDummyProperty)
                {
                    mainModule += "\nLTLSPEC F false_var=TRUE";
                }

                return mainModule;
            }

            public void Check()
            {
                //TODO
                //CheckRanges();
            }
        }
    }
}