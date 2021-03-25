namespace SMEditor.MainWindow.NewProjectWizard
{
    partial class NewProjectWizard
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
            this.sizeEnum = new System.Windows.Forms.ComboBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.createButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sizeEnum
            // 
            this.sizeEnum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeEnum.FormattingEnabled = true;
            this.sizeEnum.Location = new System.Drawing.Point(12, 34);
            this.sizeEnum.Name = "sizeEnum";
            this.sizeEnum.Size = new System.Drawing.Size(227, 24);
            this.sizeEnum.TabIndex = 0;
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(9, 14);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(35, 17);
            this.sizeLabel.TabIndex = 1;
            this.sizeLabel.Text = "Size";
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(164, 71);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 27);
            this.createButton.TabIndex = 2;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(Create_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(83, 71);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 27);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(Cancel_Click);
            // 
            // NewProjectWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 110);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.sizeEnum);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectWizard";
            this.Text = "NewProjectWizard";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NewProjectWizard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox sizeEnum;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button cancelButton;
    }
}