using FB2SMV.FBCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {

        public class PriorityInstance : IPriorityInstance
        {
            private int _priority;
            private FBInstance _instance;

            public PriorityInstance(int priority, FBInstance instance)
            {
                _priority = priority;
                _instance = instance;
            }

            public int Priority
            {
                set { _priority = value; }
                get { return _priority; }
            }

            public FBInstance Instance
            {
                get { return _instance; }
            }

            public override string ToString()
            {
                return String.Format("{0} - {1}", _priority, _instance.Name);
            }
        }

        public class CyclicDispatcher : IDispatcher
        {
            public CyclicDispatcher(string fbTypeName, IEnumerable<FBInstance> instances, bool solveDispatchingProblem)
            {
                FBTypeName = fbTypeName;
                int basicPriority = 0;
                foreach (FBInstance fbInstance in instances)
                {
                    _instances.Add(new PriorityInstance(basicPriority++, fbInstance));
                }
                _solveDispatchingProblem = solveDispatchingProblem;
            }

            int intCompareLess(int a, int b)
            {
                return a == b ? 0 : (a > b ? 1 : -1);
            }
            public string GetSmvCode()
            {
                string smvDispatcher = "";

                //string prevAlpha = null;
                string prevBeta = Smv.Alpha;
                bool firstBlock = true;
                SortInstances();
                foreach (PriorityInstance priorityInstance in _instances)
                {
                    string alphaVar = priorityInstance.Instance.Name + "_" + Smv.Alpha;
                    string betaVar = priorityInstance.Instance.Name + "_" + Smv.Beta;

                    if (_solveDispatchingProblem){
                        smvDispatcher += String.Format(Smv.NextCaseBlock, alphaVar, "\t" + prevBeta + Smv.And + Smv.Omega + (firstBlock ? Smv.And + Smv.Not + Smv.ExistsInputEvent : "") + " : " + Smv.True + ";\n");
                        firstBlock = false;
                    }
                    else
                    {
                        smvDispatcher += String.Format(Smv.NextCaseBlock, alphaVar, "\t" + prevBeta + Smv.And + Smv.Omega + Smv.And + Smv.Not + Smv.ExistsInputEvent + " : " + Smv.True + ";\n");
                    }
                    smvDispatcher += String.Format(Smv.NextCaseBlock, betaVar, "\t" + betaVar + Smv.And + Smv.Omega + " : " + Smv.False + ";\n");
                    prevBeta = betaVar;
                }

                smvDispatcher += String.Format(Smv.NextCaseBlock, Smv.Alpha, "\t" + Smv.Alpha + Smv.And + Smv.Omega + Smv.And + (Smv.Not + Smv.ExistsInputEvent) + " : " + Smv.False + ";\n");
                smvDispatcher += String.Format(Smv.NextCaseBlock, Smv.Beta, "\t" + prevBeta + Smv.And + Smv.Omega + " : " + Smv.True + ";\n");
                return smvDispatcher;
            }

            public string FBTypeName { get; private set; }

            public void SortInstances()
            {
                _instances.Sort((a, b) => intCompareLess(a.Priority, b.Priority));
            }

            public IEnumerable<IPriorityInstance> Instances
            {
                get { return _instances; }
            }

            private List<PriorityInstance> _instances = new List<PriorityInstance>();
            private bool _solveDispatchingProblem;
        }
    }
}
