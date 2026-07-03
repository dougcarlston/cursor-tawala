using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Processes;

namespace Tawala.Processes
{
    internal class StatementsListBox : ListBox
    {
        private bool selectionDisabled = true;

        public bool SelectionDisabled
        {
            get { return selectionDisabled; }
            set
            {
                selectionDisabled = value;
                if (selectionDisabled)
                {
                    Win32.ListBox_SetCurSel(this, -1);
                    selectedLinesStartIndex = -1;
                    selectedLinesEndIndex = -1;

                    if (Parent != null)
                    {
                        Parent.Invalidate(true);
                    }
                }
            }
        }

        public override int SelectedIndex
        {
            get
            {
                return selectionDisabled ? -1 : base.SelectedIndex;
            }
            set
            {
            }
        }

        public void SetSelectedIndex(int index)
        {
            base.SelectedIndex = index;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            selectionDisabled = false;
            base.OnSelectedIndexChanged(e);
            Parent.Invalidate(false);
        }

        private int selectedLinesStartIndex = -1;
        public int SelectedLinesStartIndex
        {
            set
            {
                selectedLinesStartIndex = value;
            }
            get
            {
                return selectedLinesStartIndex;
            }
        }

        public bool InSelectedLineGroup(int index)
        {
            return (index >= selectedLinesStartIndex && index <= selectedLinesEndIndex);
        }

        private int selectedLinesEndIndex = -1;
        public int SelectedLinesEndIndex
        {
            set
            {
                selectedLinesEndIndex = value;
            }
            get
            {
                return selectedLinesEndIndex;
            }
        }

        /// <summary>
        /// Index of insertion point for StatementsListBox
        /// </summary>
        /// <remarks>
        /// If -1, no insertion point exists.
        /// If >= number of lines in listbox, insertion point is
        /// after last listbox item.
        /// </remarks>
        public int InsertionIndex
        {
            set { insertionIndex = value; }
            get { return insertionIndex; }
        }

        private int insertionIndex = 0;

        /// <summary>
        /// A brush to paint secondary lines of a multi-line statement block in a "faded" version of the hightlight color
        /// </summary>
        private SolidBrush fadedHighlightBrush = new SolidBrush(Color.FromArgb((int)(SystemColors.Highlight.A * .5), SystemColors.Highlight.R, SystemColors.Highlight.G, SystemColors.Highlight.B));

        const int indentSize = 15;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            e.DrawBackground();

            if (indexIsValid(e.Index))
            {
                ProcessLine line = Items[e.Index] as ProcessLine;
                ProcessStatement statement = line.Statement as ProcessStatement;

                Color textColor = e.ForeColor;

                bool isComment = (statement != null && line is CommentLine);

                if (isComment)
                {
                    textColor = Color.Blue;
                }

                if (InSelectedLineGroup(e.Index))
                {
                    if (isComment)
                    {
                        textColor = SystemColors.HighlightText;
                    }

                    bool isFirstLine = (e.Index == SelectedLinesStartIndex);

                    if (!isFirstLine)
                    {
                        e.Graphics.FillRectangle(fadedHighlightBrush, e.Bounds);
                        textColor = SystemColors.HighlightText;
                    }
                }

                if (statement != null && !line.IsValid)
                {
                    textColor = Color.Red;
                }

                // set left edge of layout rectangle based on line's indent level
                Rectangle indentRect = e.Bounds;
                indentRect.X += (indentSize * line.IndentLevel);

                using (Brush brush = new SolidBrush(textColor))
                {
                    Font font = isComment ? new Font(e.Font, FontStyle.Italic) : e.Font;
                    e.Graphics.DrawString(line.ToString(), font, brush, indentRect, StringFormat.GenericDefault);
                }

                // when dragging a statement, draw a horizontal line indicating the insertion point
                if (insertionIndex == e.Index)
                {
                    if (!InSelectedLineGroup(e.Index))
                    {
                        using (Pen p = new Pen(SystemColors.GrayText, 3.0f))
                        {
                            e.Graphics.DrawLine(p, e.Bounds.Left + indentSize, e.Bounds.Top, e.Bounds.Right - indentSize, e.Bounds.Top);
                        }
                    }
                }
                else if (insertionIndex == Items.Count && e.Index == Items.Count - 1)
                {
                    using (Pen p = new Pen(SystemColors.GrayText, 3.0f))
                    {
                        e.Graphics.DrawLine(p, e.Bounds.Left + indentSize, e.Bounds.Bottom, e.Bounds.Right - indentSize, e.Bounds.Bottom);
                    }
                }

            }
        }

        private bool indexIsValid(int index)
        {
            return index >= 0 && Items.Count > 0;
        }

        private int getItemWidth(Graphics g, int index)
        {
            if (indexIsValid(index))
            {
                ProcessLine line = Items[index] as ProcessLine;
                ProcessStatement statement = line.Statement as ProcessStatement;

                bool isComment = (statement != null && line is CommentLine);

                Font font = isComment ? new Font(Font, FontStyle.Italic) : Font;
                return Convert.ToInt32(g.MeasureString(line.ToString(), font).Width) + indentSize * line.IndentLevel + 10;
            }

            return -1;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Project.Events.ProcessConnectedToForm += events_processFormConnectionChanged;
            Project.Events.ProcessDisconnectedFromForm += events_processFormConnectionChanged;

            Project.Events.FormChanged += events_FormChanged;
            Project.Events.FormItemChanged += events_FormItemChanged;
            Application.Idle += Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            if (Parent != null && horizontalScrollbarFlag)
            {
                horizontalScrollbarFlag = false;
                setupHorizontalScrollbar();
            }
        }

        private bool horizontalScrollbarFlag = true;

        protected override void SetItemCore(int index, object value)
        {
            horizontalScrollbarFlag = true;
            base.SetItemCore(index, value);
        }

        protected override void SetItemsCore(IList value)
        {
            horizontalScrollbarFlag = true;
            base.SetItemsCore(value);
        }

        private void setupHorizontalScrollbar()
        {
            int maxWidth = 0;
            using (Graphics g = CreateGraphics())
            {
                for (int i = 0; i < Items.Count; ++i)
                {
                    int w = getItemWidth(g, i);

                    if (w > maxWidth)
                    {
                        maxWidth = w;
                    }
                }
            }

            HorizontalScrollbar = true;
            HorizontalExtent = maxWidth;
        }

        private void events_processFormConnectionChanged(object sender, ProcessConnectionArgs e)
        {
            Invalidate();
        }

        private void events_FormChanged(object sender, ComponentEventArgs e)
        {
            Invalidate();
        }

        private void events_FormItemChanged(object sender, FormItemEventArgs e)
        {
            Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.LB_SETCURSEL)
            {
                if (selectionDisabled)
                {
                    m.WParam = (IntPtr)(-1);
                }
            }

            base.WndProc(ref m);

            if (m.Msg == Win32.WM_VSCROLL)
            {
                Rectangle r = Parent.ClientRectangle;
                r.Width = 20;
                Parent.Invalidate(r);
            }
        }


    }
}