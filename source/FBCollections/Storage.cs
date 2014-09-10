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