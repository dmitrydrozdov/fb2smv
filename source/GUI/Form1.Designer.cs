namespace GUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSMVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSMVCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fbTypesView = new System.Windows.Forms.TreeView();
            this.label5 = new System.Windows.Forms.Label();
            this.varArraySizeTextBox = new System.Windows.Forms.TextBox();
            this.varRangeTextBox = new System.Windows.Forms.TextBox();
            this.varTypeTextBox = new System.Windows.Forms.TextBox();
            this.varNameTextBox = new System.Windows.Forms.TextBox();
            this.connectedVarsTreeView = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.variablesTreeView = new System.Windows.Forms.TreeView();
            this.propChangeButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.VariablesPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.eventsPage = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.eventsPriorityListBox = new System.Windows.Forms.ListBox();
            this.eventPriorityDown = new System.Windows.Forms.Button();
            this.eventPriorityUp = new System.Windows.Forms.Button();
            this.eventsTreeView = new System.Windows.Forms.TreeView();
            this.DispatcherPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.instancePriorityListBox = new System.Windows.Forms.ListBox();
            this.instancePriorityDown = new System.Windows.Forms.Button();
            this.instancePriorityUp = new System.Windows.Forms.Button();
            this.cyclicDispatcherRadioButton = new System.Windows.Forms.RadioButton();
            this.smvCodePage = new System.Windows.Forms.TabPage();
            this.smvCodeRichTextBox = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.saveFileDialogProject = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialogSMV = new System.Windows.Forms.SaveFileDialog();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.VariablesPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.eventsPage.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.DispatcherPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.smvCodePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(772, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.generateSMVToolStripMenuItem,
            this.saveSMVCodeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveProjectToolStripMenuItem.Text = "Save project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // generateSMVToolStripMenuItem
            // 
            this.generateSMVToolStripMenuItem.Name = "generateSMVToolStripMenuItem";
            this.generateSMVToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.generateSMVToolStripMenuItem.Text = "Generate SMV code";
            this.generateSMVToolStripMenuItem.Click += new System.EventHandler(this.generateSMVToolStripMenuItem_Click);
            // 
            // saveSMVCodeToolStripMenuItem
            // 
            this.saveSMVCodeToolStripMenuItem.Name = "saveSMVCodeToolStripMenuItem";
            this.saveSMVCodeToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveSMVCodeToolStripMenuItem.Text = "Save SMV Code";
            this.saveSMVCodeToolStripMenuItem.Click += new System.EventHandler(this.saveSMVCodeToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(12, 20);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "FBDK File (.fbt)|*.fbt|XML File (.xml)|*.xml|FB-to-SMV Converter project (.f2s) |" +
    " *.f2s|All files|*.*";
            // 
            // fbTypesView
            // 
            this.fbTypesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fbTypesView.Location = new System.Drawing.Point(0, 0);
            this.fbTypesView.Name = "fbTypesView";
            this.fbTypesView.Size = new System.Drawing.Size(228, 518);
            this.fbTypesView.TabIndex = 1;
            this.fbTypesView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fbTypesView_AfterSelect);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Array";
            // 
            // varArraySizeTextBox
            // 
            this.varArraySizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varArraySizeTextBox.Enabled = false;
            this.varArraySizeTextBox.Location = new System.Drawing.Point(69, 81);
            this.varArraySizeTextBox.Name = "varArraySizeTextBox";
            this.varArraySizeTextBox.Size = new System.Drawing.Size(272, 20);
            this.varArraySizeTextBox.TabIndex = 11;
            // 
            // varRangeTextBox
            // 
            this.varRangeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varRangeTextBox.Location = new System.Drawing.Point(69, 55);
            this.varRangeTextBox.Name = "varRangeTextBox";
            this.varRangeTextBox.Size = new System.Drawing.Size(272, 20);
            this.varRangeTextBox.TabIndex = 10;
            // 
            // varTypeTextBox
            // 
            this.varTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varTypeTextBox.Enabled = false;
            this.varTypeTextBox.Location = new System.Drawing.Point(69, 29);
            this.varTypeTextBox.Name = "varTypeTextBox";
            this.varTypeTextBox.Size = new System.Drawing.Size(272, 20);
            this.varTypeTextBox.TabIndex = 9;
            // 
            // varNameTextBox
            // 
            this.varNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varNameTextBox.Enabled = false;
            this.varNameTextBox.Location = new System.Drawing.Point(69, 3);
            this.varNameTextBox.Name = "varNameTextBox";
            this.varNameTextBox.Size = new System.Drawing.Size(272, 20);
            this.varNameTextBox.TabIndex = 8;
            // 
            // connectedVarsTreeView
            // 
            this.connectedVarsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectedVarsTreeView.Location = new System.Drawing.Point(2, 160);
            this.connectedVarsTreeView.Name = "connectedVarsTreeView";
            this.connectedVarsTreeView.Size = new System.Drawing.Size(339, 304);
            this.connectedVarsTreeView.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Connected variables";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "SMV Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // variablesTreeView
            // 
            this.variablesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.variablesTreeView.Location = new System.Drawing.Point(0, 0);
            this.variablesTreeView.Name = "variablesTreeView";
            this.variablesTreeView.Size = new System.Drawing.Size(172, 467);
            this.variablesTreeView.TabIndex = 1;
            this.variablesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.variablesTreeView_AfterSelect);
            // 
            // propChangeButton
            // 
            this.propChangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.propChangeButton.Location = new System.Drawing.Point(260, 107);
            this.propChangeButton.Name = "propChangeButton";
            this.propChangeButton.Size = new System.Drawing.Size(81, 25);
            this.propChangeButton.TabIndex = 13;
            this.propChangeButton.Text = "Change";
            this.propChangeButton.UseVisualStyleBackColor = true;
            this.propChangeButton.Click += new System.EventHandler(this.propChangeButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.VariablesPage);
            this.tabControl1.Controls.Add(this.eventsPage);
            this.tabControl1.Controls.Add(this.DispatcherPage);
            this.tabControl1.Controls.Add(this.smvCodePage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(540, 518);
            this.tabControl1.TabIndex = 3;
            // 
            // VariablesPage
            // 
            this.VariablesPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.VariablesPage.Controls.Add(this.groupBox1);
            this.VariablesPage.Location = new System.Drawing.Point(4, 22);
            this.VariablesPage.Name = "VariablesPage";
            this.VariablesPage.Padding = new System.Windows.Forms.Padding(3);
            this.VariablesPage.Size = new System.Drawing.Size(532, 492);
            this.VariablesPage.TabIndex = 0;
            this.VariablesPage.Text = "Variables";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitContainer2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(526, 486);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "<Block>";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 16);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.variablesTreeView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.propChangeButton);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.varArraySizeTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.varRangeTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.connectedVarsTreeView);
            this.splitContainer2.Panel2.Controls.Add(this.varTypeTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.varNameTextBox);
            this.splitContainer2.Size = new System.Drawing.Size(520, 467);
            this.splitContainer2.SplitterDistance = 172;
            this.splitContainer2.TabIndex = 3;
            // 
            // eventsPage
            // 
            this.eventsPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.eventsPage.Controls.Add(this.groupBox4);
            this.eventsPage.Controls.Add(this.eventsTreeView);
            this.eventsPage.Location = new System.Drawing.Point(4, 22);
            this.eventsPage.Name = "eventsPage";
            this.eventsPage.Size = new System.Drawing.Size(532, 492);
            this.eventsPage.TabIndex = 3;
            this.eventsPage.Text = "Events";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.eventsPriorityListBox);
            this.groupBox4.Controls.Add(this.eventPriorityDown);
            this.groupBox4.Controls.Add(this.eventPriorityUp);
            this.groupBox4.Location = new System.Drawing.Point(172, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(254, 241);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Input events proirity";
            // 
            // eventsPriorityListBox
            // 
            this.eventsPriorityListBox.AllowDrop = true;
            this.eventsPriorityListBox.FormattingEnabled = true;
            this.eventsPriorityListBox.Location = new System.Drawing.Point(10, 19);
            this.eventsPriorityListBox.Name = "eventsPriorityListBox";
            this.eventsPriorityListBox.Size = new System.Drawing.Size(184, 212);
            this.eventsPriorityListBox.TabIndex = 4;
            // 
            // eventPriorityDown
            // 
            this.eventPriorityDown.Location = new System.Drawing.Point(200, 48);
            this.eventPriorityDown.Name = "eventPriorityDown";
            this.eventPriorityDown.Size = new System.Drawing.Size(45, 23);
            this.eventPriorityDown.TabIndex = 6;
            this.eventPriorityDown.Text = "Down";
            this.eventPriorityDown.UseVisualStyleBackColor = true;
            this.eventPriorityDown.Click += new System.EventHandler(this.eventPriorityDown_Click);
            // 
            // eventPriorityUp
            // 
            this.eventPriorityUp.Location = new System.Drawing.Point(200, 19);
            this.eventPriorityUp.Name = "eventPriorityUp";
            this.eventPriorityUp.Size = new System.Drawing.Size(46, 23);
            this.eventPriorityUp.TabIndex = 5;
            this.eventPriorityUp.Text = "Up";
            this.eventPriorityUp.UseVisualStyleBackColor = true;
            this.eventPriorityUp.Click += new System.EventHandler(this.eventPriorityUp_Click);
            // 
            // eventsTreeView
            // 
            this.eventsTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.eventsTreeView.Location = new System.Drawing.Point(0, 0);
            this.eventsTreeView.Name = "eventsTreeView";
            this.eventsTreeView.Size = new System.Drawing.Size(166, 492);
            this.eventsTreeView.TabIndex = 0;
            // 
            // DispatcherPage
            // 
            this.DispatcherPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.DispatcherPage.Controls.Add(this.groupBox2);
            this.DispatcherPage.Location = new System.Drawing.Point(4, 22);
            this.DispatcherPage.Name = "DispatcherPage";
            this.DispatcherPage.Size = new System.Drawing.Size(532, 492);
            this.DispatcherPage.TabIndex = 2;
            this.DispatcherPage.Text = "Dispatcher";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.cyclicDispatcherRadioButton);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(532, 492);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.instancePriorityListBox);
            this.groupBox3.Controls.Add(this.instancePriorityDown);
            this.groupBox3.Controls.Add(this.instancePriorityUp);
            this.groupBox3.Location = new System.Drawing.Point(143, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(254, 241);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Execution Order";
            // 
            // instancePriorityListBox
            // 
            this.instancePriorityListBox.AllowDrop = true;
            this.instancePriorityListBox.FormattingEnabled = true;
            this.instancePriorityListBox.Location = new System.Drawing.Point(10, 19);
            this.instancePriorityListBox.Name = "instancePriorityListBox";
            this.instancePriorityListBox.Size = new System.Drawing.Size(184, 199);
            this.instancePriorityListBox.TabIndex = 4;
            // 
            // instancePriorityDown
            // 
            this.instancePriorityDown.Location = new System.Drawing.Point(200, 48);
            this.instancePriorityDown.Name = "instancePriorityDown";
            this.instancePriorityDown.Size = new System.Drawing.Size(45, 23);
            this.instancePriorityDown.TabIndex = 6;
            this.instancePriorityDown.Text = "Down";
            this.instancePriorityDown.UseVisualStyleBackColor = true;
            this.instancePriorityDown.Click += new System.EventHandler(this.instancePriorityDown_Click);
            // 
            // instancePriorityUp
            // 
            this.instancePriorityUp.Location = new System.Drawing.Point(200, 19);
            this.instancePriorityUp.Name = "instancePriorityUp";
            this.instancePriorityUp.Size = new System.Drawing.Size(46, 23);
            this.instancePriorityUp.TabIndex = 5;
            this.instancePriorityUp.Text = "Up";
            this.instancePriorityUp.UseVisualStyleBackColor = true;
            this.instancePriorityUp.Click += new System.EventHandler(this.instancePriorityUp_Click);
            // 
            // cyclicDispatcherRadioButton
            // 
            this.cyclicDispatcherRadioButton.AutoSize = true;
            this.cyclicDispatcherRadioButton.Location = new System.Drawing.Point(6, 19);
            this.cyclicDispatcherRadioButton.Name = "cyclicDispatcherRadioButton";
            this.cyclicDispatcherRadioButton.Size = new System.Drawing.Size(105, 17);
            this.cyclicDispatcherRadioButton.TabIndex = 3;
            this.cyclicDispatcherRadioButton.TabStop = true;
            this.cyclicDispatcherRadioButton.Text = "Cyclic dispatcher";
            this.cyclicDispatcherRadioButton.UseVisualStyleBackColor = true;
            // 
            // smvCodePage
            // 
            this.smvCodePage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.smvCodePage.Controls.Add(this.smvCodeRichTextBox);
            this.smvCodePage.Location = new System.Drawing.Point(4, 22);
            this.smvCodePage.Name = "smvCodePage";
            this.smvCodePage.Padding = new System.Windows.Forms.Padding(3);
            this.smvCodePage.Size = new System.Drawing.Size(532, 492);
            this.smvCodePage.TabIndex = 1;
            this.smvCodePage.Text = "SMV Code";
            // 
            // smvCodeRichTextBox
            // 
            this.smvCodeRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smvCodeRichTextBox.Location = new System.Drawing.Point(3, 3);
            this.smvCodeRichTextBox.Name = "smvCodeRichTextBox";
            this.smvCodeRichTextBox.Size = new System.Drawing.Size(526, 486);
            this.smvCodeRichTextBox.TabIndex = 0;
            this.smvCodeRichTextBox.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fbTypesView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(772, 518);
            this.splitContainer1.SplitterDistance = 228;
            this.splitContainer1.TabIndex = 4;
            // 
            // saveFileDialogProject
            // 
            this.saveFileDialogProject.Filter = "FB-to-SMV Converter project (.f2s) | *.f2s";
            // 
            // saveFileDialogSMV
            // 
            this.saveFileDialogSMV.Filter = "SMV File (.smv) | *.smv";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 542);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "FB-to-SMV Converter";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.VariablesPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.eventsPage.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.DispatcherPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.smvCodePage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TreeView fbTypesView;
        private System.Windows.Forms.TreeView variablesTreeView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView connectedVarsTreeView;
        private System.Windows.Forms.TextBox varRangeTextBox;
        private System.Windows.Forms.TextBox varTypeTextBox;
        private System.Windows.Forms.TextBox varNameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox varArraySizeTextBox;
        private System.Windows.Forms.Button propChangeButton;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSMVToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage VariablesPage;
        private System.Windows.Forms.TabPage smvCodePage;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox smvCodeRichTextBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialogProject;
        private System.Windows.Forms.ToolStripMenuItem saveSMVCodeToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialogSMV;
        private System.Windows.Forms.TabPage DispatcherPage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton cyclicDispatcherRadioButton;
        private System.Windows.Forms.ListBox instancePriorityListBox;
        private System.Windows.Forms.Button instancePriorityDown;
        private System.Windows.Forms.Button instancePriorityUp;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabPage eventsPage;
        private System.Windows.Forms.TreeView eventsTreeView;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox eventsPriorityListBox;
        private System.Windows.Forms.Button eventPriorityDown;
        private System.Windows.Forms.Button eventPriorityUp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    }
}

