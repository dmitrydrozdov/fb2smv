namespace GUI
{
    partial class SettingsWindow
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
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.modCheckBox = new System.Windows.Forms.CheckBox();
            this.useProcessesCheckBox = new System.Windows.Forms.CheckBox();
            this.generateDummyPropertyCheckBox = new System.Windows.Forms.CheckBox();
            this.intRealCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(134, 109);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(215, 109);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // modCheckBox
            // 
            this.modCheckBox.AutoSize = true;
            this.modCheckBox.Location = new System.Drawing.Point(13, 13);
            this.modCheckBox.Name = "modCheckBox";
            this.modCheckBox.Size = new System.Drawing.Size(112, 17);
            this.modCheckBox.TabIndex = 2;
            this.modCheckBox.Text = "Modular arithmetic";
            this.modCheckBox.UseVisualStyleBackColor = true;
            // 
            // useProcessesCheckBox
            // 
            this.useProcessesCheckBox.AutoSize = true;
            this.useProcessesCheckBox.Location = new System.Drawing.Point(13, 37);
            this.useProcessesCheckBox.Name = "useProcessesCheckBox";
            this.useProcessesCheckBox.Size = new System.Drawing.Size(96, 17);
            this.useProcessesCheckBox.TabIndex = 3;
            this.useProcessesCheckBox.Text = "Use processes";
            this.useProcessesCheckBox.UseVisualStyleBackColor = true;
            // 
            // generateDummyPropertyCheckBox
            // 
            this.generateDummyPropertyCheckBox.AutoSize = true;
            this.generateDummyPropertyCheckBox.Location = new System.Drawing.Point(13, 61);
            this.generateDummyPropertyCheckBox.Name = "generateDummyPropertyCheckBox";
            this.generateDummyPropertyCheckBox.Size = new System.Drawing.Size(169, 17);
            this.generateDummyPropertyCheckBox.TabIndex = 4;
            this.generateDummyPropertyCheckBox.Text = "Generate dummy LTL property";
            this.generateDummyPropertyCheckBox.UseVisualStyleBackColor = true;
            // 
            // intRealCheckBox
            // 
            this.intRealCheckBox.AutoSize = true;
            this.intRealCheckBox.Location = new System.Drawing.Point(12, 84);
            this.intRealCheckBox.Name = "intRealCheckBox";
            this.intRealCheckBox.Size = new System.Drawing.Size(152, 17);
            this.intRealCheckBox.TabIndex = 4;
            this.intRealCheckBox.Text = "Integers and Reals in SMV";
            this.intRealCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsWindow
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(302, 144);
            this.Controls.Add(this.intRealCheckBox);
            this.Controls.Add(this.generateDummyPropertyCheckBox);
            this.Controls.Add(this.useProcessesCheckBox);
            this.Controls.Add(this.modCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox modCheckBox;
        private System.Windows.Forms.CheckBox useProcessesCheckBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox generateDummyPropertyCheckBox;
        private System.Windows.Forms.CheckBox intRealCheckBox;
    }
}