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
        /// <summary>
        /// SMV code generation helper. Different patterns can be used. Default is CmSmvPattern
        /// </summary>
        internal class Smv
        {
            private static ISmvPattern _smvPattern = new CmSmvPattern();

            /// <summary>
            /// Set SMV code generation pattern (default pattern is CmSmvPattern)
            /// </summary>
            /// <param name="smvPattern"></param>
            public static void SetPattern(ISmvPattern smvPattern)
            {
                if (smvPattern == null) throw new ArgumentException();
                _smvPattern = smvPattern;
            }

            public static string OsmStateChangeBlock 
            {
                get
                {
                    //TODO: move code into pattern
                    string rules = "\t" + _smvPattern.Alpha + " & " + _smvPattern.OsmStateVar + "=" + Osm.S0 + " & " + _smvPattern.ExistsInputEvent + ": " + Osm.S1 + ";\n";
                    rules += "\t" + _smvPattern.OsmStateVar + "=" + Osm.S1 + " & " + _smvPattern.ExistsEnabledEcTran + ": " + Osm.S2 + ";\n";
                    rules += "\t" + _smvPattern.OsmStateVar + "=" + Osm.S2 + " & " + _smvPattern.EcActionsCounterVar + "=0 : " + Osm.S1 + ";\n";
                    rules += "\t" + _smvPattern.OsmStateVar + "=" + Osm.S1 + " & " + _smvPattern.AbsentsEnabledEcTran + ": " + Osm.S0 + ";\n";
                    return String.Format(_smvPattern.NextCaseBlock, _smvPattern.OsmStateVar, rules);
                }
            }

            public static string True
            {
                get { return _smvPattern.True; }
            }

            public static string False
            {
                get { return _smvPattern.False; }
            }

            public static string ModuleDef
            {
                get { return _smvPattern.ModuleDef; }
            }

            public static string VarDeclarationBlock
            {
                get { return _smvPattern.VarDeclarationBlock; }
            }

            public static string VarInitializationBlock
            {
                get { return _smvPattern.VarInitializationBlock; }
            }

            public static string DefineBlock
            {
                get { return _smvPattern.DefineBlock; }
            }

            public static string OsmStateVar
            {
                get { return _smvPattern.OsmStateVar; }
            }

            public static string EccStateVar
            {
                get { return _smvPattern.EccStateVar; }
            }

            public static string EcActionsCounterVar
            {
                get { return _smvPattern.EcActionsCounterVar; }
            }

            public static string AlgStepsCounterVar
            {
                get { return _smvPattern.AlgStepsCounterVar; }
            }

            public static string NextCaseBlock
            {
                get { return _smvPattern.NextCaseBlock; }
            }

            public static string ExistsInputEvent
            {
                get { return _smvPattern.ExistsInputEvent; }
            }

            public static string ExistsEnabledEcTran
            {
                get { return _smvPattern.ExistsEnabledEcTran; }
            }

            public static string AbsentsEnabledEcTran
            {
                get { return _smvPattern.AbsentsEnabledEcTran; }
            }

            public static string Alpha
            {
                get { return _smvPattern.Alpha; }
            }

            public static string Beta
            {
                get { return _smvPattern.Beta; }
            }

            public static string Omega
            {
                get { return _smvPattern.Omega; }
            }

            public static string Assign
            {
                get { return _smvPattern.Assign; }
            }

            public static string FairnessRunning
            {
                get { return _smvPattern.FairnessRunning; }
            }

            public static string NormalVarAssignment
            {
                get { return _smvPattern.NormalVarAssignment; }
            }

            public static string NextVarAssignment
            {
                get { return _smvPattern.NextVarAssignment; }
            }

            public static char ConnectionNameSeparator
            {
                get { return _smvPattern.ConnectionNameSeparator; }
            }

            public static string And
            {
                get { return _smvPattern.And; }
            }

            public static string Or
            {
                get { return _smvPattern.Or; }
            }

            public static string Not
            {
                get { return _smvPattern.Not; }
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
                string cleared = rAnd.Replace(cond, " " + _smvPattern.And + " ");
                cleared = rOr.Replace(cleared, " " + _smvPattern.Or + " ");
                cleared = rNot.Replace(cleared, " " + _smvPattern.Not);
                return cleared;
            }

            public delegate string ProccessingFunc(string name);

            public static string ConvertConnectionVariableName(string name, ProccessingFunc moduleParamNameConversionFunc, out bool componentVar)
            {
                string[] splitArr = name.Split(_smvPattern.ConnectionNameSeparator);
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

            public static char[] OrTrimChars = {' ', '|'};
        }
    }
}