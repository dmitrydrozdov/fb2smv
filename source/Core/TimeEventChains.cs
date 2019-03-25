using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FB2SMV.FBCollections;

namespace Core
{
    public class TimeEventChains
    {

        private Storage _storage;
        private HashSet<Event> timedEvents;

        public TimeEventChains(Storage storage)
        {
            _storage = storage;
        }


        public void Construct()
        {
            timedEvents = new HashSet<Event>();
            var prevFound = -1;
            var foundEvents = 0;

            while (prevFound != foundEvents)
            {
                IEnumerable<ECAction> ecActions; 
                if (timedEvents.Count == 0)
                {
                    var timeUsages = findTimeUsages();
                    var algorithms = _storage.Algorithms.Where(alg => 
                        timeUsages.Any(tu => tu.FBType == alg.FBType && tu.AlgorithmName == alg.Name));
            
                    ecActions = _storage.EcActions.Where(action => 
                        algorithms.Any(alg => alg.FBType == action.FBType && action.Algorithm == alg.Name));   
                }
                else
                {
                    ecActions = _storage.EcActions.Where(action =>
                        _storage.Events.Any(ev =>
                            action.FBType == ev.FBType && action.Output == ev.Name && timedEvents.Contains(ev)));
                }
                
                var ecStates = _storage.EcStates.Where(state =>
                    ecActions.Any(action => action.FBType == state.FBType && action.ECState == state.Name));
            
                var searchQueue = new Queue<ECState>();
                var discovered = new HashSet<ECState>();
                foreach (ECState state in ecStates)
                {
                    searchQueue.Enqueue(state);
                    discovered.Add(state);
                }

                while (searchQueue.Any())
                {
                    ECState state = searchQueue.Dequeue();
                    var ecTransitions = _storage.EcTransitions.Where(transition =>
                        state.FBType == transition.FBType && transition.Destination == state.Name);
                    foreach (ECTransition transition in ecTransitions)
                    {
                        var conditionEvents = getEventsInCondition(transition.Condition, _storage.Events.Where(ev => ev.FBType == transition.FBType));
                        if (!conditionEvents.Any())
                        {
                            ECState sourceState = _storage.EcStates.First(ecState =>
                                ecState.FBType == transition.FBType && ecState.Name == transition.Source);
                            if (!discovered.Contains(sourceState))
                            {
                                searchQueue.Enqueue(sourceState);   
                            }
                        }
                        else
                        {
                            foreach (Event conditionEvent in conditionEvents)
                            {
                                timedEvents.Add(conditionEvent);
                                
                                if (conditionEvent.Direction != Direction.Input) continue;
                                foreach (var sourceEventInstance in findSourceEvents(conditionEvent, _storage))
                                {
                                    timedEvents.Add(sourceEventInstance.Item1);
                                    if (sourceEventInstance.Item1.FBType == "E_DELAY" || sourceEventInstance.Item1.FBType == "E_CYCLE")
                                    {
                                        var eStart = _storage.Events.First(ev =>
                                            ev.Name == "START" && ev.FBType == sourceEventInstance.Item1.FBType);
                                        timedEvents.Add(eStart);
                                        foreach (var sourceEvent in findSourceEvents(eStart, _storage))
                                        {
                                            timedEvents.Add(sourceEvent.Item1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                prevFound = foundEvents;
                foundEvents = timedEvents.Count;
            }
            foreach (Event timedEvent in timedEvents)
            {
                timedEvent.Timed = true;
            }

        }

        private IEnumerable<Tuple<Event, string>> findSourceEvents(Event inputEvent, Storage storage)
        {
            return storage.Connections.Where(conn =>
                conn.Destination.FbType == inputEvent.FBType && conn.Destination.Variable == inputEvent.Name)
                .Select(conn => Tuple.Create(storage.Events.First(ev => ev.FBType == conn.Source.FbType && ev.Name == conn.Source.Variable), conn.Source.InstanceName))
                .Distinct();
        }

        private IEnumerable<Event> getEventsInCondition(string transitionCondition, IEnumerable<Event> allEvents)
        {
            
            var result =  new HashSet<Event>();
           
            Regex evNamesRegex = new Regex(@"(\w+)");

            string[] strSplit = evNamesRegex.Split(transitionCondition);

            for (int i = 0; i < strSplit.Count(); i++)
            {
                var foundEvent = allEvents.FirstOrDefault(ev => ev.Name == strSplit[i]);
                if (foundEvent != null)
                {
                   result.Add(foundEvent);
                }
            }

            return result;
        }

        private IEnumerable<AlgorithmLine> findTimeUsages()
        {   
            return _storage.AlgorithmLines.Where(line => HasTimeUsage(line.Value) || HasTimeUsage(line.Condition));
        }

        private static bool HasTimeUsage(string STLine)
        {
            return STLine.Contains("INVOKEDBY");
        }
    }
}