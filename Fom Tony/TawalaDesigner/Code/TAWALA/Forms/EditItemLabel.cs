// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// The transient AlternateLabel editing textbox used by the ItemViewBase class.
    /// </summary>
    internal class EditItemLabel : TextBox
    {
        private string oldLabel = string.Empty;

        public EditItemLabel()
        {
            AutoSize = true;
            TextAlign = HorizontalAlignment.Center;
            Multiline = false;
            WordWrap = false;
            BorderStyle = BorderStyle.FixedSingle;
            CausesValidation = false;
        }

        public new ItemViewBase Parent { get { return base.Parent as ItemViewBase; } }

        /// <summary>
        /// Parent ItemViewBase calls this to display the control to edit the label.
        /// </summary>
        public void Show(ItemViewBase parent)
        {
            Font = parent.Font;
            Text = oldLabel = parent.AlternateLabel;
            parent.SuspendLayout();
            parent.Controls.Add(this);
            sizeToText();
            parent.Invalidate(false);
            parent.ResumeLayout(true);
            SelectAll();
            Focus();
        }

        /// <summary>
        /// Everytime the text changes must resize
        /// </summary>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            sizeToText();
        }

        /// <summary>
        /// Watch for ESC and ENTER Keys
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (Parent != null)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    Text = isValid(oldLabel) ? oldLabel : string.Empty;
                    e.Handled = true;
                    Parent.Focus();
                }
                if (e.KeyCode == Keys.Enter && isValid(Text))
                {
                    Parent.Focus();
                    e.Handled = true;
                }
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Used to size based on text but that was distracting.
        /// </summary>
        private void sizeToText()
        {
            if (Parent != null)
            {
                Location = new Point(0, (Parent.Height - 1 - Height)/2);
                Width = ItemViewBase.LabelWidth;
            }
        }

        /// <summary>
        /// Check to see if the string contains a valid item label
        /// </summary>
        private bool isValid(string s)
        {
            return Parent.Parent.Form.ItemList.ValidAlternateLabel(Parent.FormItem, null, s);
        }

        /// <summary>
        /// Watch for WM_KILLFOCUS and update the parent's AlternateLabel
        /// and then remove ourself from the parent.
        /// </summary>
        /// <remarks>
        /// This isn't ideal but because of other focus gymnastics elsewhere in the app
        /// trying to use the Validation events would cause wierd crashes.
        /// </remarks>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_KILLFOCUS)
            {
                if (Parent != null && Parent.Controls.Contains(this))
                {
                    if (Text.Length == 0)
                    {
                        Parent.AlternateLabel = string.Empty;
                    }
                    else if (isValid(Text))
                    {
                        Parent.AlternateLabel = Text;
                    }
                    else
                    {
                        Parent.AlternateLabel = isValid(oldLabel) ? oldLabel : string.Empty;
                    }
                    Parent.Controls.Remove(this);
                }
            }
            base.WndProc(ref m);
        }
    }
}