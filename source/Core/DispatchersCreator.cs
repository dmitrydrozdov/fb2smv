//TODO: remove this and move dispatcher to execution model
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2SMV.FBCollections;

namespace FB2SMV
{
    namespace Core
    {
        public class DispatchersCreator
        {
            public static List<IDispatcher> Create(IEnumerable<FBType> compositeBlockTypes, IEnumerable<FBInstance> instances, bool solveDispatchingProblem)
            {
                List<IDispatcher> dispatchers = new List<IDispatcher>();
                if (compositeBlockTypes.Any())
                {
                    dispatchers = new List<IDispatcher>();
                    foreach (FBType compositeBlock in compositeBlockTypes)
                    {
                        IEnumerable<FBInstance> curFbInstances = instances.Where((inst) => inst.FBType == compositeBlock.Name);
                        dispatchers.Add(new CyclicDispatcher(compositeBlock.Name, curFbInstances, solveDispatchingProblem));
                    }
                }
                return dispatchers;
            } 
        }
    }
}
