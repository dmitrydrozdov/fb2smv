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
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.updateButton = new System.Windows.Forms.Button();
            this.upperLimitLabel = new System.Windows.Forms.Label();
            this.lowerLimitLabel = new System.Windows.Forms.Label();
            this.upperLimitTextBox = new System.Windows.Forms.TextBox();
            this.lowerLimitTextBox = new System.Windows.Forms.TextBox();
            this.varIsConstantCheckBox = new System.Windows.Forms.CheckBox();
            this.eventsPage = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.eventsPriorityListBox = new System.Windows.Forms.ListBox();
            this.eventPriorityDown = new System.Windows.Forms.Button();
            this.eventPriorityUp = new System.Windows.Forms.Button();
            this.eventsTreeView = new System.Windows.Forms.TreeView();
            this.DispatcherPage = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tmaxTextBox = new System.Windows.Forms.TextBox();
            this.timetypeTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.timersButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.timersTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.instancePriorityListBox = new System.Windows.Forms.ListBox();
            this.instancePriorityDown = new System.Windows.Forms.Button();
            this.instancePriorityUp = new System.Windows.Forms.Button();
            this.cyclicDispatcherRadioButton = new System.Windows.Forms.RadioButton();
            this.smvCodePage = new System.Windows.Forms.TabPage();
            this.smvCodeRichTextBox = new System.Windows.Forms.RichTextBox();
            this.mainModuleTab = new System.Windows.Forms.TabPage();
            this.mainModuleRichTextBox = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.saveFileDialogProject = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialogSMV = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.messagesRichTextBox = new System.Windows.Forms.RichTextBox();
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
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.smvCodePage.SuspendLayout();
            this.mainModuleTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1151, 30);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.saveProjectToolStripMenuItem.Text = "Save project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // generateSMVToolStripMenuItem
            // 
            this.generateSMVToolStripMenuItem.Name = "generateSMVToolStripMenuItem";
            this.generateSMVToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.generateSMVToolStripMenuItem.Text = "Generate SMV code";
            this.generateSMVToolStripMenuItem.Click += new System.EventHandler(this.generateSMVToolStripMenuItem_Click);
            // 
            // saveSMVCodeToolStripMenuItem
            // 
            this.saveSMVCodeToolStripMenuItem.Name = "saveSMVCodeToolStripMenuItem";
            this.saveSMVCodeToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.saveSMVCodeToolStripMenuItem.Text = "Save SMV Code";
            this.saveSMVCodeToolStripMenuItem.Click += new System.EventHandler(this.saveSMVCodeToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(14, 24);
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
            this.fbTypesView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fbTypesView.Name = "fbTypesView";
            this.fbTypesView.Size = new System.Drawing.Size(199, 641);
            this.fbTypesView.TabIndex = 1;
            this.fbTypesView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fbTypesView_AfterSelect);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 131);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 20);
            this.label5.TabIndex = 12;
            this.label5.Text = "Array";
            // 
            // varArraySizeTextBox
            // 
            this.varArraySizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varArraySizeTextBox.Location = new System.Drawing.Point(104, 125);
            this.varArraySizeTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.varArraySizeTextBox.Name = "varArraySizeTextBox";
            this.varArraySizeTextBox.Size = new System.Drawing.Size(493, 26);
            this.varArraySizeTextBox.TabIndex = 11;
            // 
            // varRangeTextBox
            // 
            this.varRangeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varRangeTextBox.Location = new System.Drawing.Point(104, 85);
            this.varRangeTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.varRangeTextBox.Name = "varRangeTextBox";
            this.varRangeTextBox.Size = new System.Drawing.Size(493, 26);
            this.varRangeTextBox.TabIndex = 10;
            // 
            // varTypeTextBox
            // 
            this.varTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varTypeTextBox.Location = new System.Drawing.Point(104, 45);
            this.varTypeTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.varTypeTextBox.Name = "varTypeTextBox";
            this.varTypeTextBox.Size = new System.Drawing.Size(493, 26);
            this.varTypeTextBox.TabIndex = 9;
            // 
            // varNameTextBox
            // 
            this.varNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varNameTextBox.Location = new System.Drawing.Point(104, 5);
            this.varNameTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.varNameTextBox.Name = "varNameTextBox";
            this.varNameTextBox.Size = new System.Drawing.Size(493, 26);
            this.varNameTextBox.TabIndex = 8;
            // 
            // connectedVarsTreeView
            // 
            this.connectedVarsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectedVarsTreeView.Location = new System.Drawing.Point(3, 367);
            this.connectedVarsTreeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.connectedVarsTreeView.Name = "connectedVarsTreeView";
            this.connectedVarsTreeView.Size = new System.Drawing.Size(593, 194);
            this.connectedVarsTreeView.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 342);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Connected variables";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 95);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "SMV Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 55);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // variablesTreeView
            // 
            this.variablesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.variablesTreeView.Location = new System.Drawing.Point(0, 0);
            this.variablesTreeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.variablesTreeView.Name = "variablesTreeView";
            this.variablesTreeView.Size = new System.Drawing.Size(303, 569);
            this.variablesTreeView.TabIndex = 1;
            this.variablesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.variablesTreeView_AfterSelect);
            // 
            // propChangeButton
            // 
            this.propChangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.propChangeButton.Location = new System.Drawing.Point(463, 279);
            this.propChangeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.propChangeButton.Name = "propChangeButton";
            this.propChangeButton.Size = new System.Drawing.Size(122, 38);
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
            this.tabControl1.Controls.Add(this.mainModuleTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(946, 641);
            this.tabControl1.TabIndex = 3;
            // 
            // VariablesPage
            // 
            this.VariablesPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.VariablesPage.Controls.Add(this.groupBox1);
            this.VariablesPage.Location = new System.Drawing.Point(4, 29);
            this.VariablesPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.VariablesPage.Name = "VariablesPage";
            this.VariablesPage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.VariablesPage.Size = new System.Drawing.Size(938, 608);
            this.VariablesPage.TabIndex = 0;
            this.VariablesPage.Text = "Variables";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitContainer2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(4, 5);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(930, 598);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "<Block>";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(4, 24);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.variablesTreeView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.updateButton);
            this.splitContainer2.Panel2.Controls.Add(this.upperLimitLabel);
            this.splitContainer2.Panel2.Controls.Add(this.lowerLimitLabel);
            this.splitContainer2.Panel2.Controls.Add(this.upperLimitTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.lowerLimitTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.varIsConstantCheckBox);
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
            this.splitContainer2.Size = new System.Drawing.Size(922, 569);
            this.splitContainer2.SplitterDistance = 303;
            this.splitContainer2.SplitterWidth = 6;
            this.splitContainer2.TabIndex = 3;
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(103, 278);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(99, 39);
            this.updateButton.TabIndex = 19;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // upperLimitLabel
            // 
            this.upperLimitLabel.AutoSize = true;
            this.upperLimitLabel.Location = new System.Drawing.Point(9, 200);
            this.upperLimitLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.upperLimitLabel.Name = "upperLimitLabel";
            this.upperLimitLabel.Size = new System.Drawing.Size(31, 20);
            this.upperLimitLabel.TabIndex = 18;
            this.upperLimitLabel.Text = "UL";
            // 
            // lowerLimitLabel
            // 
            this.lowerLimitLabel.AutoSize = true;
            this.lowerLimitLabel.Location = new System.Drawing.Point(9, 167);
            this.lowerLimitLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lowerLimitLabel.Name = "lowerLimitLabel";
            this.lowerLimitLabel.Size = new System.Drawing.Size(29, 20);
            this.lowerLimitLabel.TabIndex = 17;
            this.lowerLimitLabel.Text = "LL";
            // 
            // upperLimitTextBox
            // 
            this.upperLimitTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.upperLimitTextBox.Location = new System.Drawing.Point(103, 197);
            this.upperLimitTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.upperLimitTextBox.Name = "upperLimitTextBox";
            this.upperLimitTextBox.Size = new System.Drawing.Size(493, 26);
            this.upperLimitTextBox.TabIndex = 16;
            // 
            // lowerLimitTextBox
            // 
            this.lowerLimitTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lowerLimitTextBox.HideSelection = false;
            this.lowerLimitTextBox.Location = new System.Drawing.Point(104, 164);
            this.lowerLimitTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lowerLimitTextBox.Name = "lowerLimitTextBox";
            this.lowerLimitTextBox.Size = new System.Drawing.Size(493, 26);
            this.lowerLimitTextBox.TabIndex = 15;
            // 
            // varIsConstantCheckBox
            // 
            this.varIsConstantCheckBox.AutoSize = true;
            this.varIsConstantCheckBox.Location = new System.Drawing.Point(104, 233);
            this.varIsConstantCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.varIsConstantCheckBox.Name = "varIsConstantCheckBox";
            this.varIsConstantCheckBox.Size = new System.Drawing.Size(98, 24);
            this.varIsConstantCheckBox.TabIndex = 14;
            this.varIsConstantCheckBox.Text = "Constant";
            this.varIsConstantCheckBox.UseVisualStyleBackColor = true;
            this.varIsConstantCheckBox.CheckedChanged += new System.EventHandler(this.varIsConstantCheckBox_CheckedChanged);
            // 
            // eventsPage
            // 
            this.eventsPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.eventsPage.Controls.Add(this.groupBox4);
            this.eventsPage.Controls.Add(this.eventsTreeView);
            this.eventsPage.Location = new System.Drawing.Point(4, 29);
            this.eventsPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.eventsPage.Name = "eventsPage";
            this.eventsPage.Size = new System.Drawing.Size(938, 608);
            this.eventsPage.TabIndex = 3;
            this.eventsPage.Text = "Events";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.eventsPriorityListBox);
            this.groupBox4.Controls.Add(this.eventPriorityDown);
            this.groupBox4.Controls.Add(this.eventPriorityUp);
            this.groupBox4.Location = new System.Drawing.Point(258, 5);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(381, 371);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Input events proirity";
            // 
            // eventsPriorityListBox
            // 
            this.eventsPriorityListBox.AllowDrop = true;
            this.eventsPriorityListBox.FormattingEnabled = true;
            this.eventsPriorityListBox.ItemHeight = 20;
            this.eventsPriorityListBox.Location = new System.Drawing.Point(15, 29);
            this.eventsPriorityListBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.eventsPriorityListBox.Name = "eventsPriorityListBox";
            this.eventsPriorityListBox.Size = new System.Drawing.Size(274, 324);
            this.eventsPriorityListBox.TabIndex = 4;
            // 
            // eventPriorityDown
            // 
            this.eventPriorityDown.Location = new System.Drawing.Point(300, 74);
            this.eventPriorityDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.eventPriorityDown.Name = "eventPriorityDown";
            this.eventPriorityDown.Size = new System.Drawing.Size(68, 35);
            this.eventPriorityDown.TabIndex = 6;
            this.eventPriorityDown.Text = "Down";
            this.eventPriorityDown.UseVisualStyleBackColor = true;
            this.eventPriorityDown.Click += new System.EventHandler(this.eventPriorityDown_Click);
            // 
            // eventPriorityUp
            // 
            this.eventPriorityUp.Location = new System.Drawing.Point(300, 29);
            this.eventPriorityUp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.eventPriorityUp.Name = "eventPriorityUp";
            this.eventPriorityUp.Size = new System.Drawing.Size(69, 35);
            this.eventPriorityUp.TabIndex = 5;
            this.eventPriorityUp.Text = "Up";
            this.eventPriorityUp.UseVisualStyleBackColor = true;
            this.eventPriorityUp.Click += new System.EventHandler(this.eventPriorityUp_Click);
            // 
            // eventsTreeView
            // 
            this.eventsTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.eventsTreeView.Location = new System.Drawing.Point(0, 0);
            this.eventsTreeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.eventsTreeView.Name = "eventsTreeView";
            this.eventsTreeView.Size = new System.Drawing.Size(247, 608);
            this.eventsTreeView.TabIndex = 0;
            // 
            // DispatcherPage
            // 
            this.DispatcherPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.DispatcherPage.Controls.Add(this.groupBox5);
            this.DispatcherPage.Controls.Add(this.groupBox2);
            this.DispatcherPage.Location = new System.Drawing.Point(4, 29);
            this.DispatcherPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.DispatcherPage.Name = "DispatcherPage";
            this.DispatcherPage.Size = new System.Drawing.Size(938, 608);
            this.DispatcherPage.TabIndex = 2;
            this.DispatcherPage.Text = "Dispatcher";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tmaxTextBox);
            this.groupBox5.Controls.Add(this.timetypeTextBox);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.timersButton);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.timersTextBox);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox5.Location = new System.Drawing.Point(0, 388);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(938, 220);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "TimeScheduler";
            // 
            // tmaxTextBox
            // 
            this.tmaxTextBox.Location = new System.Drawing.Point(167, 94);
            this.tmaxTextBox.Name = "tmaxTextBox";
            this.tmaxTextBox.Size = new System.Drawing.Size(172, 26);
            this.tmaxTextBox.TabIndex = 6;
            // 
            // timetypeTextBox
            // 
            this.timetypeTextBox.Location = new System.Drawing.Point(167, 62);
            this.timetypeTextBox.Name = "timetypeTextBox";
            this.timetypeTextBox.Size = new System.Drawing.Size(172, 26);
            this.timetypeTextBox.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 20);
            this.label8.TabIndex = 4;
            this.label8.Text = "Tmax";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Time SMV type";
            // 
            // timersButton
            // 
            this.timersButton.Location = new System.Drawing.Point(354, 56);
            this.timersButton.Name = "timersButton";
            this.timersButton.Size = new System.Drawing.Size(75, 39);
            this.timersButton.TabIndex = 2;
            this.timersButton.Text = "Apply";
            this.timersButton.UseVisualStyleBackColor = true;
            this.timersButton.Click += new System.EventHandler(this.timersButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 20);
            this.label6.TabIndex = 1;
            this.label6.Text = "Number of timers";
            // 
            // timersTextBox
            // 
            this.timersTextBox.Location = new System.Drawing.Point(167, 30);
            this.timersTextBox.Name = "timersTextBox";
            this.timersTextBox.Size = new System.Drawing.Size(172, 26);
            this.timersTextBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.cyclicDispatcherRadioButton);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(938, 383);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dispatcher control";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.instancePriorityListBox);
            this.groupBox3.Controls.Add(this.instancePriorityDown);
            this.groupBox3.Controls.Add(this.instancePriorityUp);
            this.groupBox3.Location = new System.Drawing.Point(214, 29);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(381, 348);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Execution Order";
            // 
            // instancePriorityListBox
            // 
            this.instancePriorityListBox.AllowDrop = true;
            this.instancePriorityListBox.FormattingEnabled = true;
            this.instancePriorityListBox.ItemHeight = 20;
            this.instancePriorityListBox.Location = new System.Drawing.Point(15, 29);
            this.instancePriorityListBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.instancePriorityListBox.Name = "instancePriorityListBox";
            this.instancePriorityListBox.Size = new System.Drawing.Size(274, 304);
            this.instancePriorityListBox.TabIndex = 4;
            // 
            // instancePriorityDown
            // 
            this.instancePriorityDown.Location = new System.Drawing.Point(300, 74);
            this.instancePriorityDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.instancePriorityDown.Name = "instancePriorityDown";
            this.instancePriorityDown.Size = new System.Drawing.Size(68, 35);
            this.instancePriorityDown.TabIndex = 6;
            this.instancePriorityDown.Text = "Down";
            this.instancePriorityDown.UseVisualStyleBackColor = true;
            this.instancePriorityDown.Click += new System.EventHandler(this.instancePriorityDown_Click);
            // 
            // instancePriorityUp
            // 
            this.instancePriorityUp.Location = new System.Drawing.Point(300, 29);
            this.instancePriorityUp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.instancePriorityUp.Name = "instancePriorityUp";
            this.instancePriorityUp.Size = new System.Drawing.Size(69, 35);
            this.instancePriorityUp.TabIndex = 5;
            this.instancePriorityUp.Text = "Up";
            this.instancePriorityUp.UseVisualStyleBackColor = true;
            this.instancePriorityUp.Click += new System.EventHandler(this.instancePriorityUp_Click);
            // 
            // cyclicDispatcherRadioButton
            // 
            this.cyclicDispatcherRadioButton.AutoSize = true;
            this.cyclicDispatcherRadioButton.Location = new System.Drawing.Point(9, 29);
            this.cyclicDispatcherRadioButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cyclicDispatcherRadioButton.Name = "cyclicDispatcherRadioButton";
            this.cyclicDispatcherRadioButton.Size = new System.Drawing.Size(159, 24);
            this.cyclicDispatcherRadioButton.TabIndex = 3;
            this.cyclicDispatcherRadioButton.TabStop = true;
            this.cyclicDispatcherRadioButton.Text = "Cyclic dispatcher";
            this.cyclicDispatcherRadioButton.UseVisualStyleBackColor = true;
            // 
            // smvCodePage
            // 
            this.smvCodePage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.smvCodePage.Controls.Add(this.smvCodeRichTextBox);
            this.smvCodePage.Location = new System.Drawing.Point(4, 29);
            this.smvCodePage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.smvCodePage.Name = "smvCodePage";
            this.smvCodePage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.smvCodePage.Size = new System.Drawing.Size(938, 608);
            this.smvCodePage.TabIndex = 1;
            this.smvCodePage.Text = "SMV Code";
            // 
            // smvCodeRichTextBox
            // 
            this.smvCodeRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smvCodeRichTextBox.Location = new System.Drawing.Point(4, 5);
            this.smvCodeRichTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.smvCodeRichTextBox.Name = "smvCodeRichTextBox";
            this.smvCodeRichTextBox.Size = new System.Drawing.Size(930, 598);
            this.smvCodeRichTextBox.TabIndex = 0;
            this.smvCodeRichTextBox.Text = "";
            // 
            // mainModuleTab
            // 
            this.mainModuleTab.Controls.Add(this.mainModuleRichTextBox);
            this.mainModuleTab.Location = new System.Drawing.Point(4, 29);
            this.mainModuleTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mainModuleTab.Name = "mainModuleTab";
            this.mainModuleTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mainModuleTab.Size = new System.Drawing.Size(938, 608);
            this.mainModuleTab.TabIndex = 4;
            this.mainModuleTab.Text = "Main";
            this.mainModuleTab.UseVisualStyleBackColor = true;
            // 
            // mainModuleRichTextBox
            // 
            this.mainModuleRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainModuleRichTextBox.Location = new System.Drawing.Point(4, 5);
            this.mainModuleRichTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mainModuleRichTextBox.Name = "mainModuleRichTextBox";
            this.mainModuleRichTextBox.Size = new System.Drawing.Size(930, 598);
            this.mainModuleRichTextBox.TabIndex = 0;
            this.mainModuleRichTextBox.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fbTypesView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1151, 641);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.SplitterWidth = 6;
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
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 30);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.messagesRichTextBox);
            this.splitContainer3.Size = new System.Drawing.Size(1151, 870);
            this.splitContainer3.SplitterDistance = 641;
            this.splitContainer3.SplitterWidth = 6;
            this.splitContainer3.TabIndex = 5;
            // 
            // messagesRichTextBox
            // 
            this.messagesRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.messagesRichTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.messagesRichTextBox.Name = "messagesRichTextBox";
            this.messagesRichTextBox.Size = new System.Drawing.Size(1151, 223);
            this.messagesRichTextBox.TabIndex = 0;
            this.messagesRichTextBox.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1151, 900);
            this.Controls.Add(this.splitContainer3);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.smvCodePage.ResumeLayout(false);
            this.mainModuleTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage mainModuleTab;
        private System.Windows.Forms.RichTextBox mainModuleRichTextBox;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox messagesRichTextBox;
        private System.Windows.Forms.CheckBox varIsConstantCheckBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button timersButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox timersTextBox;
        private System.Windows.Forms.TextBox tmaxTextBox;
        private System.Windows.Forms.TextBox timetypeTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox lowerLimitTextBox;
        private System.Windows.Forms.TextBox upperLimitTextBox;
        private System.Windows.Forms.Label lowerLimitLabel;
        private System.Windows.Forms.Label upperLimitLabel;
        private System.Windows.Forms.Button updateButton;
    }
}

