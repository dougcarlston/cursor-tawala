// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    internal partial class SingleLineEditPanel : UserControl
    {
        private readonly Control edit;

        public SingleLineEditPanel(IParameterControl pc)
        {
            edit = pc as Control;
            InitializeComponent();

            edit.Padding = new Padding(0);
            edit.Margin = new Padding(0);
            edit.TabStop = true;
            edit.TabIndex = 1;
            edit.MaximumSize = edit.MinimumSize = new Size(200, 0);
            edit.Left = labelName.Right;
            Controls.Add(edit);
            Controls.SetChildIndex(edit, 1);

            TabStop = true;
            base.Anchor = AnchorStyles.None;

            IParameterInfo info = ControlManager.LookupParameterInfo(edit as IParameterControl);
            labelName.Text = info.Name + ":  ";
        }

        public int LabelWidth
        {
            get
            {
                return labelName.Width;
            }
        }

        public void OptimizeLayout(int width)
        {
            labelName.AutoSize = false;
            labelName.MinimumSize = new Size(width, ClientSize.Height);
            labelName.MaximumSize = labelName.MinimumSize;
            if (edit is ComboBox)
            {
                sizeComboBox();
            }
            else
            {
                edit.MinimumSize = new Size(ClientSize.Width - width - 14, 0);
            }
            edit.Left = labelName.Right;
        }

        protected override void OnEnter(EventArgs e)
        {
            edit.Focus();
            base.OnEnter(e);
        }

        private void sizeComboBox()
        {
            var comboBox = edit as ComboBox;
            if (comboBox == null)
            {
                return;
            }

            float minWidth = 50.0f;

            using (Graphics g = comboBox.CreateGraphics())
            {
                g.PageUnit = GraphicsUnit.Pixel;
                for (int i = 0; i < comboBox.Items.Count; ++i)
                {
                    string text = comboBox.GetItemText(comboBox.Items[i]);
                    float width = g.MeasureString(text, comboBox.Font, ClientSize.Width - labelName.Width - 14).Width;
                    minWidth = Math.Max(width, minWidth);
                }
            }

            comboBox.MinimumSize = new Size((int)minWidth + 20, 0);
            comboBox.MaximumSize = comboBox.MinimumSize;
        }
    }
}