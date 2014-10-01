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
    }
}