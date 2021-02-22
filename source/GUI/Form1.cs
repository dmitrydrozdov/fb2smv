﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
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
        private static string projectFileExtension = ".f2s"; //TODO: move to resource
        FBClassParcer _parcer;
        private string _selectedFbType;
        private Variable _selectedVariable = null;
        private IEnumerable<Variable> _connectedVars;
        private VarDependencyGraph varDependencyGraph;
        //private List<IDispatcher> _dispatchers = null;
        private List<ExecutionModel> _executionModels = null;
        private IDispatcher _currentDisp = null;
        private static Form1 _inst = null;

        public Form1()
        {
            InitializeComponent();
            _inst = this;
        }

        //EventHandler xmlParsingFinishedHandler = XmlParsingFinishedHandler;

        /*private static void XmlParsingFinishedHandler(object sender, EventArgs eventArgs)
        {
            
        }*/

        private void resetWorkspace()
        {
            _parcer = new FBClassParcer(ShowMessage, Program.Settings);
            _selectedFbType = null;
            _selectedVariable = null;
            fbTypesView.Nodes.Clear();
            smvCodeRichTextBox.Text = "";
            messagesRichTextBox.Text = "";
        }

        private void resetWorkspace(ProjectFileStructure project)
        {
            _executionModels = project.ExecutionModels;
            _parcer = new FBClassParcer(project.Storage, ShowMessage);
            _selectedFbType = null;
            _selectedVariable = null;
            fbTypesView.Nodes.Clear();
            smvCodeRichTextBox.Text = "";
            messagesRichTextBox.Text = "";
        }

        private void loadFbSystem(string filename)
        {
            try
            {
                _parcer.ParseRecursive(filename, ShowMessage);
            }
            catch (Exception exception)
            {
                Program.ErrorMessage(exception.Message);
                return;
            }
            //fillTreeView();

            var compositeBlocks = _parcer.Storage.Types.Where((fbType) => fbType.Type == FBClass.Composite);
            bool solveDispatchingProblem = true;
            //_dispatchers = DispatchersCreator.Create(compositeBlocks, _parcer.Storage.Instances, solveDispatchingProblem);

            _executionModels = ExecutionModelsList.Generate(_parcer, solveDispatchingProblem);
        }

        private ProjectFileStructure loadProject(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryFormatter serializer = new BinaryFormatter();
            //ProjectFileStructure openedProject = new ProjectFileStructure();

            ProjectFileStructure openedProject = (ProjectFileStructure)serializer.Deserialize(fs);

            fs.Close();
            return openedProject;

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                /*if (mainModuleRichTextBox.Text != "")
                {

                    if (MessageBox.Show("Main module exists. Clear it?", "", MessageBoxButtons.YesNo) == DialogResult.OK)
                    {
                        mainModuleRichTextBox.Text = "";
                    }
                }*/
                if (Path.GetExtension(openFileDialog1.FileName) == projectFileExtension) //load saved project
                {
                    ProjectFileStructure openedProject = loadProject(openFileDialog1.FileName);
                    resetWorkspace(openedProject);
                } 
                else //load from .fbt files
                {
                    resetWorkspace();
                    loadFbSystem(openFileDialog1.FileName);
                }
                VisualizableStringTree t = new VisualizableStringTree();
                t.Construct(_parcer.Storage);
                fbTypesView.Nodes.Add(t.TreeViewRoot());

                try
                {
                    varDependencyGraph = new VarDependencyGraph(_parcer.Storage);
                    varDependencyGraph.Construct();
                }
                catch (KeyNotFoundException ex)
                {
                    ShowMessage(ex.Message);
                }

                //time scheduler data
                timersTextBox.Text = _parcer.Storage.TimersCount.ToString();
                timetypeTextBox.Text = _parcer.Storage.TimeSMVType;
                tmaxTextBox.Text = _parcer.Storage.Tmax.ToString();
            }
        }

        private void fillVarTreeView(IEnumerable<Variable> vars)
        {
            variablesTreeView.Nodes.Clear();
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

        private void fillEventsTreeView(IEnumerable<Event> events)
        {
            eventsTreeView.Nodes.Clear();
            TreeNode inputEvents = new TreeNode("InputEvents");
            TreeNode outputEvents = new TreeNode("OutputEvents");
            foreach (Event ev in events)
            {
                if (ev.Direction == Direction.Input) inputEvents.Nodes.Add(ev.Name, ev.Name);
                else if (ev.Direction == Direction.Output) outputEvents.Nodes.Add(ev.Name, ev.Name);
            }
            if (inputEvents.Nodes.Count > 0)
            {
                eventsTreeView.Nodes.Add(inputEvents);
                inputEvents.Expand();
            }
            if (outputEvents.Nodes.Count > 0)
            {
                eventsTreeView.Nodes.Add(outputEvents);
                outputEvents.Expand();
            }
        }

        private void fillEventsPriorityList()
        {
            eventsPriorityListBox.Items.Clear();
            if (_executionModels == null) return;
            ExecutionModel em = _executionModels.FirstOrDefault(model => model.FBTypeName == _selectedFbType);
            foreach (PriorityEvent pe in em.InputEventsPriorities)
            {
                eventsPriorityListBox.Items.Add(pe);
            }
        }

        private void fbTypesView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _selectedFbType = fbTypesView.SelectedNode.Text;
            groupBox1.Text = _selectedFbType;
            IEnumerable<Variable> vars = _parcer.Storage.Variables.Where(v => v.FBType == _selectedFbType);
            IEnumerable<Event> events = _parcer.Storage.Events.Where(ev => ev.FBType == _selectedFbType);
            fillVarTreeView(vars);
            fillEventsTreeView(events);

            FBType selectedFb = _parcer.Storage.Types.FirstOrDefault((fbType) => fbType.Name==_selectedFbType);
            groupBox2.Text = _selectedFbType;

            if (selectedFb.Type == FBClass.Composite)
            {
                //Instance order
                groupBox2.Enabled = true;
                cyclicDispatcherRadioButton.Select();
                _currentDisp = _executionModels.FirstOrDefault((em) => em.FBTypeName == _selectedFbType).Dispatcher;
                _fillInstanceList();

                //Events priority
                groupBox4.Enabled = false;
            }
            else
            {
                //Instance order
                instancePriorityListBox.Items.Clear();
                _currentDisp = null;
                groupBox2.Enabled = false;

                //Events priority
                groupBox4.Enabled = true;
                fillEventsPriorityList();
            }
            
        }

        private void _fillInstanceList()
        {
            instancePriorityListBox.Items.Clear();
            foreach (IPriorityContainer priorityInstance in _currentDisp.Instances)
            {
                instancePriorityListBox.Items.Add(priorityInstance);
            }
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
                varRangeTextBox.Text = _selectedVariable.SmvType.ToString();
                if (Smv.DataTypes.IsSimple(_selectedVariable.SmvType))
                {
                    upperLimitTextBox.Text = "0";
                    lowerLimitTextBox.Text = "0";
                }
                else
                {
                    string[] separator = { ".." };
                    string[] rangeSplit = varRangeTextBox.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    lowerLimitTextBox.Text = rangeSplit[0];
                    upperLimitTextBox.Text = rangeSplit[1];
                    
                }
                _connectedVars = varDependencyGraph.GetConnectedVariables(VarDependencyGraph.VariableKey(_selectedVariable));

                varIsConstantCheckBox.Checked = _selectedVariable.IsConstant;

                FBType selType = _parcer.Storage.Types.FirstOrDefault(t => t.Name == _selectedFbType);
                if (selType.Type == FBClass.Basic && _selectedVariable.Direction == Direction.Input && _connectedVars.Count()==0) varIsConstantCheckBox.Enabled = true;
                else varIsConstantCheckBox.Enabled = false;

                foreach (Variable connectedVar in _connectedVars)
                {
                    connectedVarsTreeView.Nodes.Add(connectedVar.ToString(), connectedVar.ToString());
                }
            }
        }

        private void propChangeButton_Click(object sender, EventArgs e)
        {
            if (Smv.DataTypes.IsSimple(_selectedVariable.SmvType)) { }

            else
            {
                string[] separator = {".."};
                string[] rangeSplit = varRangeTextBox.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (rangeSplit.Count() != 2)
                {
                    Program.ErrorMessage("Invalid range type!");
                    return;
                }

                _selectedVariable.SmvType = new Smv.DataTypes.RangeSmvType(Convert.ToInt32(rangeSplit[0]), Convert.ToInt32(rangeSplit[1]));
                foreach (Variable variable in _connectedVars)
                {
                    variable.SmvType = new Smv.DataTypes.RangeSmvType((Smv.DataTypes.RangeSmvType)_selectedVariable.SmvType);
                }
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
                name.Version.Build,
                name.Version.MajorRevision | name.Version.MinorRevision,
                copyrightAttribute.Copyright
                );
            MessageBox.Show(aboutMessage);
        }

        private void generateSMVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            smvCodeRichTextBox.Text = "";
            if (_parcer == null) return;
            SmvCodeGenerator translator = new SmvCodeGenerator(_parcer.Storage, _executionModels, Program.Settings, ShowMessage);
            translator.Check();
            foreach (string fbSmv in translator.TranslateAll())
            {
                smvCodeRichTextBox.Text += fbSmv;
            }

            smvCodeRichTextBox.Text += translator.GetTimeScheduler();

            //smvCodePage.Focus();
            if (mainModuleRichTextBox.Text == "")
            {
                mainModuleRichTextBox.Text = translator.GenerateMain();
            }
            else if (MessageBox.Show("Main module exists. Do you want to replace it?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                mainModuleRichTextBox.Text = translator.GenerateMain();
            }
            tabControl1.SelectTab(smvCodePage);
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FBType rootFbType = _parcer.Storage.Types.FirstOrDefault(t => t.IsRoot);
            if (rootFbType == null)
            {
                Program.ErrorMessage(Messages.No_FB_System_Loaded_Message_);
                return;
            }
            saveFileDialogProject.FileName = rootFbType.Name + projectFileExtension;
            if (saveFileDialogProject.ShowDialog() == DialogResult.OK)
            {
                ProjectFileStructure s = new ProjectFileStructure();
                s.ExecutionModels = _executionModels;
                s.Storage = _parcer.Storage;

                FileStream fs = new FileStream(saveFileDialogProject.FileName, FileMode.Create);
                BinaryFormatter serializer = new BinaryFormatter();
                //try
                //{
                serializer.Serialize(fs, s);
                //}

                /*if (saveFileDialogProject.ShowDialog() == DialogResult.OK)
                {
                
                }*/
            }
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
                if (mainModuleRichTextBox.Text != "")
                {
                    wr.Write(mainModuleRichTextBox.Text);
                }
                wr.Close();
            }
        }

        private void instancePriorityUp_Click(object sender, EventArgs e)
        {
            IPriorityContainer currentInstance = (IPriorityContainer)instancePriorityListBox.SelectedItem;
            if (currentInstance != null && currentInstance.Priority > 0)
            {
                IPriorityContainer higherPriorityInstance = _currentDisp.Instances.FirstOrDefault((inst) => inst.Priority == currentInstance.Priority - 1);
                currentInstance.Priority--;
                higherPriorityInstance.Priority++;
                _currentDisp.SortInstances();
                _fillInstanceList();
                instancePriorityListBox.SetSelected(currentInstance.Priority, true);
            }
        }

        private void instancePriorityDown_Click(object sender, EventArgs e)
        {
            IPriorityContainer currentInstance = (IPriorityContainer)instancePriorityListBox.SelectedItem;
            if (currentInstance != null && currentInstance.Priority < _currentDisp.Instances.Max((inst)=>inst.Priority))
            {
                IPriorityContainer lowerPriorityInstance = _currentDisp.Instances.FirstOrDefault((inst) => inst.Priority == currentInstance.Priority + 1);
                currentInstance.Priority++;
                lowerPriorityInstance.Priority--;
                _currentDisp.SortInstances();
                _fillInstanceList();
                instancePriorityListBox.SetSelected(currentInstance.Priority, true);
            }
        }

        private void eventPriorityUp_Click(object sender, EventArgs e)
        {
            ExecutionModel em = _executionModels.FirstOrDefault(model => model.FBTypeName == _selectedFbType);
            IPriorityContainer selectedEvent = (IPriorityContainer)eventsPriorityListBox.SelectedItem;
            if (selectedEvent != null && selectedEvent.Priority > 0)
            {
                IPriorityContainer higherPriorityEvent = em.InputEventsPriorities.FirstOrDefault(ep => ep.Priority == selectedEvent.Priority - 1);
                selectedEvent.Priority--;
                higherPriorityEvent.Priority++;
                em.SortInputEvents();
                fillEventsPriorityList();
                eventsPriorityListBox.SetSelected(selectedEvent.Priority, true);
            }
        }

        private void eventPriorityDown_Click(object sender, EventArgs e)
        {
            ExecutionModel em = _executionModels.FirstOrDefault(model => model.FBTypeName == _selectedFbType);
            IPriorityContainer selectedEvent = (IPriorityContainer)eventsPriorityListBox.SelectedItem;
            if (selectedEvent != null && selectedEvent.Priority < em.InputEventsPriorities.Max(ev=>ev.Priority))
            {
                IPriorityContainer lowerPriorityEvent = em.InputEventsPriorities.FirstOrDefault(ep => ep.Priority == selectedEvent.Priority + 1);
                selectedEvent.Priority++;
                lowerPriorityEvent.Priority--;
                em.SortInputEvents();
                fillEventsPriorityList();
                eventsPriorityListBox.SetSelected(selectedEvent.Priority, true);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        

        public ShowMessageDelegate ShowMessage = ShowMessageMethod;
        public static void ShowMessageMethod(string message)
        {
            _inst.messagesRichTextBox.Text += message + "\n";
        }

        private void varIsConstantCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedVariable != null)
            {
                _selectedVariable.IsConstant = varIsConstantCheckBox.Checked;
            }
        }

        private void timersButton_Click(object sender, EventArgs e)
        {
            _parcer.Storage.TimersCount = Convert.ToInt32(timersTextBox.Text);
            _parcer.Storage.TimeSMVType = timetypeTextBox.Text;
            _parcer.Storage.Tmax = Convert.ToInt32(tmaxTextBox.Text);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            /*
            _selectedVariable.Name = varArraySizeTextBox.Text;
            foreach (Variable variable in _connectedVars)
            {
                variable.Name = _selectedVariable.Name;
            }
            variablesTreeView.SelectedNode.Name = _selectedVariable.Name;
            _parcer.Storage.Variables.FirstOrDefault(v => v.FBType == _selectedFbType && v.Name == varArraySizeTextBox.Text).Name = _selectedVariable.Name;
            */
            if (Smv.DataTypes.IsNumberType(_selectedVariable.SmvType))
            {      
                int lb = Convert.ToInt32(lowerLimitTextBox.Text);
                int ub = Convert.ToInt32(upperLimitTextBox.Text);
                if (lb < ub)
                {
                    _selectedVariable.SmvType = new Smv.DataTypes.RangeSmvType(lb, ub);
                    foreach (Variable variable in _connectedVars)
                    {
                        variable.SmvType = new Smv.DataTypes.RangeSmvType((Smv.DataTypes.RangeSmvType)_selectedVariable.SmvType);
                    }
                }
                else
                {
                    Program.ErrorMessage("Invalid range type!");
                    return;
                }
            }
        }
    }
}
