using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using System;
using System.Collections.Generic;
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
        }
        public override void Init()
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

            SetProperties(SelectedType.Tool, "TestTool", new PropertyField[2]
            {
                new SliderProperty("TEST1", tempFloat1, 0, 20),
                new SliderProperty("TEST2", tempFloat2, 0, 200)
            });
        }
        public void SetProperties(SelectedType selectedType, string selectedName, PropertyField[] pfs)
        {
            ClearProperties();

            // set header
            if (selectedType == SelectedType.None) selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/nothing.png");
            if (selectedType == SelectedType.Tool) selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/tool.png");
            if (selectedType == SelectedType.Object) selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/object.png");
            selectedTypeText.Text = selectedName;

            // add properties
            foreach (PropertyField f in pfs)
            {
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
                tlp.RowCount++;

                // property name
                Label label = new Label();
                label.Text = f.name;
                label.Margin = new Padding(1, 4, 0, 0);
                tlp.Controls.Add(label);

                if (f.type == PropertyField.Type.SliderProperty)
                {
                    SliderProperty sp = (SliderProperty)f;
                    tlp.Controls.Add(sp.sc);
                }
            }

            tlp.Height = pfs.Length * 23 + 1;
        }
        public void ClearProperties()
        {
            selectedTypeText.Text = "Nothing Selected";
            selectedTypeImage.Image = System.Drawing.Image.FromFile("Thumbs/nothing.png");

            tlp.Controls.Clear();
            tlp.RowStyles.Clear();
            tlp.Height = 0;
        }
    }

    public class PropertyField
    {
        public enum Type { SliderProperty }
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
            tb.SliderSize = new System.Drawing.Size(tb.SliderSize.Width+5, tb.SliderSize.Height);
            tb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            n = new NumericTextBox();
            n.Width = 31;
            n.Height = 11;
            n.ReadOnly = true;
            n.Location = new System.Drawing.Point(n.Location.X - 2, -2);
            n.Text = _initValue.ToString();
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
}
