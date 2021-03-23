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
    public class PropertiesPanel : DockableWindow
    {
        int tempFloat1 = 10, tempFloat2 = 100;

        TableLayoutPanel tlp;
        public PropertiesPanel(bool showOnDefault) : base("Properties", showOnDefault)
        {
            tlp = new TableLayoutPanel();
            tlp.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }
        public override void Init()
        {
            p.Controls.Add(tlp);
            tlp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tlp.Width = p.Width - 10;
            tlp.RowStyles.Clear();
            tlp.Location = new System.Drawing.Point(5, 10);

            tlp.ColumnCount = 2;
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75));

            tlp.RowCount = 1;

            SetProperties(new PropertyField[2]
            {
                new SliderProperty("TEST1", tempFloat1, 0, 20),
                new SliderProperty("TEST2", tempFloat2, 0, 200)
            });
        }
        public void SetProperties(PropertyField[] pfs)
        {
            foreach (PropertyField f in pfs)
            {
                tlp.RowCount++;
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));

                // property name
                Label label = new Label();
                label.Text = f.name;
                label.Margin = new Padding(0, 5, 0, 0);
                tlp.Controls.Add(label);

                if (f.type == PropertyField.Type.SliderProperty)
                {
                    SliderProperty sp = (SliderProperty)f;
                    tlp.Controls.Add(sp.sc);
                }
            }
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

        public SliderProperty(string _name, int _Initvalue, int _min, int _max)
        {
            type = Type.SliderProperty;
            name = _name;
            value = _Initvalue;
            min = _min;
            max = _max;

            tb = new TrackBarEx();
            tb.Style = TrackBarEx.Theme.Office2016Colorful;
            tb.Location = new System.Drawing.Point(-1, -3);
            tb.ValueChanged += new EventHandler(UpdateNumBox);
            tb.ShowButtons = false;
            tb.Minimum = min;
            tb.Maximum = max;
            tb.Value = value;
            tb.AutoSize = false;
            tb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            n = new NumericTextBox();
            n.Width = 31;
            n.Height = 11;
            n.Location = new System.Drawing.Point(n.Location.X - 2, -2);
            n.Text = _Initvalue.ToString();

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
            

            tb.Size = new System.Drawing.Size(sc.Panel1.Width + 4, 20);


            safeToUpdate = true;
        }

        bool safeToUpdate = false;
        private void UpdateNumBox(object sender, EventArgs e)
        {
            if(safeToUpdate) n.Text = tb.Value.ToString();
        }

        public int GetValue() { return value; }
    }
}
