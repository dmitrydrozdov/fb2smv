using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        public interface IDispatcher
        {
            string GetSmvCode();
            string FBTypeName { get; }

            void SortInstances();

            IEnumerable<IPriorityContainer> Instances { get; }
        }
    }
}
