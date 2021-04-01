using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMEditor.Editor.Layout
{
    public enum SelectedType { None, Tool, Object }
    public enum DecimalPlaceCount { None, Single, Double }
    public class PropertiesPanel : DockableWindow
    {
        int tempFloat1 = 10, tempFloat2 = 100;

        TableLayoutPanel tlp;
        PropertyGrid pg;
        SplitContainer header;
        PictureBox selectedTypeImage;
        Label selectedTypeText;
        public PropertiesPanel(bool showOnDefault) : base("Properties", showOnDefault)
        {
            tlp = new TableLayoutPanel();
            tlp.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tlp.BackColor = System.Drawing.Color.Beige;

            header = new SplitContainer();
            selectedTypeImage = new PictureBox();
            selectedTypeText = new Label();
            header.Panel1.Controls.Add(selectedTypeImage);
            header.Panel2.Controls.Add(selectedTypeText);
            header.Panel1.BackColor = System.Drawing.Color.Beige;
            header.Panel2.BackColor = System.Drawing.Color.Beige;

            ClearProperties();
            Program.mainWindow.dockingManager.SetControlMinimumSize(p, new Size(250, 0));
            Program.mainWindow.dockingManager.SetControlSize(p, new Size(260, 0));
        }
        public void Init1()
        {

            p.Controls.Add(tlp);
            p.Controls.Add(header);

            header.Width = p.Width - 10;
            header.Height = 25;
            header.BorderStyle = BorderStyle.FixedSingle;
            header.Location = new System.Drawing.Point(5, 5);
            header.SplitterDistance = 25;
            header.IsSplitterFixed = true;
            header.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;

            selectedTypeImage.Size = new System.Drawing.Size(25, 25);
            selectedTypeImage.Location = new System.Drawing.Point(2, 2);

            selectedTypeText.Size = new System.Drawing.Size(header.Panel2.Width, 25);
            selectedTypeText.Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Regular);
            selectedTypeText.Location = new System.Drawing.Point(1, 4);


            tlp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlp.Width = p.Width - 10;
            tlp.Location = new System.Drawing.Point(5, 35);

            tlp.ColumnCount = 2;
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));

            tlp.RowStyles.Clear();
            tlp.Controls.Clear();
            tlp.RowCount = 2;

            AddProperty(SelectedType.Tool, "TestTool", new PropertyField[2]
            {
                new SliderProperty("TEST1", tempFloat1, 0, 20),
                new SliderProperty("TEST2", tempFloat2, 0, 200)
            });
        }
        public override void Resize()
        {
            // this is just so that all of the properties' controls' update visually upon resize.
            tlp.Invalidate();
            foreach(Control c in tlp.Controls)
            {
                c.Invalidate();
                foreach (Control c2 in c.Controls)
                {
                    c2.Invalidate();
                    foreach (Control c3 in c2.Controls)
                    {
                        c3.Invalidate();
                        foreach(Control c4 in c3.Controls)
                        {
                            c4.Invalidate();
                        }
                    }
                }
            }
        }
        public void AddProperty(SelectedType selectedType, string selectedName, PropertyField[] pfs)
        {
            ClearProperties();

            // set header
            if (selectedType == SelectedType.None) selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/nothing.png");
            if (selectedType == SelectedType.Tool) selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/tool.png");
            if (selectedType == SelectedType.Object) selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/object.png");
            selectedTypeText.Text = selectedName;

            // add properties
            int height = 0;
            foreach (PropertyField f in pfs)
            {
                tlp.RowCount++;

                // property name
                Label label = new Label();
                label.Text = f.name;
                label.Margin = new Padding(1, 4, 0, 0);
                tlp.Controls.Add(label);
                
                // specific types
                if (f.type == PropertyField.Type.SliderProperty)
                {
                    SliderProperty sp = (SliderProperty)f;
                    tlp.Controls.Add(sp.sc);
                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
                    height += 23;
                }
                if (f.type == PropertyField.Type.EnumProperty)
                {
                    EnumProperty ep = (EnumProperty)f;
                    tlp.Controls.Add(ep.sc);
                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
                    height += 23;
                }
            }

            tlp.Height = height + 1;
        }
        public void ClearProperties()
        {
            selectedTypeText.Text = "Nothing Selected";
            selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/nothing.png");

            tlp.Controls.Clear();
            tlp.RowStyles.Clear();
            tlp.Height = 0;
        }



        List<Control> controls = new List<Control>();
        TableLayoutPanel currentGroup;
        public override void Init()
        {
            currentGroup = new TableLayoutPanel();
            NewGroup();
            controls.Add(currentGroup);
        }

        public void NewGroup()
        {
            if (!controls.Contains(currentGroup)) controls.Add(currentGroup);
            currentGroup = new TableLayoutPanel();

            currentGroup.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            //currentGroup.BackColor = System.Drawing.Color.Beige;
            currentGroup.BackColor = Color.FromArgb(230, 230, 230);

            currentGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            currentGroup.Width = p.Width - 10;
            currentGroup.Height = 1;
            currentGroup.Location = new System.Drawing.Point(5, yTracker);

            currentGroup.ColumnCount = 2;
            currentGroup.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));

            currentGroup.RowStyles.Clear();
            currentGroup.Controls.Clear();
            currentGroup.RowCount = 2;

        }
        public void AddProperty(PropertyField f)
        {
            tlp.RowCount++;

            // property name
            Label label = new Label();
            label.Text = f.name;
            label.Margin = new Padding(1, 4, 0, 0);
            currentGroup.Controls.Add(label);

            // specific types
            if (f.type == PropertyField.Type.SliderProperty)
            {
                SliderProperty sp = (SliderProperty)f;
                currentGroup.Controls.Add(sp.sc);
                currentGroup.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
                currentGroup.Height += 23;
            }
            if (f.type == PropertyField.Type.EnumProperty)
            {
                EnumProperty ep = (EnumProperty)f;
                currentGroup.Controls.Add(ep.sc);
                currentGroup.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
                currentGroup.Height += 23;
            }
        }
        public void AddLabel(string s)
        {
            Label label = new Label();
            label.Text = s;
            label.Margin = new Padding(1, 4, 0, 0);
            currentGroup.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            currentGroup.Height += 23;
            currentGroup.Controls.Add(label);
            currentGroup.Controls.Add(new Label()); //to pad the 2nd column.
        }

        public void AddTitle(Image i, string text)
        {
            SplitContainer header = new SplitContainer();
            Label selectedTypeText = new Label();
            PictureBox selectedTypeImage = new PictureBox();

            header.Width = p.Width - 10;
            header.Height = 25;
            header.BorderStyle = BorderStyle.FixedSingle;
            header.Location = new System.Drawing.Point(5, 5);
            header.SplitterDistance = 25;
            header.IsSplitterFixed = true;
            header.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            header.FixedPanel = FixedPanel.Panel1;
            header.Panel1.Controls.Add(selectedTypeImage);
            header.Panel2.Controls.Add(selectedTypeText);

            selectedTypeImage.Size = new System.Drawing.Size(25, 25);
            selectedTypeImage.Location = new System.Drawing.Point(2, 2);
            selectedTypeImage.Image = i;

            selectedTypeText.Size = new System.Drawing.Size(header.Panel2.Width, 25);
            selectedTypeText.Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Regular);
            selectedTypeText.Location = new System.Drawing.Point(1, 4);
            selectedTypeText.Text = text;

            controls.Add(header);
        }

        int yTracker;
        public void Present()
        {
            if (!controls.Contains(currentGroup)) controls.Add(currentGroup);
            yTracker = 5;

            foreach (Control c in controls.Distinct())
            {
                c.Width = p.Width - 10;
                c.Location = new Point(5, yTracker);
                p.Controls.Add(c);
                yTracker += c.Height + 10;
            }
        }
        public void Clear()
        {
            foreach(Control c in controls)
            {
                p.Controls.Remove(c);
            }
            NewGroup();
            controls.Clear();
        }
    }

    public class PropertyField
    {
        public enum Type { SliderProperty, EnumProperty }
        public Type type;
        public string name;
    }
    public class SliderProperty : PropertyField
    {
        public SplitContainer sc;

        private TrackBarEx tb;
        private NumericTextBox n;
        private int value, min, max;
        private float divideBy;

        public SliderProperty(string _name, int _initValue, int _min, int _max, DecimalPlaceCount dpc = DecimalPlaceCount.None)
        {
            type = Type.SliderProperty;
            name = _name;
            value = _initValue;
            min = _min;
            max = _max;
            switch(dpc)
            {
                case DecimalPlaceCount.None:
                    divideBy = 1;
                    break;
                case DecimalPlaceCount.Single:
                    divideBy = 10;
                    break;
                case DecimalPlaceCount.Double:
                    divideBy = 100;
                    break;
            }

            tb = new TrackBarEx();
            tb.Style = TrackBarEx.Theme.Office2016Colorful;
            tb.Location = new System.Drawing.Point(-2, -3);
            tb.ValueChanged += new EventHandler(UpdateNumBox);
            //tb.ShowButtons = false;
            tb.Minimum = min;
            tb.Maximum = max;
            tb.Value = value;
            tb.AutoSize = false;
            //tb.BackColor = Color.FromArgb(45, 45, 48);
            tb.SliderSize = new System.Drawing.Size(tb.SliderSize.Width+5, tb.SliderSize.Height);
            tb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            n = new NumericTextBox();
            n.Width = 31;
            n.Height = 11;
            n.ReadOnly = true;
            n.Location = new System.Drawing.Point(n.Location.X - 2, -2);
            n.Text = (_initValue / divideBy).ToString();
            n.NumberDecimalDigits = 1;

            // divider for trackbar and numerictextbox.
            sc = new SplitContainer();
            sc.Orientation = Orientation.Vertical;
            sc.Panel1.Controls.Add(tb);
            sc.Panel2.Controls.Add(n);
            sc.BorderStyle = BorderStyle.FixedSingle;
            sc.SplitterDistance = sc.Width - 31;
            sc.FixedPanel = FixedPanel.Panel2;
            sc.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            sc.IsSplitterFixed = true;
            sc.Size = new System.Drawing.Size(sc.Width-15, 17);
            

            tb.Size = new System.Drawing.Size(sc.Panel1.Width, 20);


            safeToUpdate = true;
        }

        bool safeToUpdate = false;
        private void UpdateNumBox(object sender, EventArgs e)
        {
            if (safeToUpdate) n.Text = (tb.Value / divideBy).ToString();
        }

        public float GetValue() { return tb.Value / divideBy; }
    }
    public class EnumProperty : PropertyField
    {
        public SplitContainer sc;
        public ComboBox cb;
        public EnumProperty(string _name, string[] options, int defaultOptionIndex)
        {
            type = Type.EnumProperty;
            name = _name;

            sc = new SplitContainer();
            sc.SplitterDistance = 1000;
            sc.Panel2Collapsed = true;
            sc.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            sc.BorderStyle = BorderStyle.FixedSingle;

            cb = new ComboBox();
            cb.Height = 20;
            cb.Location = new System.Drawing.Point(-2, -4);
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.FlatStyle = FlatStyle.Flat;
            cb.Width = sc.Width + 3;
            cb.BackColor = Color.FromArgb(241, 241, 241);
            //cb.st
            cb.Font = new System.Drawing.Font(cb.Font.FontFamily, 7.5f);
            cb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            sc.Panel1.Controls.Add(cb);

            foreach (string s in options)
            {
                cb.Items.Add(s);
            }
            cb.SelectedIndex = defaultOptionIndex;
        }
        public string GetActiveName() { return cb.SelectedText; }
        public int GetActiveIndex() { return cb.SelectedIndex; }
    }
}
