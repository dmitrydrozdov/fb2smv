/*
 * Each function block can be coddected to it's own execution model,
 * which determines events selection, variables sampling, and dispatchers
 * in composite function blocks.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2SMV.FBCollections;

namespace FB2SMV
{
    namespace Core
    {
        [Serializable]
        public class ExecutionModel
        {
            public ExecutionModel(string fbTypeName)
            {
                FBTypeName = fbTypeName;
            }
            private List<PriorityEvent> _inputEventsPriorities = new List<PriorityEvent>();
            public string FBTypeName { get; private set; }
            public IDispatcher Dispatcher = null;

            public IEnumerable<PriorityEvent> InputEventsPriorities
            {
                get { return _inputEventsPriorities; }
            }
            public void SortInputEvents()
            {
                _inputEventsPriorities.Sort((a, b) => ServiceClasses.Comparisions.IntCompareGreater(a.Priority, b.Priority));
            }

            public void AddInputPriorityEvent(PriorityEvent ev)
            {
                _inputEventsPriorities.Add(ev);
            }
        }
        [Serializable]
        public class PriorityEvent : PriorityContainer<Event>
        {
            public PriorityEvent(int priority, Event ev)
                : base(priority, ev)
            { }

            public override string ToString()
            {
                return String.Format("{0} - {1}", Priority, Value.Name);
            }
        }

        public class ExecutionModelsList
        {
            public static List<ExecutionModel> Generate(FBClassParcer parcer, bool solveDispatchingProblem)
            {
                List<ExecutionModel> outList = new List<ExecutionModel>();
                foreach (FBType fbType in parcer.Storage.Types)
                {
                    ExecutionModel em = new ExecutionModel(fbType.Name);
                    int basicPriority = 0;
                    foreach (Event ev in parcer.Storage.Events.Where(ev => ev.FBType == fbType.Name && ev.Direction == Direction.Input))
                    {
                        em.AddInputPriorityEvent(new PriorityEvent(basicPriority++, ev));
                    }
                    if (fbType.Type == FBClass.Composite)
                    {
                        //create dispatcher
                        IEnumerable<FBInstance> curFbInstances = parcer.Storage.Instances.Where((inst) => inst.FBType == fbType.Name);
                        em.Dispatcher = new CyclicDispatcher(fbType.Name, curFbInstances, solveDispatchingProblem);
                    }

                    outList.Add(em);
                }
                return outList;
            }
        }
    }
}
