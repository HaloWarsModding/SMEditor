using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing.Drawing2D;
using Syncfusion.WinForms.Controls;
using System;
using System.Windows.Forms;
using Syncfusion.Runtime.Serialization;
using SMEditor.Editor.Layout;
using SMEditor.Editor;
using SMEditor.Editor.Tools;
using SMEditor.MainWindow.NewProjectWizard;

namespace SMEditor
{
    partial class MainWindowOld
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindowOld));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.d3D11Control = new SlimDX.Windows.D3D11Control();
            this.layout = new Syncfusion.Windows.Forms.Tools.SplitContainerAdv();
            this.contextToolBar = new System.Windows.Forms.ToolStrip();
            this.gridGroupingControl1 = new Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl();
            this.dockingManager = new Syncfusion.Windows.Forms.Tools.DockingManager(this.components);
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layout)).BeginInit();
            this.layout.Panel1.SuspendLayout();
            this.layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGroupingControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockingManager)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1261, 28);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.openProjectToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.testToolStripMenuItem.Text = "New";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.NewProject_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Location = new System.Drawing.Point(0, 651);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip.Size = new System.Drawing.Size(1261, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // d3D11Control
            // 
            this.d3D11Control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.d3D11Control.BackColor = System.Drawing.Color.Black;
            this.d3D11Control.Location = new System.Drawing.Point(3, 3);
            this.d3D11Control.Name = "d3D11Control";
            this.d3D11Control.Size = new System.Drawing.Size(1, 1);
            this.d3D11Control.TabIndex = 2;
            this.d3D11Control.Load += new System.EventHandler(this.d3D11Control_Load);
            this.d3D11Control.Resize += new System.EventHandler(this.d3D11Control_Resize);
            // 
            // layout
            // 
            this.layout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.layout.BeforeTouchSize = 7;
            this.layout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layout.Location = new System.Drawing.Point(12, 65);
            this.layout.Name = "layout";
            // 
            // layout.Panel1
            // 
            this.layout.Panel1.Controls.Add(this.contextToolBar);
            this.layout.Panel1MinSize = 300;
            this.layout.Size = new System.Drawing.Size(1238, 580);
            this.layout.SplitterDistance = 300;
            this.layout.TabIndex = 5;
            this.layout.ThemeName = "None";
            // 
            // contextToolBar
            // 
            this.contextToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.contextToolBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextToolBar.Location = new System.Drawing.Point(0, 0);
            this.contextToolBar.Name = "contextToolBar";
            this.contextToolBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextToolBar.Size = new System.Drawing.Size(298, 25);
            this.contextToolBar.TabIndex = 0;
            this.contextToolBar.Text = "ContextToolBar";
            // 
            // gridGroupingControl1
            // 
            this.gridGroupingControl1.AlphaBlendSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.gridGroupingControl1.BackColor = System.Drawing.SystemColors.Window;
            this.gridGroupingControl1.Location = new System.Drawing.Point(17, 117);
            this.gridGroupingControl1.Name = "gridGroupingControl1";
            this.gridGroupingControl1.ShowCurrentCellBorderBehavior = Syncfusion.Windows.Forms.Grid.GridShowCurrentCellBorder.GrayWhenLostFocus;
            this.gridGroupingControl1.Size = new System.Drawing.Size(265, 283);
            this.gridGroupingControl1.TabIndex = 1;
            this.gridGroupingControl1.Text = "gridGroupingControl1";
            this.gridGroupingControl1.UseRightToLeftCompatibleTextBox = true;
            this.gridGroupingControl1.VersionInfo = "18.4460.0.30";
            // 
            // dockingManager
            // 
            this.dockingManager.AnimateAutoHiddenWindow = true;
            this.dockingManager.AutoHideEnabled = false;
            this.dockingManager.AutoHideTabForeColor = System.Drawing.Color.Empty;
            this.dockingManager.CloseTabOnMiddleClick = false;
            this.dockingManager.DockLayoutStream = ((System.IO.MemoryStream)(resources.GetObject("dockingManager.DockLayoutStream")));
            this.dockingManager.DockToFill = true;
            this.dockingManager.DragProviderStyle = Syncfusion.Windows.Forms.Tools.DragProviderStyle.Office2016Colorful;
            this.dockingManager.HostControl = this;
            this.dockingManager.MetroButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dockingManager.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.dockingManager.MetroSplitterBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.dockingManager.ReduceFlickeringInRtl = false;
            this.dockingManager.ThemeName = "Default";
            this.dockingManager.ThemeStyle.DockPreviewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dockingManager.VisualStyle = Syncfusion.Windows.Forms.VisualStyle.Default;
            this.dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Close, "CloseButton"));
            this.dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Pin, "PinButton"));
            this.dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Maximize, "MaximizeButton"));
            this.dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Restore, "RestoreButton"));
            this.dockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Menu, "MenuButton"));
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.openProjectToolStripMenuItem.Text = "Open Project";
            // 
            // MainWindowOld
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1261, 673);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(799, 349);
            this.Name = "MainWindowOld";
            this.Text = "SMEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CLOSING);
            this.Load += new System.EventHandler(this.PRELOAD);
            this.Shown += new System.EventHandler(this.internal_POSTLOAD);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.layout.Panel1.ResumeLayout(false);
            this.layout.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layout)).EndInit();
            this.layout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridGroupingControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockingManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        NewProjectWizard newProjectWizard;
        private void NewProject_Click(object sender, EventArgs e)
        {
            newProjectWizard = new NewProjectWizard();
            newProjectWizard.Show();
            newProjectWizard.Location = new System.Drawing.Point(Location.X + (Width / 2) - (newProjectWizard.Width/2), Location.Y + (Height / 2)- (newProjectWizard.Height / 2));
        }
        private void LoadProject_Click(object sender, EventArgs e)
        {
        }
        private void SaveProject_Click(object sender, EventArgs e)
        {
        }
        private void SaveAsProject_Click(object sender, EventArgs e)
        {
        }


        private void internal_POSTLOAD(object sender, EventArgs e)
        {
            this.Refresh();
            POSTLOAD();
        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        public SplitContainerAdv layout;
        public SlimDX.Windows.D3D11Control d3D11Control;
        public System.Windows.Forms.ToolStrip contextToolBar;
        private Syncfusion.Windows.Forms.Grid.Grouping.GridGroupingControl gridGroupingControl1;
        public DockingManager dockingManager;
        public ToolStripMenuItem windowToolStripMenuItem;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem openProjectToolStripMenuItem;
    }
}

