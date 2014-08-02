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
        class CmSmv : ISmv//SMV code patterns  for Carnegie-Melon syntax
        {
            public static string True = "TRUE";
            public static string False = "FALSE";
            public static string ModuleDef = "MODULE {0}({1})\n";
            public static string VarDeclarationBlock = "VAR {0} : {1};\n";
            public static string VarInitializationBlock = "init({0}):= {1};\n";
            public static string DefineBlock = "DEFINE {0}:= {1};\n";
            public static string OsmStateVar = "S_smv";
            public static string EccStateVar = "Q_smv";
            public static string EcActionsCounterVar = "NA";
            public static string AlgStepsCounterVar = "NI";
            public static string NextCaseBlock = "next({0}):= case\n{1}\t" + True + " : {0};\nesac;\n";
            public static string ExistsInputEvent = "ExistsInputEvent";
            public static string ExistsEnabledEcTran = "ExistsEnabledECTran";
            public static string AbsentsEnabledEcTran = "(!ExistsEnabledECTran)";
            public static string Alpha = "alpha";
            public static string Beta = "beta";
            public static string Omega = "omega";
            public static string Assign = "\nASSIGN\n";
            public static string FairnessRunning = "FAIRNESS running\n";

            public static string NormalVarAssignment = "{0} := {1};\n";
            public static string NextVarAssignment = "next({0}) := {1};\n";
            public static char ConnectionNameSeparator = '.';

            //public static string CommonAlphaBetaRules = String.Format(NextCaseBlock, Alpha, "\t" + Beta + " : " + True) + String.Format(NextCaseBlock, Beta, "\t" + Beta + " : " + False);

            public static string And = " & ";
            public static string Or = " | ";
            public static string Not = "!";

            public static string OsmStateChangeBlock
            {
                get
                {
                    string rules = "\t" + CmSmv.Alpha + " & " + OsmStateVar + "=" + Osm.S0 + " & " + ExistsInputEvent +
                                   ": " + Osm.S1 + ";\n";
                    rules += "\t" + OsmStateVar + "=" + Osm.S1 + " & " + ExistsEnabledEcTran + ": " + Osm.S2 + ";\n";
                    rules += "\t" + OsmStateVar + "=" + Osm.S2 + " & " + EcActionsCounterVar + "=0 : " + Osm.S1 + ";\n";
                    rules += "\t" + OsmStateVar + "=" + Osm.S1 + " & " + AbsentsEnabledEcTran + ": " + Osm.S0 + ";\n";
                    return String.Format(NextCaseBlock, OsmStateVar, rules);
                }
            }
            public static string OsmState(string name)
            {
                return name + "_osm";
            }
            public static string EccState(string name)
            {
                return name + "_ecc";
            }
            public static string InitialValue(Variable variable)
            {
                if (variable.InitialValue != null) return variable.InitialValue;
                if (String.Compare(variable.Type, "BOOL", StringComparison.InvariantCultureIgnoreCase) == 0)
                    return "FALSE";
                return "0";
            }
            public static string ClearConditionExpr(string cond)
            {
                Regex rAnd = new Regex(@"((?<=\w)&(?=\w))|((?<=\W)(AND|and|And)(?=\W))");
                Regex rOr = new Regex(@"(?<=\w)\|(?=\w)|((?<=\W)(OR|or|Or)(?=\W))");
                Regex rNot = new Regex(@"(?<=\W)(NOT|not|Not)");
                //cond = cond.Replace("AND", Smv.And);
                string cleared = rAnd.Replace(cond, " " + And + " ");
                cleared = rOr.Replace(cleared, " " + Or + " ");
                cleared = rNot.Replace(cleared, " " + Not);
                return cleared;
            }
            public delegate string ProccessingFunc(string name);
            public static string ConvertConnectionVariableName(string name, ProccessingFunc moduleParamNameConversionFunc, out bool componentVar)
            {
                string[] splitArr = name.Split(CmSmv.ConnectionNameSeparator);
                string converted = "";
                if (splitArr.Count() == 0) throw new Exception("No connection var name");
                else if (splitArr.Count() == 1)
                {
                    converted = moduleParamNameConversionFunc(splitArr[0]);
                    componentVar = false;
                }
                else if (splitArr.Count() == 2)
                {
                    converted = splitArr[0] + "_" + splitArr[1];
                    componentVar = true;
                }
                else throw new Exception("Unknown var name: " + name);
                return converted;
            }
            public static class DataTypes
            {
                public static string BoolType = "boolean";
                public static string NormalRangeType = "0..99";

                public static string GetType(string varType)
                {
                    if (String.Compare(varType, "BOOL", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return BoolType;
                    if (String.Compare(varType, "INT", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return NormalRangeType;
                    if (String.Compare(varType, "UINT", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return NormalRangeType;
                    return null;
                }
            }
            public static class ModuleParameters
            {
                public static string Splitter = ", ";
                public static string EventPreffix = "event_";
                public static string VariablePreffix = "";
                public static string EventSuffix = "";
                public static string VariableSuffix = "_";

                public static string Event(string name)
                {
                    return EventPreffix + name + EventSuffix;
                }

                public static string Variable(string name)
                {
                    return VariablePreffix + name + VariableSuffix;
                }
            }
            public static class Osm
            {
                public static string S0
                {
                    get { return OsmState("s0"); }
                }

                public static string S1
                {
                    get { return OsmState("s1"); }
                }

                public static string S2
                {
                    get { return OsmState("s2"); }
                }
            }

            public static char[] OrTrimChars = { ' ', '|' };
            //public static string
        }

    }
}