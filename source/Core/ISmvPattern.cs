using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace Core
    {
        public interface ISmvPattern //SMV code patterns interface.
        {
            string True { get; }
            string False { get; }
            string ModuleDef { get; }
            string VarDeclarationBlock { get; }
            string VarInitializationBlock { get; }
            string DefineBlock { get; }
            string OsmStateVar { get; }
            string EccStateVar { get; }
            string EcActionsCounterVar { get; }
            string AlgStepsCounterVar { get; }
            string NextCaseBlock { get; }
            string EmptyNextCaseBlock { get; }
            string ExistsInputEvent { get; }
            string ExistsEnabledEcTran { get; }
            string AbsentsEnabledEcTran { get; }
            string Alpha { get; }
            string Beta { get; }
            string Omega { get; }
            string Phi { get; }
            string Assign { get; }
            string Running { get; }
            string NormalVarAssignment { get; }
            string NextVarAssignment { get; }
            char ConnectionNameSeparator { get; }
            string And { get; }
            string Or { get; }
            string Not { get; }
            string Fairness { get; }

            string NdtInitializationBlock { get; }
        }
    }
}
