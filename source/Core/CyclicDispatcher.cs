using FB2SMV.FBCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        class CyclicDispatcher : IDispatcher
        {
            public CyclicDispatcher(IEnumerable<FBInstance> instances, bool solveDispatchingProblem)
            {
                _instances = instances;
                _solveDispatchingProblem = solveDispatchingProblem;
            }
            public string GetSmvCode()
            {
                string smvDispatcher = "";

                //string prevAlpha = null;
                string prevBeta = Smv.Alpha;
                bool firstBlock = true;
                foreach (FBInstance instance in _instances)
                {
                    string alphaVar = instance.Name + "_" + Smv.Alpha;
                    string betaVar = instance.Name + "_" + Smv.Beta;

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

            private IEnumerable<FBInstance> _instances;
            private bool _solveDispatchingProblem;
        }
    }
}
