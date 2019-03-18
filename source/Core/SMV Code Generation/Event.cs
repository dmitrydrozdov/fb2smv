using FB2SMV.FBCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV.Core
{ 
    public class EventModule
    {
        private string _timeVariableType;

        public EventModule(string timeVariableType)
        {
            _timeVariableType = timeVariableType;
        }

        public string GetSMVCode()
        {
            string smvCode = "";
            string moduleIOs = "";

            string valueInput = "valueI";
            string timeInput = "timestamp";
            moduleIOs += $"{valueInput}, {timeInput}";
            smvCode += String.Format(Smv.ModuleDef, "Event", moduleIOs);
            smvCode += String.Format(Smv.VarDeclarationBlock, "value", Smv.DataTypes.BoolType);
            smvCode += String.Format(Smv.VarDeclarationBlock, "ts_last", _timeVariableType);
            smvCode += String.Format(Smv.VarDeclarationBlock, "ts_born", _timeVariableType);
            smvCode += Smv.Assign;
            smvCode += String.Format(Smv.VarInitializationBlock, "value", valueInput);
            smvCode += String.Format(Smv.VarInitializationBlock, "ts_last", timeInput);
            smvCode += String.Format(Smv.VarInitializationBlock, "ts_born", timeInput);
            smvCode += Environment.NewLine;
            return smvCode;
        }
        
    }

    public static class EventInstance
    {
        public static string EventPreffix = "event_";
        public static string EventSuffix = "";

        public static string Name(string name)
        {
            return EventPreffix + name + EventSuffix;
        }

        public static string Value(string name)
        {
            return Name(name) + ".value";
        }
        
        public static string TSLast(string name)
        {
            return Name(name) + ".ts_last";
        }
        
        public static string TSBorn(string name)
        {
            return Name(name) + ".ts_born";
        }

        public static string SmvType(string value, string timestamp)
        {
            return String.Format("Event({0}, {1})", value, timestamp);
        }
    }
    
    
}