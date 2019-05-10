using FB2SMV.FBCollections;
using System;

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

    public class EventInstance
    {
        public const string EventPreffix = "event_";
        public const string EventSuffix = "";

        public readonly FBInstance Instance;
        public readonly Event Event;

        public EventInstance(Event ev, FBInstance instance)
        {
            Event = ev;
            Instance = instance;
        }

        public bool IsComponentVar()
        {
            return Instance != null;
        }

        public string SmvName()
        {
            if (Instance != null)
            {
                return Instance.Name + "_" + Event.Name;
            }

            return ParameterName();
        }

        public string ParameterName()
        {
            return ParameterName(Event);
        }

        public static string ParameterName(Event ev)
        {
            return EventPreffix + ev.Name + EventSuffix;
        }

        public string Value()
        {
            return SmvName() + (Event.Timed ? ".value" : "");
        }
        
        public string TSLast()
        {
            return SmvName() + ".ts_last";
        }
        
        public string TSBorn()
        {
            return SmvName() + ".ts_born";
        }

        public static string SmvType(string value, string timestamp)
        {
            return String.Format("Event({0}, {1})", value, timestamp);
        }

        public static string SmvType(Event ev)
        {
            return ev.Timed 
                ? SmvType("FALSE", TimeScheduler.TGlobal) 
                : Smv.DataTypes.BoolType;
        }

        protected bool Equals(EventInstance other)
        {
            return Equals(Instance, other.Instance) && Equals(Event, other.Event);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventInstance) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Instance != null ? Instance.GetHashCode() : 0) * 397) ^ (Event != null ? Event.GetHashCode() : 0);
            }
        }
    }
    
    
}