using SMEditor.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMEditor.MainWindow.NewProjectWizard
{
    public partial class NewProjectWizard : Form
    {
        public NewProjectWizard()
        {
            InitializeComponent();
        }

        private void NewProjectWizard_Load(object sender, EventArgs e)
        {
            sizeEnum.Items.Add("Small (512x512)");
            sizeEnum.Items.Add("Small (768x768)");
            sizeEnum.Items.Add("Medium (1024x1024)");
            sizeEnum.Items.Add("Medium (1536x1536)");
            sizeEnum.Items.Add("Large (2048x2048)");

            sizeEnum.SelectedIndex = 0;
        }

        private void Create_Click(object o, EventArgs e)
        {
            if (Editor.Editor.projectLoaded)
            {
                if (MessageBox.Show("Are you sure you want to create a new project? Any unsaved data will be lost.", "Warning!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                    == DialogResult.No)
                {
                    return;
                }
            }

            HWDEScenarioSize s = HWDEScenarioSize.small512;
            Console.WriteLine(sizeEnum.Items[sizeEnum.SelectedIndex].ToString());
            switch(sizeEnum.Items[sizeEnum.SelectedIndex].ToString())
            {
                case "Small (512x512)":
                    s = HWDEScenarioSize.small512;
                    break;
                case "Small (768x768)":
                    s = HWDEScenarioSize.small768;
                    break;
                case "Medium (1024x1024)":
                    s = HWDEScenarioSize.medium1024;
                    break;
                case "Medium (1536x1536)":
                    s = HWDEScenarioSize.medium1536;
                    break;
                case "Large (2048x2048)":
                    s = HWDEScenarioSize.large2048;
                    break;
            }
            Editor.Editor.LoadNewProject(s);

            this.Close();
        }
        private void Cancel_Click(object o, EventArgs e)
        {
            this.Close();
        }
    }
}
