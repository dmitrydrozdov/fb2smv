using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FB2SMV.FBCollections;

namespace FB2SMV
{
    namespace Core
    {
        class CmSmvPattern : ISmvPattern //SMV code patterns  for Carnegie-Melon syntax
        {
            private static string _true = "TRUE";
            private static string _false = "FALSE";
            private static string _moduleDef = "MODULE {0}({1})\n";
            private static string _varDeclarationBlock = "VAR {0} : {1};\n";
            private static string _varInitializationBlock = "init({0}):= {1};\n";
            private static string _defineBlock = "DEFINE {0}:= {1};\n";
            private static string _osmStateVar = "S_smv";
            private static string _eccStateVar = "Q_smv";
            private static string _ecActionsCounterVar = "NA";
            private static string _algStepsCounterVar = "NI";
            private static string _nextCaseBlock = "next({0}):= case\n{1}\t" + _true + " : {0};\nesac;\n";
            private static string _emptyNextCaseBlock = "next({0}):= case\n{1}esac;\n";
            private static string _existsInputEvent = "ExistsInputEvent";
            private static string _existsEnabledEcTran = "ExistsEnabledECTran";
            private static string _absentsEnabledEcTran = "(!ExistsEnabledECTran)";
            private static string _alpha = "alpha";
            private static string _beta = "beta";
            private static string _omega = "omega";
            private static string _phi = "phi";
            private static string _assign = "\nASSIGN\n";

            private static string _normalVarAssignment = "{0} := {1};\n";
            private static string _nextVarAssignment = "next({0}) := {1};\n";
            private static char   _connectionNameSeparator = '.';

            private static string _and = " & ";
            private static string _or = " | ";
            private static string _not = "!";
            private static string _fairness = "FAIRNESS ({0})\n";
            private static string _running = "running";

            public string True
            {
                get { return _true; }
            }
            public string False
            {
                get { return _false; }
            }
            public string ModuleDef
            {
                get { return _moduleDef; }
            }
            public string VarDeclarationBlock
            {
                get { return _varDeclarationBlock; }
            }
            public string VarInitializationBlock
            {
                get { return _varInitializationBlock; }
            }
            public string DefineBlock
            {
                get { return _defineBlock; }
            }
            public string OsmStateVar
            {
                get { return _osmStateVar; }
            }
            public string EccStateVar
            {
                get { return _eccStateVar; }
            }
            public string EcActionsCounterVar
            {
                get { return _ecActionsCounterVar; }
            }
            public string AlgStepsCounterVar
            {
                get { return _algStepsCounterVar; }
            }
            public string NextCaseBlock
            {
                get { return _nextCaseBlock; }
            }
            public string EmptyNextCaseBlock
            {
                get { return _emptyNextCaseBlock; }
            }
            public string ExistsInputEvent
            {
                get { return _existsInputEvent; }
            }
            public string ExistsEnabledEcTran
            {
                get { return _existsEnabledEcTran; }
            }
            public string AbsentsEnabledEcTran
            {
                get { return _absentsEnabledEcTran; }
            }
            public string Alpha
            {
                get { return _alpha; }
            }
            public string Beta
            {
                get { return _beta; }
            }
            public string Omega
            {
                get { return _omega; }
            }
            public string Phi
            {
                get { return _phi; }
            }
            public string Assign
            {
                get { return _assign; }
            }

            public string Running
            {
                get { return _running; }
            }

            public string NormalVarAssignment
            {
                get { return _normalVarAssignment; }
            }
            public string NextVarAssignment
            {
                get { return _nextVarAssignment; }
            }
            public char ConnectionNameSeparator
            {
                get { return _connectionNameSeparator; }
            }
            public string And
            {
                get { return _and; }
            }
            public string Or
            {
                get { return _or; }
            }
            public string Not
            {
                get { return _not; }
            }

            public string Fairness 
            {
                get { return _fairness; }
            }
        }
    }
}