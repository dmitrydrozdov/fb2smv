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
                string prevBeta = CmSmv.Alpha;
                bool firstBlock = true;
                foreach (FBInstance instance in _instances)
                {
                    string alphaVar = instance.Name + "_" + CmSmv.Alpha;
                    string betaVar = instance.Name + "_" + CmSmv.Beta;

                    if (_solveDispatchingProblem){
                        smvDispatcher += String.Format(CmSmv.NextCaseBlock, alphaVar, "\t" + prevBeta + CmSmv.And + CmSmv.Omega + (firstBlock ? CmSmv.And + CmSmv.Not + CmSmv.ExistsInputEvent : "") + " : " + CmSmv.True + ";\n");
                        firstBlock = false;
                    }
                    else
                    {
                        smvDispatcher += String.Format(CmSmv.NextCaseBlock, alphaVar, "\t" + prevBeta + CmSmv.And + CmSmv.Omega + CmSmv.And + CmSmv.Not + CmSmv.ExistsInputEvent + " : " + CmSmv.True + ";\n");
                    }
                    smvDispatcher += String.Format(CmSmv.NextCaseBlock, betaVar, "\t" + betaVar + CmSmv.And + CmSmv.Omega + " : " + CmSmv.False + ";\n");
                    prevBeta = betaVar;
                }

                smvDispatcher += String.Format(CmSmv.NextCaseBlock, CmSmv.Alpha, "\t" + CmSmv.Alpha + CmSmv.And + CmSmv.Omega + CmSmv.And + (CmSmv.Not + CmSmv.ExistsInputEvent) + " : " + CmSmv.False + ";\n");
                smvDispatcher += String.Format(CmSmv.NextCaseBlock, CmSmv.Beta, "\t" + prevBeta + CmSmv.And + CmSmv.Omega + " : " + CmSmv.True + ";\n");
                return smvDispatcher;
            }

            private IEnumerable<FBInstance> _instances;
            private bool _solveDispatchingProblem;
        }
    }
}
