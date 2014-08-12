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
    }
}
