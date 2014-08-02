using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Core;
using FB2SMV.Core;
using FB2SMV.FBCollections;
using FB2SMV.Core.Structures;
using GUI.Properties;

namespace GUI
{
    public partial class Form1 : Form
    {
        FBClassParcer _parcer;
        private string _selectedFbType;
        private Variable _selectedVariable = null;
        private IEnumerable<Variable> _connectedVars;
        private VarDependencyGraph varDependencyGraph;

        public Form1()
        {
            InitializeComponent();
        }

        EventHandler xmlParsingFinishedHandler = XmlParsingFinishedHandler;

        private static void XmlParsingFinishedHandler(object sender, EventArgs eventArgs)
        {
            
        }

        private void clear()
        {
            _parcer = new FBClassParcer();
            _selectedFbType = null;
            _selectedVariable = null;
            fbTypesView.Nodes.Clear();
            smvCodeRichTextBox.Text = "";
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                clear();
                try
                {
                    _parcer.ParseRecursive(openFileDialog1.FileName);
                }
                catch (Exception exception)
                {
                    Program.ErrorMessage(exception.Message);
                    return;
                }
                //fillTreeView();
                VisualizableStringTree t = new VisualizableStringTree();
                t.Construct(_parcer.Storage);
                varDependencyGraph = new VarDependencyGraph(_parcer.Storage);
                varDependencyGraph.Construct();

                fbTypesView.Nodes.Add(t.TreeViewRoot());
            }
        }

        private void fillTreeView()
        {
            var rootFbType = _parcer.Storage.Types.FirstOrDefault(t => t.IsRoot);
            if (rootFbType != null)
            {
                fbTypesView.Nodes.Add(rootFbType.Name, rootFbType.Name);
                foreach (FBInstance fbInstance in _parcer.Storage.Instances)
                {
                    TreeNode[] curNodes = fbTypesView.Nodes.Find(fbInstance.FBType, true);
                    //TreeNode curNode = fbTypesView.Nodes[fbInstance.FBType];
                    if (curNodes.Count() != 0)
                    {
                        curNodes[0].Nodes.Add(fbInstance.InstanceType, fbInstance.InstanceType);
                    }
                    else
                    {
                        fbTypesView.Nodes.Add(fbInstance.FBType, fbInstance.FBType);
                        curNodes = fbTypesView.Nodes.Find(fbInstance.FBType, true);
                        if (curNodes.Count() != 0)
                        {
                            curNodes[0].Nodes.Add(fbInstance.InstanceType, fbInstance.InstanceType);
                        }
                    }
                }
            }
        }

        private void fillVarTreeView(IEnumerable<Variable> vars)
        {
            TreeNode inputVars = new TreeNode("InputVars");
            TreeNode outputVars = new TreeNode("OutputVars");
            TreeNode internalVars = new TreeNode("InternalVars");
            foreach (Variable v in vars)
            {
                string varText = v.ArraySize == 0 ? String.Format("({0}) {1}", v.Type, v.Name) : String.Format("({0}[{2}]) {1}", v.Type, v.Name, v.ArraySize);
                if (v.Direction == Direction.Input) inputVars.Nodes.Add(v.Name, varText);
                else if (v.Direction == Direction.Output) outputVars.Nodes.Add(v.Name, varText);
                else if (v.Direction == Direction.Internal) internalVars.Nodes.Add(v.Name, varText);
            }
            if (inputVars.Nodes.Count > 0)
            {
                variablesTreeView.Nodes.Add(inputVars);
                inputVars.Expand();
            }
            if (outputVars.Nodes.Count > 0)
            {
                variablesTreeView.Nodes.Add(outputVars);
                outputVars.Expand();
            }
            if (internalVars.Nodes.Count > 0)
            {
                variablesTreeView.Nodes.Add(internalVars);
                internalVars.Expand();
            }
        }

        private void fbTypesView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            variablesTreeView.Nodes.Clear();
            _selectedFbType = fbTypesView.SelectedNode.Text;
            groupBox1.Text = _selectedFbType;
            IEnumerable<Variable> vars = _parcer.Storage.Variables.Where(v => v.FBType == _selectedFbType);
            fillVarTreeView(vars);
        }

        private void variablesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            connectedVarsTreeView.Nodes.Clear();
            _selectedVariable = _parcer.Storage.Variables.FirstOrDefault(v => v.FBType == _selectedFbType && v.Name == variablesTreeView.SelectedNode.Name);
            if (_selectedVariable != null)
            {
                varNameTextBox.Text = _selectedVariable.Name;
                varTypeTextBox.Text = _selectedVariable.Type;
                varArraySizeTextBox.Text = Convert.ToString(_selectedVariable.ArraySize);
                varRangeTextBox.Text = _selectedVariable.SmvType;

                _connectedVars = varDependencyGraph.GetConnectedVariables(VarDependencyGraph.VariableKey(_selectedVariable));

                foreach (Variable connectedVar in _connectedVars)
                {
                    connectedVarsTreeView.Nodes.Add(connectedVar.ToString(), connectedVar.ToString());
                }
            }
        }

        private void propChangeButton_Click(object sender, EventArgs e)
        {
            _selectedVariable.SmvType = varRangeTextBox.Text;
            foreach (Variable variable in _connectedVars)
            {
                variable.SmvType = varRangeTextBox.Text;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName name = assembly.GetName();
            string description = "FB-to-SMV converter";

            var copyrightAttribute = assembly   .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
                                                .OfType<AssemblyCopyrightAttribute>()
                                                .FirstOrDefault();

            string aboutMessage = String.Format("{0}\nVersion: {1}.{2}.{3}.{4}\n{5}",
                description,
                name.Version.Major, 
                name.Version.Minor, 
                name.Version.MajorRevision,
                name.Version.MinorRevision,
                copyrightAttribute.Copyright
                );
            MessageBox.Show(aboutMessage);
        }

        private void saveSMVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SmvCodeGenerator translator = new SmvCodeGenerator(_parcer.Storage);
            foreach (string fbSmv in translator.TranslateAll())
            {
                smvCodeRichTextBox.Text += fbSmv;
            }
            //smvCodePage.Focus();
            tabControl1.SelectTab(smvCodePage);
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*if (saveFileDialogProject.ShowDialog() == DialogResult.OK)
            {
                
            }*/
        }

        private void saveSMVCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FBType rootFbType = _parcer.Storage.Types.FirstOrDefault(t => t.IsRoot);
            if (rootFbType == null)
            {
                Program.ErrorMessage(Messages.No_FB_System_Loaded_Message_);
                return;
            }
            if (smvCodeRichTextBox.Text == "")
            {
                Program.ErrorMessage(Messages.SMV_Code_Empty_);
                return;
            }

            saveFileDialogSMV.FileName = rootFbType.Name + ".smv";
            if (saveFileDialogSMV.ShowDialog() == DialogResult.OK)
            {
                StreamWriter wr = new StreamWriter(saveFileDialogSMV.FileName);
                wr.Write(smvCodeRichTextBox.Text);
                wr.Close();
            }
        }
    }
}
