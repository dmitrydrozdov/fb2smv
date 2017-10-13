using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            _loadSettings();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _saveSettings();
            this.Close();
        }
        private void _loadSettings()
        {
            modCheckBox.Checked = Program.Settings.ModularArithmetics;
            useProcessesCheckBox.Checked = Program.Settings.UseProcesses;
            generateDummyPropertyCheckBox.Checked = Program.Settings.GenerateDummyProperty;
            intRealCheckBox.Checked = Program.Settings.nuXmvInfiniteDataTypes;
        }
        private void _saveSettings()
        {
            Program.Settings.ModularArithmetics = modCheckBox.Checked;
            Program.Settings.UseProcesses = useProcessesCheckBox.Checked;
            Program.Settings.GenerateDummyProperty = generateDummyPropertyCheckBox.Checked;
            Program.Settings.nuXmvInfiniteDataTypes = intRealCheckBox.Checked;
            Program.Settings.useDispatcher = !(asynchCheckBox.Checked);
        }

        private void intRealCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
