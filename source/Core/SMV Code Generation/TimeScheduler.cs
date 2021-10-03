using FB2SMV.FBCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV.Core
{ 
    public class TimeScheduler
    {
        private int _inputsCount;
        private IEnumerable<FBInstance> _inputs;
        private string _timeVariableType;
        private int _maxLocalTime;

        public TimeScheduler(IEnumerable<FBInstance> inputs, string timeVariableType, int maxLocalTime)
        {
            _inputsCount = inputs.Count();
            _inputs = inputs;
            _timeVariableType = timeVariableType;
            _maxLocalTime = maxLocalTime;
        }

        private string _forAll(Func<int, string> aggregateFunc)
        {
            string aggregateString = "";
            for (int i = 1; i <= _inputsCount; i++)
            {
                aggregateString += aggregateFunc(i);
            }
            return aggregateString;
        }

        private string _vDefBlock(int i)
        {
            return String.Format("V{0}:=case\n\tD{0}o >= 0 : D{0}o;\n\tTRUE: {1};\nesac;\n", i, _maxLocalTime);
        }

        private string _DiRule(int i)
        {
            return String.Format("D{0}i:= case\n\t({1} < Tmax) & beta & gamma & D{0}o > 0 : D{0}o - DMin;\n\tTRUE: D{0}o;\nesac;\n", i, TGlobal);
        }

        private string _GetDMin()
        {
            if (_inputsCount > 1)
            {
                string dMinRules = "";
                for(int i = 1; i <= _inputsCount; i++)
                {
                    string currentV = "V" + i;
                    string ruleLine = "";
                    for(int j = 1; j <= _inputsCount; j++)
                    {
                        if (j == i) continue;
                        string comparedV = "V" + j;
                        ruleLine += String.Format("({0}<={1}){2}", currentV, comparedV, Smv.And);

                    }
                    dMinRules += String.Format("\t{0} : {1};\n", ruleLine.Trim(Smv.AndTrimChars), currentV);
                }
                return String.Format("DMin:=case\n{0}\n\tTRUE: {1};esac;\n", dMinRules, 0);
            }

            return "DMin:= V1;";

        }

        private string _TGlobal()
        {
            string rules = "";
            string ruleTemplate = _forAll((int i) => "D" + i.ToString() + "o {0} 0{1}");
            const string ruleFormat = "\t(TGlobal < Tmax) & beta & gamma & ({0}) : {1};\n";
            ruleTemplate = ruleTemplate.Substring(0, ruleTemplate.Length - 3); //remove last {1}
            rules += String.Format(ruleFormat, String.Format(ruleTemplate, ">", Smv.Or), "TGlobal + DGmin");
            //rules += String.Format(ruleFormat, String.Format(ruleTemplate, "<", Smv.And), "TGlobal + 1"); //global time increment for test case
            return String.Format(Smv.NextCaseBlock, "TGlobal", rules);
        }

        public string GetSMVCode(int Tmax)
        {
            string smvCode = "";
            string moduleIOs = "";

            moduleIOs += _forAll( (int i) => String.Format("D{0}i, D{0}o, ", i) ); //generate module parameters: D1i, D1o, D2i, ...
            moduleIOs += String.Format("{0}, {1}", Smv.Beta, "gamma");

            smvCode += String.Format(Smv.ModuleDef, "TimeScheduler", moduleIOs);
            smvCode += _forAll((int i) => String.Format(Smv.VarDeclarationBlock, "V" + i.ToString(), _timeVariableType) );
            smvCode += String.Format(Smv.VarDeclarationBlock, "DMin", _timeVariableType);
            smvCode += String.Format(Smv.VarDeclarationBlock, "TGlobal", _timeVariableType);

            smvCode += Smv.Assign + Environment.NewLine;
            smvCode += String.Format(Smv.VarInitializationBlock, TGlobal, 0);
            smvCode += _forAll((int i) => _vDefBlock(i) );
            smvCode += _GetDMin();
            smvCode += _forAll((int i)=> _DiRule(i));
            smvCode += _TGlobal();
            smvCode += String.Format(Smv.DefineBlock, "Tmax", Tmax);

            return smvCode;
        }

        public static string TGlobal => "TGlobal";

    }
}