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
            this.saveSMVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSMVCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.smvCodePage = new System.Windows.Forms.TabPage();
            this.smvCodeRichTextBox = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.saveFileDialogProject = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialogSMV = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.VariablesPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
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
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1086, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.saveSMVToolStripMenuItem,
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
            this.saveProjectToolStripMenuItem.Enabled = false;
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveProjectToolStripMenuItem.Text = "Save project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // saveSMVToolStripMenuItem
            // 
            this.saveSMVToolStripMenuItem.Name = "saveSMVToolStripMenuItem";
            this.saveSMVToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveSMVToolStripMenuItem.Text = "Generate SMV code";
            this.saveSMVToolStripMenuItem.Click += new System.EventHandler(this.saveSMVToolStripMenuItem_Click);
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
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "FBDK File (.fbt)|*.fbt|XML File (.xml)|*.xml|All files|*.*";
            // 
            // fbTypesView
            // 
            this.fbTypesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fbTypesView.Location = new System.Drawing.Point(0, 0);
            this.fbTypesView.Name = "fbTypesView";
            this.fbTypesView.Size = new System.Drawing.Size(322, 702);
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
            this.varArraySizeTextBox.Size = new System.Drawing.Size(419, 20);
            this.varArraySizeTextBox.TabIndex = 11;
            // 
            // varRangeTextBox
            // 
            this.varRangeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varRangeTextBox.Location = new System.Drawing.Point(69, 55);
            this.varRangeTextBox.Name = "varRangeTextBox";
            this.varRangeTextBox.Size = new System.Drawing.Size(419, 20);
            this.varRangeTextBox.TabIndex = 10;
            // 
            // varTypeTextBox
            // 
            this.varTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varTypeTextBox.Enabled = false;
            this.varTypeTextBox.Location = new System.Drawing.Point(69, 29);
            this.varTypeTextBox.Name = "varTypeTextBox";
            this.varTypeTextBox.Size = new System.Drawing.Size(419, 20);
            this.varTypeTextBox.TabIndex = 9;
            // 
            // varNameTextBox
            // 
            this.varNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.varNameTextBox.Enabled = false;
            this.varNameTextBox.Location = new System.Drawing.Point(69, 3);
            this.varNameTextBox.Name = "varNameTextBox";
            this.varNameTextBox.Size = new System.Drawing.Size(419, 20);
            this.varNameTextBox.TabIndex = 8;
            // 
            // connectedVarsTreeView
            // 
            this.connectedVarsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectedVarsTreeView.Location = new System.Drawing.Point(2, 160);
            this.connectedVarsTreeView.Name = "connectedVarsTreeView";
            this.connectedVarsTreeView.Size = new System.Drawing.Size(486, 488);
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
            this.variablesTreeView.Size = new System.Drawing.Size(245, 651);
            this.variablesTreeView.TabIndex = 1;
            this.variablesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.variablesTreeView_AfterSelect);
            // 
            // propChangeButton
            // 
            this.propChangeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.propChangeButton.Location = new System.Drawing.Point(407, 107);
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
            this.tabControl1.Controls.Add(this.smvCodePage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(760, 702);
            this.tabControl1.TabIndex = 3;
            // 
            // VariablesPage
            // 
            this.VariablesPage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.VariablesPage.Controls.Add(this.groupBox1);
            this.VariablesPage.Location = new System.Drawing.Point(4, 22);
            this.VariablesPage.Name = "VariablesPage";
            this.VariablesPage.Padding = new System.Windows.Forms.Padding(3);
            this.VariablesPage.Size = new System.Drawing.Size(752, 676);
            this.VariablesPage.TabIndex = 0;
            this.VariablesPage.Text = "Variables";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitContainer2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(746, 670);
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
            this.splitContainer2.Size = new System.Drawing.Size(740, 651);
            this.splitContainer2.SplitterDistance = 245;
            this.splitContainer2.TabIndex = 3;
            // 
            // smvCodePage
            // 
            this.smvCodePage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.smvCodePage.Controls.Add(this.smvCodeRichTextBox);
            this.smvCodePage.Location = new System.Drawing.Point(4, 22);
            this.smvCodePage.Name = "smvCodePage";
            this.smvCodePage.Padding = new System.Windows.Forms.Padding(3);
            this.smvCodePage.Size = new System.Drawing.Size(752, 676);
            this.smvCodePage.TabIndex = 1;
            this.smvCodePage.Text = "SMV Code";
            // 
            // smvCodeRichTextBox
            // 
            this.smvCodeRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smvCodeRichTextBox.Location = new System.Drawing.Point(3, 3);
            this.smvCodeRichTextBox.Name = "smvCodeRichTextBox";
            this.smvCodeRichTextBox.Size = new System.Drawing.Size(746, 670);
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
            this.splitContainer1.Size = new System.Drawing.Size(1086, 702);
            this.splitContainer1.SplitterDistance = 322;
            this.splitContainer1.TabIndex = 4;
            // 
            // saveFileDialogSMV
            // 
            this.saveFileDialogSMV.Filter = "SMV File (.smv) | *.smv";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1086, 726);
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
        private System.Windows.Forms.ToolStripMenuItem saveSMVToolStripMenuItem;
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
    }
}

