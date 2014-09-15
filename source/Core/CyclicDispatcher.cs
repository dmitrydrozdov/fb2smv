using FB2SMV.FBCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        [Serializable]
        public class PriorityInstance : PriorityContainer<FBInstance>
        {
            public PriorityInstance(int priority, FBInstance instance)
                : base(priority, instance)
            {}

            public override string ToString()
            {
                return String.Format("{0} - {1}", Priority, Value.Name);
            }
        }

        [Serializable]
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

            public string GetSmvCode(bool useProcesses)
            {
                string smvDispatcher = "";

                //string prevAlpha = null;
                string prevBeta = Smv.Alpha;
                bool firstBlock = true;
                SortInstances();
                foreach (PriorityInstance priorityInstance in _instances)
                {
                    string alphaVar = priorityInstance.Value.Name + "_" + Smv.Alpha;
                    string betaVar = priorityInstance.Value.Name + "_" + Smv.Beta;

                    string instanceBetaSet = "";
                    string instanceAlphaReset = "";
                    if (!useProcesses)
                    {
                        instanceBetaSet = String.Format("\t{0}.alpha_beta : {1};\n", priorityInstance.Value.Name, Smv.True);
                        instanceAlphaReset = String.Format("\t{0}.alpha_beta : {1};\n", priorityInstance.Value.Name, Smv.False);
                    }

                    if (_solveDispatchingProblem){
                        smvDispatcher += String.Format(Smv.NextCaseBlock, alphaVar, "\t" + prevBeta + Smv.And + Smv.Omega + (firstBlock ? Smv.And + Smv.Not + Smv.ExistsInputEvent : "") + " : " + Smv.True + ";\n" + instanceAlphaReset);
                        firstBlock = false;
                    }
                    else
                    {
                        smvDispatcher += String.Format(Smv.NextCaseBlock, alphaVar, "\t" + prevBeta + Smv.And + Smv.Omega + Smv.And + Smv.Not + Smv.ExistsInputEvent + " : " + Smv.True + ";\n" + instanceAlphaReset);
                    }
                    smvDispatcher += String.Format(Smv.NextCaseBlock, betaVar, "\t" + betaVar + Smv.And + Smv.Omega + " : " + Smv.False + ";\n" + instanceBetaSet);
                    prevBeta = betaVar;
                }

                smvDispatcher += String.Format(Smv.NextCaseBlock, Smv.Alpha, "\t" + Smv.Alpha + Smv.And + Smv.Omega + Smv.And + (Smv.Not + Smv.ExistsInputEvent) + " : " + Smv.False + ";\n");
                smvDispatcher += String.Format(Smv.NextCaseBlock, Smv.Beta, "\t" + prevBeta + Smv.And + Smv.Omega + " : " + Smv.True + ";\n");
                return smvDispatcher;
            }

            public string FBTypeName { get; private set; }

            public void SortInstances()
            {
                _instances.Sort((a, b) => ServiceClasses.Comparisions.IntCompareGreater(a.Priority, b.Priority));
            }

            public IEnumerable<IPriorityContainer> Instances
            {
                get { return _instances; }
            }

            private List<PriorityInstance> _instances = new List<PriorityInstance>();
            private bool _solveDispatchingProblem;
        }
    }
}
