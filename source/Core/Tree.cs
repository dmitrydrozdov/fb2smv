using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2SMV
{
    namespace ServiceClasses
    {
        public class TreeNode<T>
        {
            public TreeNode(T value)
            {
                container = value;
                childNodes = new List<TreeNode<T>>();
            }

            public void AppendChild(TreeNode<T> childNode)
            {
                childNodes.Add(childNode);
            }

            public TreeNode<T> FindChild(T value, Func<T, T, bool> comparisionFunc)
            {
                foreach (TreeNode<T> childNode in childNodes)
                {
                    if (comparisionFunc(childNode.container, value)) return childNode;
                }
                return null;
            }
            public T container;
            public List<TreeNode<T>> childNodes;
            public override string ToString()
            {
                return container.ToString();
            }
        }

        public class SimpleTree<T>
        {
            public TreeNode<T> Root;

            public TreeNode<T> FindNode(T value, Func<T, T, bool> comparisionFunc )
            {
                Queue<TreeNode<T>> watchQueue = new Queue<TreeNode<T>>();
                watchQueue.Enqueue(Root);
                while (watchQueue.Any())
                {
                    TreeNode<T> curNode = watchQueue.Dequeue();
                    if (comparisionFunc(curNode.container, value)) return curNode;
                    else
                    {
                        foreach (TreeNode<T> childNode in curNode.childNodes)
                        {
                            watchQueue.Enqueue(childNode);
                        }
                    }
                }
                return null;
            }
        }
    }
}