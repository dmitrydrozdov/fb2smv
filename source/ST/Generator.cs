using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace ST
    {
        public class OutputLine
        {
            public int NI;
            public string Variable;
            public string Condition;
            public string Value;

            public OutputLine(int counter, string variable, string condition, string value)
            {
                NI = counter;
                Variable = variable;
                if (condition == "") Condition = "1";
                else Condition = condition;
                Value = value;
            }

            public override string ToString()
            {
                return String.Format("CODE({0}, {1}, {2}, {3});", Variable, NI, Condition, Value);
            }
        }

        internal class Generator //Variable changing rules table generator
        {
            private bool _conditionEndPointLeftBranchChild(BinaryNode node)
            {
                return node.LeftChild != null
                       && node.LeftChild.Type == NodeType.ConditionEndPoint
                       && node.LeftChild.LeftAncestor == node;
            }

            private bool _conditionEndPointRightBranchChild(BinaryNode node)
            {
                return node.LeftChild != null
                       && node.LeftChild.Type == NodeType.ConditionEndPoint
                       && node.LeftChild.RightAncestor == node;
            }

            private void _addChildrenAvoidEndpoinDoubleAdding(ref Stack<BinaryNode> traverseStack, BinaryNode node)
            {
                if (node.RightChild != null) traverseStack.Push(node.RightChild);
                if (node.LeftChild != null)
                {
                    if (node.LeftChild.Type != NodeType.ConditionEndPoint) traverseStack.Push(node.LeftChild);
                    if (_conditionEndPointRightBranchChild(node)) traverseStack.Push(node.LeftChild);
                }
            }

            private void _countTreeNodes(BinaryGraph tree)
            {
                Stack<BinaryNode> traverseStack = new Stack<BinaryNode>();
                List<BinaryNode> allGraphNodes = new List<BinaryNode>();

                traverseStack.Push(_tree.Root);

                while (traverseStack.Any())
                {
                    BinaryNode currentNode = traverseStack.Pop();
                    allGraphNodes.Add(currentNode);
                    _addChildrenAvoidEndpoinDoubleAdding(ref traverseStack, currentNode);
                }

                //_tree.Root.Counter = 1;
                if (_tree.Root.Type == NodeType.Condition) _tree.Root.Counter = 0;
                else if(_tree.Root.Type == NodeType.Expression) _tree.Root.Counter = 1;
                else throw new Exception("Invalid first operator");

                foreach (BinaryNode node in allGraphNodes)
                {
                    if (node.LeftAncestor != null)
                    {
                        if (node.Type == NodeType.Expression)
                        {
                            node.Counter = node.LeftAncestor.Counter + 1;
                        }
                        else if (node.Type == NodeType.Condition)
                        {
                            node.Counter = node.LeftAncestor.Counter;
                        }
                        else if (node.Type == NodeType.ConditionEndPoint)
                        {
                            node.Counter = Math.Max(node.LeftAncestor.Counter, node.RightAncestor.Counter);
                        }
                        else throw new Exception("Unsupported node type throw instruction enumeration");
                    }
                }
            }

            public Generator(BinaryGraph tree)
            {
                if (tree == null) return;
                _tree = tree;
                _countTreeNodes(_tree);
                //BinaryNode currentNode = _tree.Root;
                Stack<string> conditionsStack = new Stack<string>();
                Stack<BinaryNode> traverseStack = new Stack<BinaryNode>();

                traverseStack.Push(_tree.Root);
                //int counter = 1;

                while (traverseStack.Any())
                {
                    BinaryNode currentNode = traverseStack.Pop();
                    _addChildrenAvoidEndpoinDoubleAdding(ref traverseStack, currentNode);

                    if (currentNode.Type == NodeType.Expression)
                    {
                        //String.Join(" & ", conditionsStack);

                        
                        char[] trimChars = { ' ', '&' }; //TODO: use Smv.OrTrimChars & Smv.AndTrimChars
                        //_outputLines.Add(new OutputLine(currentNode.Counter, currentNode.));

                        string[] varval = currentNode.Value.Split(new string[] { ":=" }, StringSplitOptions.RemoveEmptyEntries);

                        string condition = conditionsStack.Aggregate("", (current, s) => current + ("(" + s + ") & ")).TrimEnd(trimChars);
                        if (conditionsStack.Count > 1)
                        {
                            condition = String.Format("({0})", condition);
                        }
                        _outputLines.Add(new OutputLine(currentNode.Counter, varval[0].Trim(), condition, varval[1].Trim()));

                        if (_conditionEndPointLeftBranchChild(currentNode))
                        {
                            string lastCondition = conditionsStack.Pop();
                            conditionsStack.Push(String.Format("!({0})", lastCondition));
                        }
                    }
                    else if (currentNode.Type == NodeType.Condition)
                    {
                        conditionsStack.Push(currentNode.Value);
                    }
                    else if (currentNode.Type == NodeType.ConditionEndPoint)
                    {
                        conditionsStack.Pop();
                    }
                    else throw new Exception("Unsupported node type throw instruction enumeration");
                }
                //_outputLines.Sort((a, b) => String.CompareOrdinal(a.Variable, b.Variable)); //Sort by variable name
            }

            public List<OutputLine> Lines
            {
                get { return _outputLines; }
                private set { }
            }

            private BinaryGraph _tree;
            private List<OutputLine> _outputLines = new List<OutputLine>();
        }
    }
}