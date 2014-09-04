using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FB2SMV.FBCollections;
using FB2SMV.ServiceClasses;
using FB2SMV.Core.Structures;
using GUI.Properties;

namespace GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void ErrorMessage(string message)
        {
            MessageBox.Show(message, Messages.ErrorMessageBox_Caption_);
        }

        public static FB2SMV.Core.Settings Settings = new FB2SMV.Core.Settings();
        /*public static TreeNode ConstructFBTypesTree(Storage storage)
        {
            
        }*/
    }

    class ConnectionNode
    {
        public readonly string FbType;
        public readonly string InstanceName;
        public readonly string Variable;

        public ConnectionNode(string fbType, string instName, string variable)
        {
            FbType = fbType;
            InstanceName = instName;
            Variable = variable;
        }

        public ConnectionNode(string fbType, string connectionNodeName)
        {
            string[] nameSplit = connectionNodeName.Split('.');
            FbType = fbType;
            InstanceName = nameSplit[0];
            Variable = nameSplit[1];
        }
    }

    class VisualizableStringTree : TypesTree
    {
        public TreeNode TreeViewRoot()
        {
            TreeNode rootNode = new TreeNode(Root.container);
            AppendChildren(Root, rootNode);
            return rootNode;
        }

        private void AppendChildren(TreeNode<string> myNode, TreeNode treeViewNode)
        {
            if (!myNode.childNodes.Any()) return;
            else
            {
                foreach (TreeNode<string> childNode in myNode.childNodes)
                {
                    TreeNode treeViewChildNode = new TreeNode(childNode.container);
                    AppendChildren(childNode, treeViewChildNode);
                    treeViewNode.Nodes.Add(treeViewChildNode);
                }
            }
        }
    }
}
