using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace ST
    {
        internal enum NodeType
        {
            Expression,
            Condition,
            ConditionAlternative,
            ConditionAlternativeElsif,
            ConditionEndPoint
        }

        internal enum BranchType
        {
            Left,
            Right
        }

        internal class BinaryNode //Binary tree(graph) element
        {
            //private static string[] conditionOperators = {"if"}; //Condition operator
            //private static string[] endpointOperators = {"end_if"}; //Condition end operator
            //private static string[] conditionRightHandOperators = { "else", "elsif" }; //Condition "else" branch operator

            public BinaryNode(NodeType type, string val)
            {
                _type = type;
                _value = val;
            }

            public BinaryNode LeftChild
            {
                set
                {
                    _leftChild = value;
                    _leftChild.Ancestor = this;
                }
                get { return _leftChild; }
            }

            public BinaryNode RightChild
            {
                set
                {
                    _rightChild = value;
                    _rightChild.Ancestor = this;
                }
                get { return _rightChild; }
            }

            public BinaryNode Ancestor
            {
                get { return _leftAncestor; }
                set { _leftAncestor = value; }
            }

            public BinaryNode LeftAncestor
            {
                get { return _leftAncestor; }
                set { _leftAncestor = value; }
            }

            public BinaryNode RightAncestor
            {
                get { return _rightAncestor; }
                set { _rightAncestor = value; }
            }

            public string Value
            {
                get { return _value; }
                //set { _value = value; }
                private set { }
            }

            public int Counter
            {
                get { return _counter; }
                set { if (_counter < value) _counter = value; }

            }

            public NodeType Type
            {
                get { return _type; }
                private set { }
            }

            public override string ToString()
            {
                if (_type == NodeType.Condition) return String.Format("IF ({0})", _value);
                else if (_type == NodeType.ConditionAlternative) return String.Format("ELSE {0}", _value);
                //TODO: condition elsif
                else if (_type == NodeType.ConditionEndPoint) return "END_IF";
                else if (_type == NodeType.Expression) return _value;
                else return base.ToString();
            }

            private int _counter;
            private NodeType _type;
            private BinaryNode _leftAncestor;
            private BinaryNode _rightAncestor;
            private BinaryNode _leftChild;
            private BinaryNode _rightChild;
            private string _value = "";
        }

        internal class BinaryGraph
        {
            public BinaryGraph()
            {
            }

            public BinaryNode Root
            {
                get { return _root; }
                private set { }
            }

            public void SetRoot(BinaryNode node)
            {
                _root = node;
            }

            private BinaryNode _root = null;
        }

        internal class Parcer //Syntax analyzer
        {
            //TODO: make this strings common
            private static string[] conditionOperators = { "if" }; //Condition operator
            private static string[] endpointOperators = { "end_if" }; //Condition end operator
            private static string[] conditionAlternativeOperators = { "else", "elsif" }; //Condition "else" branch operator
            private int _elsifCnt = 0;

            public Parcer(StringSplitter lexicalAnalyzer)
            {
                _lexicalAnalyzer = lexicalAnalyzer;
            }

            public BinaryGraph Parse()
            {
                Stack<BinaryNode> graphConstructionStack = new Stack<BinaryNode>();
                Stack<BinaryNode> leftHeadStack = new Stack<BinaryNode>();

                if (_lexicalAnalyzer.AnyString())
                {
                    BinaryNode node = createNewTokenNode();
                    _tree.SetRoot(node); //TODO: SetRoot
                    graphConstructionStack.Push(node);


                    while (!_lexicalAnalyzer.End())
                    {
                        BinaryNode newNode = createNewTokenNode();
                        BinaryNode stackTop = graphConstructionStack.Peek();
                        if (stackTop.Type != NodeType.Condition && stackTop.Type != NodeType.ConditionAlternativeElsif) graphConstructionStack.Pop();

                        if (newNode.Type != NodeType.ConditionAlternative && newNode.Type != NodeType.ConditionAlternativeElsif && newNode.Type != NodeType.ConditionEndPoint)
                        {
                            if (stackTop.LeftChild == null)
                            {
                                stackTop.LeftChild = newNode;
                                newNode.LeftAncestor = stackTop;
                            }
                            else if (stackTop.RightChild == null)
                            {
                                stackTop.RightChild = newNode;
                                newNode.LeftAncestor = stackTop;
                            }
                            else throw new Exception();
                        }
                        else if (newNode.Type == NodeType.ConditionAlternative)
                        {
                            if (stackTop.Type == NodeType.Condition || stackTop.Type == NodeType.ConditionAlternative || stackTop.Type == NodeType.ConditionAlternativeElsif) throw new Exception("Invalid condition operator syntax");

                            leftHeadStack.Push(stackTop);
                        }
                        else if(newNode.Type == NodeType.ConditionAlternativeElsif)
                        {
                            if (stackTop.Type == NodeType.Condition || stackTop.Type == NodeType.ConditionAlternative || stackTop.Type == NodeType.ConditionAlternativeElsif) throw new Exception("Invalid condition operator syntax");
                            leftHeadStack.Push(stackTop);

                            stackTop = graphConstructionStack.Peek();
                            if (stackTop.Type != NodeType.Condition && stackTop.Type != NodeType.ConditionAlternativeElsif) throw new Exception();

                            if (stackTop.LeftChild != null && stackTop.RightChild == null)
                            {
                                stackTop.RightChild = newNode;
                                newNode.LeftAncestor = stackTop;
                            }
                            else throw new Exception();
                        }
                        else if (newNode.Type == NodeType.ConditionEndPoint)
                        {
                            treatConditionEndPoint(ref graphConstructionStack, ref stackTop, ref leftHeadStack, newNode);
                            if (_elsifCnt > 0)
                            {
                                for (int i = 0; i < _elsifCnt; i++)
                                {
                                    graphConstructionStack.Push(newNode);
                                    stackTop = graphConstructionStack.Peek();
                                    if (stackTop.Type != NodeType.Condition) graphConstructionStack.Pop();

                                    newNode = new BinaryNode(NodeType.ConditionEndPoint, "");
                                    treatConditionEndPoint(ref graphConstructionStack, ref stackTop, ref leftHeadStack, newNode);
                                }
                                _elsifCnt = 0;
                            }
                        }
                        if (newNode.Type != NodeType.ConditionAlternative) graphConstructionStack.Push(newNode);
                    }
                    return _tree;
                }
                else return null;
                
            }

            private void treatConditionEndPoint(ref Stack<BinaryNode> graphConstructionStack, ref BinaryNode stackTop, ref Stack<BinaryNode> leftHeadStack, BinaryNode newNode)
            {
                BinaryNode condNode = graphConstructionStack.Peek();
                if (stackTop.Type == NodeType.Condition) throw new Exception("Missing operator before \"END_IF\"");
                if (condNode.Type != NodeType.Condition && condNode.Type != NodeType.ConditionAlternativeElsif) throw new Exception("Invalid condition operator!");
                if (condNode.RightChild != null)
                {
                    if (!leftHeadStack.Any()) throw new ArgumentNullException();

                    BinaryNode leftHead = leftHeadStack.Pop();
                    stackTop.LeftChild = newNode;
                    leftHead.LeftChild = newNode;
                    newNode.RightAncestor = stackTop;
                    newNode.LeftAncestor = leftHead;
                }
                else
                {
                    condNode.RightChild = newNode;
                    newNode.RightAncestor = condNode;
                    stackTop.LeftChild = newNode;
                    newNode.LeftAncestor = stackTop;
                }
                graphConstructionStack.Pop(); //Delete expression node from stack
            }

            private BinaryNode createNewTokenNode()
            {
                //BinaryNode node = null;
                string token = _lexicalAnalyzer.GetNextToken();
                if (
                    conditionOperators.Any(
                        conditionOperator =>
                            String.Compare(token, conditionOperator, StringComparison.InvariantCultureIgnoreCase) == 0))
                    //If token is a condition operator
                {
                    token = _lexicalAnalyzer.GetNextToken();
                    return new BinaryNode(NodeType.Condition, token);
                }
                if (
                    endpointOperators.Any(
                        endpointOperator =>
                            String.Compare(token, endpointOperator, StringComparison.InvariantCultureIgnoreCase) == 0))
                    //If token is a condition end operator
                {
                    return new BinaryNode(NodeType.ConditionEndPoint, "");
                }
                if (
                    conditionAlternativeOperators.Any(
                        conditionAlternativeOperator =>
                            String.Compare(token, conditionAlternativeOperator,
                                StringComparison.InvariantCultureIgnoreCase) ==
                            0)) //If token is a condition "else" operator
                {
                    //token = _lexicalAnalyzer.GetNextToken();
                    if (String.Compare(token, "elsif", StringComparison.InvariantCultureIgnoreCase) == 0)  //Elsif operator. //TODO: 
                    {
                        _elsifCnt++;
                        token = _lexicalAnalyzer.GetNextToken();
                        return new BinaryNode(NodeType.ConditionAlternativeElsif, token);
                    }

                    
                    return new BinaryNode(NodeType.ConditionAlternative, ""/*token*/);
                }
                else //If token is not a condition operator - create standard node
                {
                    if (String.Compare(token, "then", StringComparison.InvariantCultureIgnoreCase) == 0)
                        return createNewTokenNode();
                        //token = _lexicalAnalyzer.GetNextToken();
                    else
                        return new BinaryNode(NodeType.Expression, token);
                }

            }

            private StringSplitter _lexicalAnalyzer;
            private BinaryGraph _tree = new BinaryGraph();
        }
    }
}