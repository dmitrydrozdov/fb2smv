using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace FBCollections
    {
        [Serializable]
        public class Storage
        {
            public Storage()
            {
            }

            public void PutFBType(FBType fbType)
            {
                Types.Add(fbType);
            }

            public void PutEvent(Event ev)
            {
                Events.Add(ev);
            }

            public void PutVariable(Variable variable)
            {
                Variables.Add(variable);
            }

            public void PutState(ECState state)
            {
                EcStates.Add(state);
            }

            public void PutECTransition(ECTransition transition)
            {
                EcTransitions.Add(transition);
            }

            public void PutECAction(ECAction action)
            {
                EcActions.Add(action);
            }

            public void PutAlgorithm(Algorithm algorithm)
            {
                Algorithms.Add(algorithm);
            }

            public void PutAlgorithmLine(AlgorithmLine line)
            {
                AlgorithmLines.Add(line);
            }

            public void PutWithConnection(WithConnection connection)
            {
                WithConnections.Add(connection);
            }

            public void PutFBInstance(FBInstance instance)
            {
                Instances.Add(instance);
            }

            public void PutConnection(Connection connection)
            {
                Connections.Add(connection);
            }

            public void PutInstanceParameter(InstanceParameter instanceParameter)
            {
                InstanceParameters.Add(instanceParameter);
            }

            public int TimersCount
            {
                get
                {
                    if(_timersCount < 0) _timersCount = Instances.Where((FBInstance i) => i.InstanceType == "E_DELAY" || i.InstanceType == "E_CYCLE").Count();
                    return _timersCount;
                }
                set
                {
                    _timersCount = value;
                }
            }

            public string TimeSMVType
            {
                get { return _timeType; }
                set { _timeType = value; }
            }

            public int Tmax
            {
                get { return _tmax; }
                set { _tmax = value; }
            }

            private int _timersCount = -1;
            private string _timeType = "integer";
            private int _tmax = 100;

            public readonly List<FBType> Types = new List<FBType>();
            public readonly List<Event> Events = new List<Event>();
            public readonly List<Variable> Variables = new List<Variable>();
            public readonly List<ECState> EcStates = new List<ECState>();
            public readonly List<ECTransition> EcTransitions = new List<ECTransition>();
            public readonly List<ECAction> EcActions = new List<ECAction>();
            public readonly List<Algorithm> Algorithms = new List<Algorithm>();
            public readonly List<WithConnection> WithConnections = new List<WithConnection>();
            public readonly List<FBInstance> Instances = new List<FBInstance>();
            public readonly List<Connection> Connections = new List<Connection>();
            public readonly List<AlgorithmLine> AlgorithmLines = new List<AlgorithmLine>();
            public readonly List<InstanceParameter> InstanceParameters = new List<InstanceParameter>(); 
        }
    }
}