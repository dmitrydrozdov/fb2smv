using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FB2SMV
{
    namespace FBCollections
    {
        public class Common
        {
        }

        public enum FBClass
        {
            Basic,
            Composite,
            Library
        }

        public enum Direction
        {
            Input,
            Output,
            Internal
        }

        [Serializable]
        public class FBPart
        {
            public FBPart(string name, string comment, string fbType)
            {
                Name = name;
                Comment = comment;
                FBType = fbType;
            }

            public FBPart()
            {
            }

            public string Name;
            public string Comment;
            public string FBType;
        }
        [Serializable]
        public class FBInterface : FBPart
        {
            public Direction Direction;
        }

        [Serializable]
        public class FBType
        {
            public FBType(string name, string comment, FBClass type, bool isRoot = false)
            {
                Name = name;
                Comment = comment;
                Type = type;
                IsRoot = isRoot;
            }

            public FBType()
            {
            }

            public readonly string Name;
            public readonly string Comment;
            public readonly FBClass Type;
            public readonly bool IsRoot;

            public override string ToString()
            {
                return Name + (Type == FBClass.Basic ? "[basic]" : "[composite]");
            }
        }

        [Serializable]
        public class Event : FBInterface
        {
            public bool Timed;
            public Event(string name, string comment, string fbType, Direction direction, bool timed = false)
            {
                if (direction == Direction.Internal) throw new Exception("Event cannot be internal.");
                Name = name;
                Comment = comment;
                FBType = fbType;
                Direction = direction;
                Timed = timed;
            }

            public Event()
            {
            }

            public override string ToString()
            {
                return String.Format(Direction == Direction.Input ? "{0}->{1}" : "{0}<-{1}", Name, FBType);
            }
        }

        public interface ISmvType
        {}

        [Serializable]
        public class Variable : FBInterface
        {

            public readonly string Type;
            public readonly string InitialValue;
            public readonly int ArraySize;
            public ISmvType SmvType;
            public bool IsConstant;

            public Variable(string name, string comment, string fbType, Direction direction, string type, int arrSize,
                string initialValue, ISmvType smvType, bool constant = false)
            {
                Name = name;
                Comment = comment;
                FBType = fbType;
                Direction = direction;
                Type = type;
                ArraySize = arrSize;
                InitialValue = initialValue;
                SmvType = smvType;
                IsConstant = constant;
            }

            public override string ToString()
            {
                if (Direction == Direction.Input) return String.Format("({0}){1}->{2}", Type, Name, FBType);
                else if (Direction == Direction.Output) return String.Format("({0}){1}<-{2}", Type, Name, FBType);
                else return String.Format("({0}){1}--{2}", Type, Name, FBType);
            }
        }

        [Serializable]
        public class WithConnection
        {
            public WithConnection(string fbType, string ev, string variable)
            {
                FBType = fbType;
                Event = ev;
                Var = variable;
            }

            public readonly string FBType;
            public readonly string Event;
            public readonly string Var;
        }


    }
}