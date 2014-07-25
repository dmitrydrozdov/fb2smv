using System;
using System.Collections.Generic;
using System.Linq;
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
            Composite
        }

        public enum Direction
        {
            Input,
            Output,
            Internal
        }

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

        public class FBInterface : FBPart
        {
            public Direction Direction;
        }

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

        public class Event : FBInterface
        {
            public Event(string name, string comment, string fbType, Direction direction)
            {
                if (direction == Direction.Internal) throw new Exception("Event cannot be internal.");
                Name = name;
                Comment = comment;
                FBType = fbType;
                Direction = direction;
            }

            public Event()
            {
            }

            public override string ToString()
            {
                return String.Format(Direction == Direction.Input ? "{0}->{1}" : "{0}<-{1}", Name, FBType);
            }
        }

        public class Variable : FBInterface
        {

            public readonly string Type;
            public readonly string InitialValue;
            public readonly int ArraySize;
            public string SmvType;

            public Variable(string name, string comment, string fbType, Direction direction, string type, int arrSize,
                string initialValue, string smvType)
            {
                Name = name;
                Comment = comment;
                FBType = fbType;
                Direction = direction;
                Type = type;
                ArraySize = arrSize;
                InitialValue = initialValue;
                SmvType = smvType;
            }

            public override string ToString()
            {
                if (Direction == Direction.Input) return String.Format("({0}){1}->{2}", Type, Name, FBType);
                else if (Direction == Direction.Output) return String.Format("({0}){1}<-{2}", Type, Name, FBType);
                else return String.Format("({0}){1}--{2}", Type, Name, FBType);
            }
        }

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