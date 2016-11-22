using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Services;
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
        public class Smv
        {
            private ISmvPattern _smvPattern = new CmSmvPattern();

            /// <summary>
            /// Set SMV code generation pattern (default pattern is CmSmvPattern)
            /// </summary>
            /// <param name="smvPattern"></param>
            public Smv()
            {
                _smvPattern = new CmSmvPattern();
            }
            public Smv(ISmvPattern pattern)
            {
                if (pattern == null) throw new ArgumentException();
                _smvPattern = pattern;
            }

            public string OsmStateChangeBlock 
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

            public string True
            {
                get { return _smvPattern.True; }
            }

            public string False
            {
                get { return _smvPattern.False; }
            }

            public string ModuleDef
            {
                get { return _smvPattern.ModuleDef; }
            }

            public string VarDeclarationBlock
            {
                get { return _smvPattern.VarDeclarationBlock; }
            }

            public string VarInitializationBlock
            {
                get { return _smvPattern.VarInitializationBlock; }
            }

            public string DefineBlock
            {
                get { return _smvPattern.DefineBlock; }
            }

            public string OsmStateVar
            {
                get { return _smvPattern.OsmStateVar; }
            }

            public string EccStateVar
            {
                get { return _smvPattern.EccStateVar; }
            }

            public string EcActionsCounterVar
            {
                get { return _smvPattern.EcActionsCounterVar; }
            }

            public string AlgStepsCounterVar
            {
                get { return _smvPattern.AlgStepsCounterVar; }
            }

            public string NextCaseBlock
            {
                get { return _smvPattern.NextCaseBlock; }
            }

            public string EmptyNextCaseBlock
            {
                get { return _smvPattern.EmptyNextCaseBlock; }
            }

            public string ExistsInputEvent
            {
                get { return _smvPattern.ExistsInputEvent; }
            }

            public string ExistsEnabledEcTran
            {
                get { return _smvPattern.ExistsEnabledEcTran; }
            }

            public string AbsentsEnabledEcTran
            {
                get { return _smvPattern.AbsentsEnabledEcTran; }
            }

            public string Alpha
            {
                get { return _smvPattern.Alpha; }
            }

            public string Beta
            {
                get { return _smvPattern.Beta; }
            }

            public string Omega
            {
                get { return _smvPattern.Omega; }
            }

            public string Assign
            {
                get { return _smvPattern.Assign; }
            }

            public string Trans
            {
                get { return _smvPattern.Trans; }
            }

            public string InitStatement
            {
                get { return _smvPattern.InitStatement; }
            }

            public string Eq
            {
                get { return _smvPattern.Eq; }
            }

            /*public static string FairnessRunning
            {
                get { return _smvPattern.FairnessRunning; }
            }*/

            public string Fairness(string fairCondition)
            {
                return String.Format(_smvPattern.Fairness, fairCondition);
            }

            public string NormalVarAssignment
            {
                get { return _smvPattern.NormalVarAssignment; }
            }

            public string NextVarAssignment
            {
                get { return _smvPattern.NextVarAssignment; }
            }

            public char ConnectionNameSeparator
            {
                get { return _smvPattern.ConnectionNameSeparator; }
            }

            public string And
            {
                get { return _smvPattern.And; }
            }

            public string Or
            {
                get { return _smvPattern.Or; }
            }

            public string Not
            {
                get { return _smvPattern.Not; }
            }

            public string NotFunc(string expr)
            {
                return "(" + _smvPattern.Not + expr + ")";
            }

            public string Running
            {
                get { return _smvPattern.Running; }
            }

            public static string OsmState(string name)
            {
                return name + "_osm";
            }

            public static string EccState(string name)
            {
                return name + "_ecc";
            }

            public static string ArrayIndex(int index)
            {
                return "[" + index + "]";
            }

            public string ClearInitialValue(string initialValue, Variable variable) //TODO: fix in SmvCodeGenerator.Check()
            {
                string val = initialValue;
                
                if (variable.SmvType.GetType() == typeof(DataTypes.RangeSmvType))
                {
                    if (val == "0.0") val = "0";
                    if (Convert.ToInt32(val) < ((DataTypes.RangeSmvType)variable.SmvType).RangeBegin)
                        val = ((DataTypes.RangeSmvType)variable.SmvType).RangeBegin.ToString();
                    else if (Convert.ToInt32(val) > ((DataTypes.RangeSmvType)variable.SmvType).RangeEnd)
                        val = ((DataTypes.RangeSmvType)variable.SmvType).RangeEnd.ToString();
                }
                else if (variable.SmvType.GetType() == typeof(DataTypes.BoolSmvType))
                {
                    if (val == "0") return _smvPattern.False;
                    if (val == "1") return _smvPattern.True;
                }
                return val;
            }

            public string InitialValue(Variable variable)
            {
                if (variable.SmvType is DataTypes.RangeSmvType)
                {
                    if (variable.InitialValue != null)
                    {
                        string val;
                        val = variable.InitialValue == "0.0" ? "0" : variable.InitialValue;
                        if (Convert.ToInt32(val) < ((DataTypes.RangeSmvType) variable.SmvType).RangeBegin)
                            val = ((DataTypes.RangeSmvType) variable.SmvType).RangeBegin.ToString();
                        else if (Convert.ToInt32(val) > ((DataTypes.RangeSmvType)variable.SmvType).RangeEnd)
                            val = ((DataTypes.RangeSmvType)variable.SmvType).RangeEnd.ToString();

                        return val;
                    }
                    else return ((DataTypes.RangeSmvType)variable.SmvType).RangeBegin.ToString();
                }
                else if (variable.SmvType is DataTypes.BoolSmvType)
                {
                    if (variable.InitialValue != null && variable.InitialValue == "1") return _smvPattern.True;
                    else if (variable.InitialValue != null && variable.InitialValue == "0") return _smvPattern.False;
                    else if (variable.InitialValue != null) return variable.InitialValue;
                    else return _smvPattern.False;
                }

                if(variable.InitialValue != null) return variable.InitialValue;
                else return "0";
            }

            public string ClearConditionExpr(string cond)
            {
                Regex rAnd = new Regex(@"((?<=\w)&(?=\w))|((?<=\W)(AND|and|And)(?=\W))");
                Regex rOr = new Regex(@"(?<=\w)\|(?=\w)|((?<=\W)(OR|or|Or)(?=\W))");
                Regex rNot = new Regex(@"(?<=(\W|^))(NOT|not|Not)(?=\W)");//new Regex(@"(?<=\W)(NOT|not|Not)");

                Regex rFalse = new Regex(@"((?<=\W)(false|False)(?=\W))"); //TODO: switch off case sensitive
                Regex rTrue = new Regex(@"((?<=\W)(true|True)(?=\W))"); //TODO: switch off case sensitive
                //cond = cond.Replace("AND", Smv.And);
                string cleared = rAnd.Replace(cond, " " + _smvPattern.And + " ");
                cleared = rOr.Replace(cleared, " " + _smvPattern.Or + " ");
                cleared = rNot.Replace(cleared, " " + _smvPattern.Not);
                cleared = rFalse.Replace(cleared, " " + _smvPattern.False);
                cleared = rTrue.Replace(cleared, " " + _smvPattern.True);
                cleared = cleared.Replace("<>", "!=");
                return cleared;
            }

            public delegate string ProccessingFunc(string name);

            public string ConvertConnectionVariableName(string name, ProccessingFunc moduleParamNameConversionFunc, out bool componentVar)
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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns>[0] - instance name or variable name; [1] - variable name</returns>
            public string[] SplitConnectionVariableName(string name) //TODO: refactoring: ConnectionNode.Instance + ConnectionNode.Name
            {
                string[] splitArr = name.Split(_smvPattern.ConnectionNameSeparator);
                if (splitArr.Count() == 0) throw new Exception("No connection var name");
                return splitArr;
            }

            public class DataTypes
            {
                public static bool IsSimple(ISmvType type)
                {
                    if (type is BoolSmvType || type is IntSmvType || type is RealSmvType) return true;
                    else return false;
                }
                [Serializable]
                public class BoolSmvType : ISmvType
                {
                    public override string ToString()
                    {
                        return "boolean";
                    }
                }

                [Serializable]
                public class IntSmvType : ISmvType
                {
                    public override string ToString()
                    {
                        return "integer";
                    }
                }

                [Serializable]
                public class RealSmvType : ISmvType
                {
                    public override string ToString()
                    {
                        return "real";
                    }
                }

                [Serializable]
                public class RangeSmvType : ISmvType
                {
                    public RangeSmvType(int begin, int end)
                    {
                        RangeBegin = begin;
                        RangeEnd = end;
                    }

                    public RangeSmvType(RangeSmvType copyInstance)
                    {
                        RangeBegin = copyInstance.RangeBegin;
                        RangeEnd = copyInstance.RangeEnd;
                    }
                    public int RangeBegin { get; private set; }
                    public int RangeEnd { get; private set; }
                    public override string ToString()
                    {
                        return RangeBegin + ".." + RangeEnd;
                    }
                }

                public static string BoolType = "boolean";
                //public static string NormalRangeType = "0..99";

                public static ISmvType GetType(string varType, ShowMessageDelegate showMessage, bool smvInfiniteTypes)
                {
                    if (IEC61499.DataTypeMatch(varType, IEC61499.DataTypes.BOOL))
                        return new BoolSmvType();
                    if (IEC61499.DataTypeMatch(varType, IEC61499.DataTypes.INT))
                        if (smvInfiniteTypes) return new IntSmvType();
                        else return new RangeSmvType(0,99);
                    if (IEC61499.DataTypeMatch(varType, IEC61499.DataTypes.UINT))
                        if (smvInfiniteTypes) return new IntSmvType();
                        else return new RangeSmvType(0, 99);
                    if (IEC61499.DataTypeMatch(varType, IEC61499.DataTypes.DINT))
                        if (smvInfiniteTypes) return new IntSmvType();
                        else return new RangeSmvType(0, 99);
                    if (IEC61499.DataTypeMatch(varType, IEC61499.DataTypes.TIME)) //Костыль
                    {
                        showMessage(String.Format("Warning! Unsupported data type \"{0}\" will be changed to range [0..500]", varType));
                        return new RangeSmvType(0, 500);
                    }
                    if (IEC61499.DataTypeMatch(varType, IEC61499.DataTypes.STRING)) //Костыль
                    {
                        showMessage(String.Format("Warning! Unsupported data type \"{0}\" will be changed to bool", varType));
                        return new BoolSmvType();
                    }
                    if (IEC61499.DataTypeMatch(varType, IEC61499.DataTypes.REAL)) //Костыль
                    {
                        if (smvInfiniteTypes) return new RealSmvType();
                        else {
                            showMessage(String.Format("Warning! Data type \"{0}\" is not supported for current settings and will be changed to range [0..99]", varType));
                            return new RangeSmvType(0, 99);
                        }
                    }
                    throw new Exception(String.Format("Unsupported data type \"{0}\"!", varType));
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