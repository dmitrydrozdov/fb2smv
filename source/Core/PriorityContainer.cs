using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        public interface IPriorityContainer
        {
            int Priority { set; get; }
        }
        [Serializable]
        public class PriorityContainer<T> : IPriorityContainer
        {
            private int _priority;
            private T _value;

            public PriorityContainer(int priority, T value)
            {
                _priority = priority;
                _value = value;
            }

            public int Priority
            {
                set { _priority = value; }
                get { return _priority; }
            }

            public T Value
            {
                get { return _value; }
            }
        }
    }
}
