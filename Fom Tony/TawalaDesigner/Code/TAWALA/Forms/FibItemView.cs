// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tawala.Functions.Controls;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// A Fill-in-the-Blank Form Item UI control
    /// </summary>
    public partial class FibItemView : ItemViewBase
    {
        private static readonly FibOptions optionsPanel = new FibOptions();
        private InternalBlankList internalBlanks = new InternalBlankList();

        public FibItemView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            itemTextEditor.Dock = DockStyle.None;
        }

        public FibItem ProjectFibItem { get { return FormItem as FibItem; } }

        public Blank CurrentBlank { get { return internalBlanks.Current >= 0 && FormItem != null ? ProjectFibItem.BlankList[internalBlanks.Current] : null; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string rtfString = ProjectFibItem.Rtf;
            bool initialText = rtfString.Contains("~@!~");
            int start = 0, end = 0;
            if (initialText)
            {
                start = rtfString.IndexOf("~@!~");
                end = rtfString.LastIndexOf("~@!~");
                if (start < 0 || start == end)
                {
                    initialText = false;
                }
                else
                {
                    itemTextEditor.SetRTF(rtfString);
                    string rawText = itemTextEditor.GetText();
                    start = rawText.IndexOf("~@!~");
                    end = rawText.LastIndexOf("~@!~");
                    rtfString = Regex.Replace(rtfString, "~@!~", "");
                }
            }

            itemTextEditor.SetRTF(rtfString);

            if (initialText)
            {
                AllowSetFocus = true;
                itemTextEditor.Select(start, end - start - 4);
                itemTextEditor.RetainSelectionOnEntry = true;
            }

            copyBlankAttributesFromProject();

            itemTextEditor.ForceOnChanged(); // to make sure control layout encompasses all text

            // don't hook these events until after the above is done
            itemTextEditor.GotFocus += itemTextEditor_GotFocus;
            itemTextEditor.InputPositionChanged += itemTextEditor_InputPositionChanged;
            itemTextEditor.Changed += itemTextEditor_Changed;
            Project.Events.SynchronizeProject += events_SynchronizeProject;
        }

        private void events_SynchronizeProject(object sender, EventArgs e)
        {
            if (ContainsFocus)
            {
                optionsPanel.ForceAlternateLabelUpdate();
                optionsPanel.ForceHeightUpdate();

                synchronizeProjectFibItem();
            }
        }

        private void synchronizeProjectFibItem()
        {
            ProjectFibItem.Rtf = itemTextEditor.GetRTF();
            copyBlankAttributesToProject();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            releaseOptionsPanel();

            Project.Events.SynchronizeProject -= events_SynchronizeProject;
            base.OnHandleDestroyed(e);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size rtb = itemTextEditor.GetPreferredSize(proposedSize);
            var s = new Size(proposedSize.Width, rtb.Height + 1);

            if (optionsPanel.OwningFibItemView == this)
            {
                s.Height += optionsPanel.Height;
            }

            return s;
        }

        internal void EditValidationFunction()
        {
            if (internalBlanks.Current != -1)
            {
                if (CurrentBlank.ValidationFunction != null)
                {
                    ConfigureFunctionDialog.Presenter.EditFunction(CurrentBlank.ValidationFunction, handleValidationFunctionConfigured);
                }
            }
        }

        private void handleValidationFunctionConfigured(object sender, FunctionConfiguredEventArgs args)
        {
            if (!args.Canceled)
            {
                if (internalBlanks.Current != -1)
                {
                    internalBlanks[internalBlanks.Current].ValidationFunction = args.EditedInstance;
                    ProjectFibItem.BlankList[internalBlanks.Current].ValidationFunction = args.EditedInstance;
                    optionsPanel.ValidationFunction = args.EditedInstance;
                }
            }
        }

        private void copyBlankAttributesFromProject()
        {
            generateNewBlankList();
            Debug.Assert(internalBlanks.Count == ProjectFibItem.BlankList.Count);
            for (int i = 0, j = 0; i < ProjectFibItem.BlankList.Count; i++, j++)
            {
                if (j < internalBlanks.Count)
                {
                    internalBlanks[j].AlternateLabel = ProjectFibItem.BlankList[i].AlternateLabel;
                    internalBlanks[j].Required = ProjectFibItem.BlankList[i].Required;
                    internalBlanks[j].Height = ProjectFibItem.BlankList[i].Height;
                    internalBlanks[j].ValidationFunction = ProjectFibItem.BlankList[i].ValidationFunction;
                }
            }
        }

        private void copyBlankAttributesToProject()
        {
            Debug.Assert(internalBlanks.Count == ProjectFibItem.BlankList.Count);
            for (int i = 0, j = 0; i < internalBlanks.Count; i++, j++)
            {
                if (j < ProjectFibItem.BlankList.Count)
                {
                    if (!ProjectFibItem.BlankList[j].AlternateLabel.Equals(internalBlanks[i].AlternateLabel))
                    {
                        ProjectFibItem.BlankList[j].AlternateLabel = internalBlanks[i].AlternateLabel;
                    }

                    ProjectFibItem.BlankList[j].Required = internalBlanks[i].Required;
                    ProjectFibItem.BlankList[j].Height = internalBlanks[i].Height;
                    ProjectFibItem.BlankList[j].ValidationFunction = internalBlanks[i].ValidationFunction;
                }
            }
        }

        protected override void OnValidated(EventArgs e)
        {
            synchronizeProjectFibItem();

            releaseOptionsPanel();
            base.OnValidated(e);
        }

        private void releaseOptionsPanel()
        {
            if (optionsPanel.OwningFibItemView == this)
            {
                optionsPanel.OwningFibItemView = null; // hide the options panel
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent == null)
            {
                if (optionsPanel.OwningFibItemView == this)
                {
                    optionsPanel.OwningFibItemView = null;
                }
            }

            base.OnParentChanged(e);
        }

        private void enableOptionsPanel(bool b)
        {
            if (optionsPanel.OwningFibItemView == this)
            {
                optionsPanel.Enabled = b;
            }
        }

        private void generateNewBlankList()
        {
            internalBlanks = new InternalBlankList();

            string rawText = stripCarriageReturns(itemTextEditor.GetText());
            MatchCollection matches = Regex.Matches(rawText, "_+");

            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    internalBlanks.Add(new InternalBlank(m.Index, m.Index + m.Length));
                }
            }
        }

        private static string stripCarriageReturns(string text)
        {
            return text.Replace("\r\n", "\n");
        }

        /// <summary>
        /// compares the current list of blanks with the previous one
        /// and transfers the attributes to the appropriate blanks in the new list
        /// </summary>
        /// 
        /// <remarks>
        /// A new list of blanks is generated every time a change occurs in the FibItemView.
        /// The change could be an edit, or a change of selection. The text is scanned for
        /// underscores and a new list of blanks is generated. This method then determines
        /// which new blanks are really "the same" as the previous blanks, and transfers
        /// the attributes.
        /// 
        /// The logic behind this mechanism is as follows:
        /// 
        ///	 1.	If the number of blanks is unchanged, then the new blanks are considered
        ///		to be the same as the old ones. (The only contorversial case I could think
        ///		of is if an old blank is entirely selected and is replaced by typing a
        ///		single underscore character. I'm letting this slip through as "the same"
        ///		blank. So shoot me.)
        /// 
        ///	 2.	If there are fewer new blanks than previously:
        ///		 a.	If there was no selection previously the cursor was in or adjacent
        ///			to a single blank. That is the blank that got deleted, and so its
        ///			attributes get "dropped" (don't get transeferred to a new blank).
        ///		 b. Similarly, if there was a selection, but it was within a single blank,
        ///			or contained a single blank exactly, that is the blank whose attributes
        ///			get dropped.
        ///		 c. If the selection encompassed more than one blank (meaning a single
        ///			blank plus other text, or multiple blanks) then the blanks that
        ///			were within the selection were deleted, and their attributes get
        ///			dropped.
        ///		 d. Blanks that are partially selected, and therefore combined into a
        ///			single blank, inherit the attributes of the leftmost blank.
        /// 
        ///  3. If there are more new blanks than previously the blank immediately to
        ///		the right of the cursor is the "new" one because an intervening character
        ///		has just been inserted to sever an old blank. The new blank is skipped
        ///		over as attributed are transferred.
        /// </remarks>
        private void updateBlankList()
        {
            InternalBlankList previousInternalBlanks = internalBlanks;

            generateNewBlankList();

            if (numberOfBlanksHasChanged(previousInternalBlanks))
            {
                if (oneOrMoreBlanksWereDeleted(previousInternalBlanks))
                {
                    copyAttributesToBlankListAfterDeletionOfBlank(previousInternalBlanks);
                }
                else
                {
                    copyAttributesToBlankListAfterAdditionOfBlank(previousInternalBlanks);
                }

                synchronizeProjectFibItem();
            }
            else
            {
                copyAttributesToBlankList(previousInternalBlanks);
            }

            updateInsertionIndexForNewBlankList();
        }

        private bool numberOfBlanksHasChanged(InternalBlankList previousInternalBlanks)
        {
            return internalBlanks.Count != previousInternalBlanks.Count;
        }

        private void copyAttributesToBlankList(InternalBlankList previousInternalBlanks)
        {
            for (int i = 0; i < internalBlanks.Count; i++)
            {
                internalBlanks[i].AlternateLabel = previousInternalBlanks[i].AlternateLabel;
                internalBlanks[i].Required = previousInternalBlanks[i].Required;
                internalBlanks[i].Height = previousInternalBlanks[i].Height;
                internalBlanks[i].ValidationFunction = previousInternalBlanks[i].ValidationFunction;
            }
        }

        private bool oneOrMoreBlanksWereDeleted(InternalBlankList previousInternalBlanks)
        {
            return internalBlanks.Count < previousInternalBlanks.Count;
        }

        private void copyAttributesToBlankListAfterDeletionOfBlank(InternalBlankList previousInternalBlanks)
        {
            if (singleBlankOnlyWasDeleted(previousInternalBlanks))
            {
                copyAttributesExceptFromSingleDeletedBlank(previousInternalBlanks);
            }
            else
            {
                copyRemainingAttributesAfterCompoundDeletion(previousInternalBlanks);
            }
        }

        private static bool singleBlankOnlyWasDeleted(InternalBlankList previousInternalBlanks)
        {
            return previousInternalBlanks.Current != -1;
        }

        private void copyAttributesExceptFromSingleDeletedBlank(InternalBlankList previousInternalBlanks)
        {
            for (int i = 0, j = 0; i < internalBlanks.Count; i++, j++)
            {
                // skip the blank that was deleted
                if (i == previousInternalBlanks.Current)
                {
                    j++;
                }
                if (j < previousInternalBlanks.Count)
                {
                    internalBlanks[i].AlternateLabel = previousInternalBlanks[j].AlternateLabel;
                    internalBlanks[i].Required = previousInternalBlanks[j].Required;
                    internalBlanks[i].Height = previousInternalBlanks[j].Height;
                    internalBlanks[i].ValidationFunction = previousInternalBlanks[j].ValidationFunction;
                }
            }
        }

        // handles deletion when selection encompasses at least one blank plus other text
        private void copyRemainingAttributesAfterCompoundDeletion(InternalBlankList previousInternalBlanks)
        {
            for (int i = 0, j = 0; i < internalBlanks.Count && j < previousInternalBlanks.Count; i++, j++)
            {
                // skip over the selected ones
                while (previousInternalBlanks[j].Selected)
                {
                    if (++j >= previousInternalBlanks.Count)
                    {
                        break;
                    }
                }

                if (j < previousInternalBlanks.Count)
                {
                    internalBlanks[i].AlternateLabel = previousInternalBlanks[j].AlternateLabel;
                    internalBlanks[i].Required = previousInternalBlanks[j].Required;
                    internalBlanks[i].Height = previousInternalBlanks[j].Height;
                    internalBlanks[i].ValidationFunction = previousInternalBlanks[j].ValidationFunction;
                }
            }
        }

        private void copyAttributesToBlankListAfterAdditionOfBlank(InternalBlankList previousInternalBlanks)
        {
            if (previousInternalBlanks.InsertionIndex != -1)
            {
                for (int i = 0, j = 0; i < internalBlanks.Count && j < previousInternalBlanks.Count; i++, j++)
                {
                    // the insertion index indicates position of the "new" blank
                    if (i == previousInternalBlanks.InsertionIndex)
                    {
                        i++;
                    }

                    if (i < internalBlanks.Count)
                    {
                        internalBlanks[i].AlternateLabel = previousInternalBlanks[j].AlternateLabel;
                        internalBlanks[i].Required = previousInternalBlanks[j].Required;
                        internalBlanks[i].Height = previousInternalBlanks[j].Height;
                        internalBlanks[i].ValidationFunction = previousInternalBlanks[j].ValidationFunction;
                    }
                }
            }
        }

        private void updateInsertionIndexForNewBlankList()
        {
            for (internalBlanks.InsertionIndex = 0; internalBlanks.InsertionIndex < internalBlanks.Count; internalBlanks.InsertionIndex++)
            {
                if (internalBlanks[internalBlanks.InsertionIndex].Start > itemTextEditor.Selection.Start)
                {
                    break;
                }
            }
        }

        private void updateOptionsPanel()
        {
            string fibText = stripCarriageReturns(itemTextEditor.GetText());

            if (fibText.Length > 0)
            {
                bool enable = false;

                int selectionStart = itemTextEditor.Selection.Start;
                int selectionLength = itemTextEditor.Selection.Length;

                if (singleBlankIsSelected(fibText, selectionStart, selectionLength))
                {
                    enable = true;
                    setOptionsPanelAttributesFromBlankAttributes(selectionStart);
                }
                else
                {
                    clearOptionsPanelAttributes();
                    identifyMultipleSelectedBlanks(selectionStart, selectionLength);
                    internalBlanks.Current = -1;
                }

                enableOptionsPanel(enable);
            }
        }

        private bool singleBlankIsSelected(string fibText, int selectionStart, int selectionLength)
        {
            bool blankSelected = false;

            if (selectionLength == 0)
            {
                blankSelected = cursorIsInOrAdjacentToBlank(fibText, selectionStart);
            }
            else
            {
                blankSelected = selectionComprisesOnlyOneBlank(fibText, selectionStart, selectionLength);
            }

            return blankSelected;
        }

        private static bool selectionComprisesOnlyOneBlank(string fibText, int selectionStart, int selectionLength)
        {
            for (int i = selectionStart; i < selectionStart + selectionLength; i++)
            {
                if (fibText[i] != '_')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool cursorIsInOrAdjacentToBlank(string fibText, int cursorIndex)
        {
            bool inBlank = (cursorIndex < fibText.Length) && (fibText[cursorIndex] == '_');
            bool justAfterBlank = (cursorIndex > 0) && (fibText[cursorIndex - 1] == '_');

            return inBlank || justAfterBlank;
        }

        private void setOptionsPanelAttributesFromBlankAttributes(int selectionStart)
        {
            int index = indexOfMatchingBlank(selectionStart);

            if (index != -1)
            {
                optionsPanel.AlternateLabel = internalBlanks[index].AlternateLabel;
                optionsPanel.Required = internalBlanks[index].Required;
                optionsPanel.BlankHeight = internalBlanks[index].Height;
                optionsPanel.ValidationFunction = internalBlanks[index].ValidationFunction;
            }

            internalBlanks.Current = index;
        }

        private int indexOfMatchingBlank(int selectionStart)
        {
            bool blankFound = false;

            int index = 0;
            while (index < internalBlanks.Count)
            {
                InternalBlank currentInternalBlank = internalBlanks[index];
                if (selectionStart >= currentInternalBlank.Start && selectionStart <= currentInternalBlank.End)
                {
                    blankFound = true;
                    break;
                }
                index++;
            }

            return blankFound ? index : -1;
        }

        private static void clearOptionsPanelAttributes()
        {
            optionsPanel.AlternateLabel = "";
            optionsPanel.Required = false;
            optionsPanel.BlankHeight = -1;
            optionsPanel.AlternateLabel = null;
            optionsPanel.ValidationFunction = null;
        }

        private void identifyMultipleSelectedBlanks(int selectionStart, int selectionLength)
        {
            for (int i = 0; i < internalBlanks.Count; i++)
            {
                if (selectionStart <= internalBlanks[i].Start && (selectionStart + selectionLength) >= internalBlanks[i].End)
                {
                    internalBlanks[i].Selected = true;
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (itemTextEditor != null)
            {
                itemTextEditor.Location = new Point(itemTextEditor.Location.X, 0);
                itemTextEditor.Size = itemTextEditor.GetPreferredSize(new Size(Width, 0));

                if (optionsPanel.OwningFibItemView == this)
                {
                    optionsPanel.Location = new Point(itemTextEditor.Location.X, itemTextEditor.Height);
                }
            }
            base.OnLayout(levent);
        }

        // called by the FibOptions container CheckChanged event occurs
        public void UpdateCurrentBlankRequiredFlag()
        {
            if (internalBlanks.Current != -1)
            {
                internalBlanks[internalBlanks.Current].Required = optionsPanel.Required;
                ProjectFibItem.BlankList[internalBlanks.Current].Required = optionsPanel.Required;
            }
        }

        public void UpdateCurrentBlankValidationFunction()
        {
            if (internalBlanks.Current != -1)
            {
                internalBlanks[internalBlanks.Current].ValidationFunction = optionsPanel.ValidationFunction;
                ProjectFibItem.BlankList[internalBlanks.Current].ValidationFunction = optionsPanel.ValidationFunction;
            }
        }

        // called by the FibOptions container when it loses focus
        public void UpdateCurrentBlankAlternateLabel()
        {
            if (internalBlanks.Current != -1)
            {
                internalBlanks[internalBlanks.Current].AlternateLabel = optionsPanel.AlternateLabel;
                ProjectFibItem.BlankList[internalBlanks.Current].AlternateLabel = optionsPanel.AlternateLabel;
            }
        }

        // called by the FibOptions container when it loses focus
        public void UpdateCurrentBlankHeight()
        {
            if (internalBlanks.Current != -1)
            {
                internalBlanks[internalBlanks.Current].Height = optionsPanel.BlankHeight;
                ProjectFibItem.BlankList[internalBlanks.Current].Height = optionsPanel.BlankHeight;
            }
        }

        #region Text Editor Events

        private void itemTextEditor_GotFocus(object sender, EventArgs e)
        {
            optionsPanel.OwningFibItemView = this;
            updateOptionsPanel();
        }

        private void itemTextEditor_InputPositionChanged(object sender, EventArgs e)
        {
            updateBlankList();
            updateOptionsPanel();
        }

        private void itemTextEditor_Changed(object sender, EventArgs e)
        {
            if (ProjectFibItem != null)
            {
                updateBlankList();
            }
        }

        #endregion

        #region Nested type: InternalBlank

        /// <summary>
        /// class to describe a single blank item
        /// </summary>
        private sealed class InternalBlank
        {
            private string alternateLabel = "";

            public InternalBlank(int start, int end)
            {
                Height = 1;
                Start = start;
                End = end;
            }

            public string AlternateLabel
            {
                get { return alternateLabel; }
                set
                {
                    // always trim whitespace from labels
                    alternateLabel = value.Trim();
                }
            }

            public int Height { get; set; }
            public int End { get; private set; }
            public int Start { get; private set; }
            public bool Required { get; set; }
            public bool Selected { get; set; }
            public IFunction ValidationFunction { get; set; }
        }

        #endregion

        #region Nested type: InternalBlankList

        /// <summary>
        /// class to hold a list of blank items in this FIB
        /// </summary>
        private sealed class InternalBlankList : Collection<InternalBlank>
        {
            // index of the blank that has the cursor in it
            // value of -1 indicates cursor is not in the blank
            public int Current;

            // returns the number of blanks that fall within a selection

            // index at which a new blank would be inserted
            // based on the cursor postion in the text
            public int InsertionIndex = -1;

            public InternalBlankList()
            {
                Current = -1;
            }
        }

        #endregion
    }
}