// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.Common;

namespace Tawala.Forms
{
    public class McqItemTextEditor : ItemTextEditor
    {
        private bool leftArrowKeyPressed;
        private bool updatingLabel;

        public string SelectedText { get { return Selection.Text.Replace("\r\n", "\n"); } set { Selection.Text = value; } }

        public int SelectionStart { get { return Selection.Start; } set { Selection.Start = value; } }

        public int SelectionLength { get { return Selection.Length; } set { Selection.Length = value; } }

        protected bool SelectionProtected { get { return false; } set { } }

        protected Color SelectionColor
        {
            get
            {
                Color color = Color.Blue;
                //				Selection.GetFontColor(out color);
                return color;
            }
            set
            {
                //				Selection.SetFontColor(value);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                insertNewLabel();
                e.Handled = true;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (handleDelete())
                {
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (handleBackspace())
                {
                    //e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                leftArrowKeyPressed = true;
            }
        }

        public override void OnInputPositionChanged(EventArgs e)
        {
            //if (selectionInLabel() && !updatingLabel)
            //{
            //    int labelStart = getCurrentLabelStart();
            //    if (leftArrowKeyPressed)
            //    {
            //        SelectionStart = labelStart;
            //        leftArrowKeyPressed = false;
            //    }
            //    else
            //    {
            //        SelectionStart = labelStart + 7;
            //    }
            //}

            if (selectionEntirelyInLabel() && !updatingLabel)
            {
                int labelStart = getCurrentLabelStart();
                if (leftArrowKeyPressed)
                {
                    SelectionStart = labelStart;
                    leftArrowKeyPressed = false;
                }
                else
                {
                    SelectionStart = labelStart + 7;
                }

                SelectionLength = 0;
            }

            base.OnInputPositionChanged(e);
        }

        private void insertNewLabel()
        {
            if (selectionIncludesNewline())
            {
                SelectionLength--;
            }

            SelectedText = LabelLine(0);

            refreshLabels();
        }

        private bool selectionIncludesNewline()
        {
            return SelectedText.EndsWith("\n");
        }

        public string LabelLine(int index)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("\n{0}) ", Label(index));
            return sb.ToString();
        }

        public string Label(int index)
        {
            return new AlphaLabel(index).ToString().PadLeft(4);
        }

        public void RefreshLabels()
        {
            refreshLabels();
        }

        /// <summary>
        /// Re-label all of the choices.  A choice label always starts with "\n".
        /// </summary>
        private void refreshLabels()
        {
            int selStart = SelectionStart;
            int selLength = SelectionLength;
            Select(0, 0);

            int index = GetCleanText().IndexOf("\n");
            int line = 0;

            while (index >= 0)
            {
                updateLabelAlpha(index, line);
                //setLabelColor(index);
                //protectLabel(index);
                moveInsertionPointAfterLabel(index);

                line++;
                index = GetCleanText().IndexOf("\n", index + 1);
            }

            Select(selStart, selLength);
        }

        private void moveInsertionPointAfterLabel(int index)
        {
            Select(index + 7, 0);
            SelectionProtected = false;
            SelectionColor = ForeColor;
        }

        private void protectLabel(int index)
        {
            Select(index, 7);
            SelectionProtected = true;
        }

        private void setLabelColor(int index)
        {
            updatingLabel = true;
            Select(index + 7, 5);
            SelectionProtected = false;
            SelectionColor = Color.DarkBlue;
            updatingLabel = false;
        }

        private void updateLabelAlpha(int index, int line)
        {
            updatingLabel = true;
            Select(index + 1, 4);
            SelectionProtected = false;
            SelectedText = Label(line);
            updatingLabel = false;
        }

        //public void Select(int start, int length)
        //{
        //    Selection.Start = start;
        //    Selection.Length = length;
        //}

        //private TXTextControl.Selection Selection
        //{
        //    get
        //    {
        //        return base.TxSelection;
        //    }
        //}

        public string GetCleanText()
        {
            return GetText().Replace("\r\n", "\n");
        }

        private bool handleBackspace()
        {
            int firstLabelStart = getFirstLabelStart();
            int labelStart = getCurrentLabelStart();
            int nextLabelStart = getNextLabelStart(labelStart);

            if (atBeginningOfChoiceText(labelStart))
            {
                if (!firstLabelIsOnlyLabel())
                {
                    deleteLabel(labelStart);
                }

                return true;
            }

            // REVISIT: Not ideal to refresh on every keystroke, but the actual deletion
            //			is handled upstream in DerivedTxControl.OnKeyDown()
            refreshLabels();

            return false;
        }

        private bool handleDelete()
        {
            int firstLabelStart = getFirstLabelStart();
            int labelStart = getCurrentLabelStart();
            int nextLabelStart = getNextLabelStart(labelStart);

            if (atEndOfChoiceText(nextLabelStart))
            {
                if (!nextLabelIsOnlyLabel(firstLabelStart))
                {
                    deleteLabel(nextLabelStart);
                }

                return true;
            }

            // REVISIT: Not ideal to refresh on every keystroke, but the actual deletion
            //			is handled upstream in DerivedTxControl.OnKeyDown()
            refreshLabels();

            return false;
        }

        private void deleteLabel(int labelStart)
        {
            Select(labelStart, 7);
            Selection.Text = "";

            refreshLabels();
        }

        private int getFirstLabelStart()
        {
            return GetCleanText().IndexOf('\n');
        }

        private int getCurrentLabelStart()
        {
            return SelectionStart <= 0 ? -1 : GetCleanText().LastIndexOf('\n', SelectionStart - 1, SelectionStart);
        }

        private int getNextLabelStart(int index)
        {
            string text = GetCleanText();
            return index + 1 >= text.Length ? -1 : text.IndexOf('\n', index + 1);
        }

        private bool atBeginningOfChoiceText(int labelStart)
        {
            return (labelStart + 7 == SelectionStart);
        }

        private bool atEndOfChoiceText(int nextLabelStart)
        {
            return nextLabelStart > 0 && SelectionStart == nextLabelStart;
        }

        private bool firstLabelIsOnlyLabel()
        {
            int labelStart = getCurrentLabelStart();
            return (getFirstLabelStart() == labelStart && getNextLabelStart(labelStart) < 0);
        }

        private bool nextLabelIsOnlyLabel(int labelStart)
        {
            return (isBeforeFirstLabel(labelStart) && getNextLabelStart(labelStart) < 0);
        }

        private bool isBeforeFirstLabel(int firstLabelStart)
        {
            return (SelectionStart < firstLabelStart + 7);
        }

        private bool selectionInLabel()
        {
            int labelStart = getCurrentLabelStart();
            if (labelStart == -1)
            {
                return false;
            }
            return (SelectionStart - labelStart <= 6);
        }

        private bool selectionEntirelyInLabel()
        {
            int labelStart = getCurrentLabelStart();
            if (labelStart == -1)
            {
                return false;
            }

            int labelEnd = labelStart + 7;
            int selectionEnd = SelectionStart + SelectionLength;

            return SelectionStart >= labelStart && selectionEnd <= labelEnd;

            //return (SelectionStart - labelStart <= 6);
        }
    }
}