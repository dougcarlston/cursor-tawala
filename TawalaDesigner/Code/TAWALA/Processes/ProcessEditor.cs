// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Tawala.Processes.Properties;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.UndoSupport;
using Component=System.ComponentModel.Component;
using Form=System.Windows.Forms.Form;
using Process=Tawala.Projects.Processes.Process;

namespace Tawala.Processes
{
    /// <summary>
    /// The view class that encapsulates the entire Process UI which is added to the designer's viewPanel when
    /// a process is being shown.
    /// </summary>
    public partial class ProcessEditor : UserControl, IEditMenu, IProcessEditor
    {
        private readonly BindingSource statementBinder = new BindingSource();
        private readonly Dictionary<Type, IStatementEditor> statementTypeToEditorMap = new Dictionary<Type, IStatementEditor>();
        private readonly UndoManager undoManager = new UndoManager();
        private IStatementEditor currentDetails;
        private Process process;
        private bool projectEventsConnected;

        private IStatementSelector statementSelector;
        private Type[] statementViewTypes;
        protected bool validateLines;
        private Form winForm;

        /// <summary>
        /// This constructor is solely for keeping the VSN Form Designer happy
        /// </summary>
        public ProcessEditor()
        {
            InitializeComponent();
        }

        public int InsertionPoint { get { return listBoxStatements.InsertionIndex; } }

        public SplitContainer SplitContainer { get { return splitContainer; } }

        public Panel DetailsPanel { get { return detailsContainerPanel; } }

        public Panel EditPanel { get { return editPanel; } }

        /// <summary>
        /// Return the statements listbox contents as an array
        /// </summary>
        internal string[] ListBoxLines
        {
            get
            {
                var processLineStrings = new string[listBoxStatements.Items.Count];

                for (int i = 0; i < processLineStrings.Length; ++i)
                {
                    var processLine = listBoxStatements.Items[i] as ProcessLine;
                    string processLineString = processLine.ToString();
                    processLineStrings[i] = processLineString.PadLeft(processLineString.Length + processLine.IndentLevel*2);
                }

                return processLineStrings;
            }
        }

        #region IProcessEditor Members

        /// <summary>
        /// Initialize with palette and array used for building menus
        /// </summary>
        public Collection<ToolStripItem> Init(IStatementSelector statementSelector, Type[] statementViewTypes)
        {
            this.statementSelector = statementSelector;
            this.statementViewTypes = statementViewTypes;

            var statementMenuItems = new Collection<ToolStripItem>();

            for (int i = 0; i < statementViewTypes.Length; ++i)
            {
                Type statementViewType = statementViewTypes[i];

                if (isSeparator(statementViewType))
                {
                    if (separatorIsNeeded(statementViewTypes, i))
                    {
                        statementMenuItems.Add(new ToolStripSeparator());
                    }
                }
                else
                {
                    Type statementType =
                        mapStatementToDetails(statementViewType);

                    statementMenuItems.Add(createStatementMenuItem(statementViewType, statementType));
                }
            }

            return statementMenuItems;
        }

        /// <summary>
        /// Process that is loaded into the UI
        /// </summary>
        public Process Process
        {
            get { return process; }
            set
            {
                if (value != null)
                {
                    process = value;

                    statementBinder.DataSource = process.Lines;
                    listBoxStatements.DataSource = statementBinder;

                    listBoxStatements.SelectionDisabled = true;

                    RefreshLines();
                }
            }
        }

        public void StatementButtonClicked(Component component)
        {
            if (component is Control)
            {
                setDetails(((Control)component).Tag as Type);
            }
            else if (component is ToolStripItem)
            {
                setDetails(((ToolStripItem)component).Tag as Type);
            }

            if (currentDetails != null)
            {
                currentDetails.Edit(null);
                listBoxStatements.SelectionDisabled = true;
            }

            scrollInsertionPointIntoView();
        }

        #endregion

        private ToolStripMenuItem createStatementMenuItem(Type statementViewType, Type statementType)
        {
            var statementMenuItem = new ToolStripMenuItem(statementViewType.Name.Replace("Details", ""));
            statementMenuItem.Click += insertStatementStripMenuItem_Click;
            statementMenuItem.Enabled = true;
            statementMenuItem.Tag = statementType;
            return statementMenuItem;
        }

        private Type mapStatementToDetails(Type statementViewType)
        {
            FieldInfo fieldInfo = statementViewType.GetField("statementType",
                                                             BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var statementType = fieldInfo.GetValue(null) as Type;

            var details = (IStatementEditor)Activator.CreateInstance(statementViewType);

            if (!statementTypeToEditorMap.ContainsKey(statementType))
            {
                statementTypeToEditorMap.Add(statementType, details);
            }

            return statementType;
        }

        private static bool separatorIsNeeded(Type[] statementViewTypes, int i)
        {
            return i > 0 && i < statementViewTypes.Length - 1;
        }

        private static bool isSeparator(Type statementViewType)
        {
            return statementViewType == null;
        }

        private void insertStatementStripMenuItem_Click(object sender, EventArgs e)
        {
            StatementButtonClicked(sender as ToolStripItem);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Application.Idle += application_Idle;

            ParentForm.Activated += parentForm_Activated;
            ParentForm.Deactivate += parentForm_Deactivate;
            ParentForm.FormClosed += parentForm_FormClosed;
        }

        private void parentForm_Activated(object sender, EventArgs e)
        {
            statementSelector.SyncButtonToCurrentStatementType(currentDetails != null ? currentDetails.BaseStatementType : null);

            if (currentDetails != null)
            {
                currentDetails.MDIWindowActivated();
            }
        }

        private void parentForm_Deactivate(object sender, EventArgs e)
        {
            if (currentDetails != null)
            {
                if (ParentForm.IsMdiChild)
                {
                    currentDetails.MDIWindowDeactivated();
                }
            }
        }

        private void parentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (currentDetails != null)
            {
                currentDetails.ParentWindowClosed();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ConnectProjectEvents(false);
            Application.Idle -= application_Idle;

            base.OnHandleDestroyed(e);
        }

        public void ConnectProjectEvents(bool connect)
        {
            if (connect && !projectEventsConnected)
            {
                Project.Events.StatementAdded += project_StatementAddedEvent;
                Project.Events.StatementRemoved += project_StatementRemovedEvent;
                Project.Events.StatementModified += project_StatementModifiedEvent;
                projectEventsConnected = true;
            }
            else if (!connect && projectEventsConnected)
            {
                Project.Events.StatementAdded -= project_StatementAddedEvent;
                Project.Events.StatementRemoved -= project_StatementRemovedEvent;
                Project.Events.StatementModified -= project_StatementModifiedEvent;
                projectEventsConnected = false;
            }
        }

        /// <summary>
        /// Indicates whether the insertion point is at a
        /// valid position for statement insertion.
        /// </summary>
        public bool AtInsertPosition()
        {
            bool atInsertPosition = false;

            if (insertionPointIsPastLastLine())
            {
                atInsertPosition = true;
            }
            else if (insertionPointIsAtOrAfterFirstLine())
            {
                if (Process.Lines[listBoxStatements.InsertionIndex].CanInsertBefore)
                {
                    atInsertPosition = true;
                }
            }

            return atInsertPosition;
        }

        private bool insertionPointIsAtOrAfterFirstLine()
        {
            return listBoxStatements.InsertionIndex >= 0;
        }

        private bool insertionPointIsPastLastLine()
        {
            return listBoxStatements.InsertionIndex >= Process.Lines.Count;
        }

        private void scrollInsertionPointIntoView()
        {
            Win32.ListBox_SetCaretIndex(listBoxStatements, calculateCaretIndexForInsertionPoint());
        }

        private int calculateCaretIndexForInsertionPoint()
        {
            int caretIndex = Math.Max(0, listBoxStatements.InsertionIndex - 1);
            caretIndex = Math.Min(listBoxStatements.Items.Count - 1, caretIndex);
            return caretIndex;
        }

        /// <summary>
        /// Called by a Details panel when Inserting/Adding a new statement
        /// </summary>
        public void AddStatement(ProcessStatement ps)
        {
            var newLines = new ProcessLineList(ps);

            if (insertionPointIsNonexistent() || insertionPointIsPastLastLine())
            {
                Process.Lines.Add(newLines);
            }
            else if (Process.Lines[listBoxStatements.InsertionIndex].CanInsertBefore)
            {
                Process.Lines.Insert(listBoxStatements.InsertionIndex, newLines);
            }
            else
            {
                return;
            }

            ProcessLine insertedLine = newLines[0];

            if (insertedLine.SelectsGroup)
            {
                setInsertionPointBetweenParentheses(insertedLine);
            }
            else
            {
                setInsertionPointPastInsertedLine(insertedLine);
            }

            RefreshLines();

            scrollInsertionPointIntoView();
        }

        private void setInsertionPointPastInsertedLine(ProcessLine insertedLine)
        {
            listBoxStatements.InsertionIndex = Process.Lines.LineGroupEndIndex(insertedLine) + 1;
        }

        private void setInsertionPointBetweenParentheses(ProcessLine insertedLine)
        {
            listBoxStatements.InsertionIndex = Process.Lines.LineGroupStartIndex(insertedLine) + 2;
        }

        private bool insertionPointIsNonexistent()
        {
            return listBoxStatements.InsertionIndex == -1;
        }

        public void ModifyStatement(ProcessStatement statement)
        {
            Process.Lines.Replace(listBoxStatements.SelectedIndex, statement);

            // apparently have to play this little trick to get a selected item to scroll into view
            int saveIndex = listBoxStatements.SelectedIndex;
            listBoxStatements.SetSelectedIndex(-1);

            RefreshLines();

            scrollInsertionPointIntoView();

            listBoxStatements.SetSelectedIndex(saveIndex);
            setupStatementForEditing();
        }

        /// <summary>
        /// Remembers the Add or Modify action for subsequent undo
        /// </summary>
        public void RememberAction(string actionText)
        {
            undoManager.Remember(getCurrentState(actionText));
        }

        private void editStatementDetails(ProcessLine line)
        {
            if (line != null && line.Statement != null)
            {
                updateInsertionIndicatorLine(Process.Lines.IndexOf(line));

                setDetails(line.Statement.GetStatementType());

                if (currentDetails != null)
                {
                    currentDetails.Edit(line.Statement);
                }
            }
        }

        private void setDetails(Type statementType)
        {
            detailsContainerPanel.SuspendLayout();

            Control details = null;

            if (Process != null && Process.Lines != null)
            {
                if (Process.Lines.Count == 0)
                {
                    details = detailsInstructionPanel;
                }

                if (statementType != null)
                {
                    if (statementTypeToEditorMap.ContainsKey(statementType))
                    {
                        details = statementTypeToEditorMap[statementType] as Control;
                    }
                }
            }

            if (details != null)
            {
                if (!detailsContainerPanel.Controls.Contains(details))
                {
                    process.ActiveGetStatement = null;

                    detailsContainerPanel.Controls.Clear();
                    detailsContainerPanel.Controls.Add(details);
                }
            }
            else
            {
                detailsContainerPanel.Controls.Clear();
            }

            Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs());

            detailsContainerPanel.ResumeLayout();
        }

        /// <summary>
        /// Refresh the statements listbox datasource.  Try to remember current selection and resync to it.
        /// </summary>
        internal void RefreshLines()
        {
            if (Process != null)
            {
                Process.Lines.SetIndentLevels();

                int topindex = Win32.ListBox_GetTopIndex(listBoxStatements);
                int selectedIndex = listBoxStatements.SelectedIndex;

                int position = statementBinder.Position;
                statementBinder.DataSource = Process.Lines;
                statementBinder.ResetBindings(false);

                if (selectedIndex != -1)
                {
                    statementBinder.Position = position;
                }

                Win32.ListBox_SetTopIndex(listBoxStatements, topindex);

                validateLines = true;

                editPanel.Invalidate();
            }
        }

        #region Details Panel events

        private void detailsPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            currentDetails = e.Control as IStatementEditor;
            statementSelector.SyncButtonToCurrentStatementType(currentDetails != null ? currentDetails.BaseStatementType : null);
        }

        private void detailsPanel_ControlRemoved(object sender, ControlEventArgs e)
        {
            currentDetails = null;
            statementSelector.SyncButtonToCurrentStatementType(null);
        }

        private void detailsPanel_Resize(object sender, EventArgs e)
        {
            detailsContainerPanel.Invalidate();
        }

        #endregion

        #region Project Events

        private void project_StatementAddedEvent(object sender, StatementEventArgs e)
        {
            RefreshLines();
        }

        private void project_StatementRemovedEvent(object sender, StatementEventArgs e)
        {
            RefreshLines();

            if (selectionIndexIsNonexistent())
            {
                var line = listBoxStatements.SelectedItem as ProcessLine;

                if (line == null || line.Statement == null)
                {
                    setDetails(null);
                }
                else
                {
                    editStatementDetails(line);
                }
            }
            else if (statementsListBoxIsEmpty())
            {
                if (currentDetails != null)
                {
                    currentDetails.SwitchToAddMode();
                }

                listBoxStatements.SelectionDisabled = true;
            }
        }

        private bool statementsListBoxIsEmpty()
        {
            return listBoxStatements.Items.Count == 0;
        }

        private bool selectionIndexIsNonexistent()
        {
            return listBoxStatements.SelectedIndex != -1;
        }

        private void project_StatementModifiedEvent(object sender, StatementEventArgs e)
        {
            RefreshLines();
        }

        #endregion

        #region ListBox Events

        private static readonly string processLineFormatString = "processLine";
        private Rectangle dragBox = Rectangle.Empty;

        private int dragSourceIndex = -1;
        private bool inLowerScrollZone;
        private bool inUpperScrollZone;
        private Rectangle selectionRepaintRectangle = Rectangle.Empty;

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            setupStatementForEditing();
        }

        private void setupStatementForEditing()
        {
            var line = (ProcessLine)listBoxStatements.SelectedItem;

            selectAllLinesInStatementGroup(line, true);

            if (line != null)
            {
                bool listBoxWasFocused = listBoxStatements.Focused;

                if (line.Statement != null)
                {
                    editStatementDetails(line);
                }
                else
                {
                    setDetails(null);
                }

                restoreStatementsListBoxFocus(listBoxWasFocused);
            }

            editPanel.Invalidate();
        }

        private void restoreStatementsListBoxFocus(bool listBoxWasFocused)
        {
            if (listBoxWasFocused && !listBoxStatements.Focused)
            {
                listBoxStatements.Focus();
            }
        }

        private void selectAllLinesInStatementGroup(ProcessLine line, bool repaintImmediately)
        {
            Rectangle previousSelectionRepaintRectangle = selectionRepaintRectangle;

            if (line == null)
            {
                listBoxStatements.SelectedLinesStartIndex = -1;
                listBoxStatements.SelectedLinesEndIndex = -1;

                selectionRepaintRectangle = Rectangle.Empty;
            }
            else
            {
                listBoxStatements.SelectedLinesStartIndex = process.Lines.LineGroupStartIndex(line);
                listBoxStatements.SelectedLinesEndIndex = process.Lines.LineGroupEndIndex(line);

                calculateNewSelectionRepaintRectangle();
            }

            if (repaintImmediately)
            {
                if (!previousSelectionRepaintRectangle.IsEmpty)
                {
                    listBoxStatements.Invalidate(previousSelectionRepaintRectangle);
                }
                if (!selectionRepaintRectangle.IsEmpty)
                {
                    listBoxStatements.Invalidate(selectionRepaintRectangle);
                }

                listBoxStatements.Update();
            }
        }

        private void calculateNewSelectionRepaintRectangle()
        {
            selectionRepaintRectangle = listBoxStatements.GetItemRectangle(listBoxStatements.SelectedLinesStartIndex);
            selectionRepaintRectangle.Inflate(0,
                                              (listBoxStatements.GetItemRectangle(listBoxStatements.SelectedLinesEndIndex).Bottom -
                                               selectionRepaintRectangle.Bottom));
        }

        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            listBoxStatements.SelectionDisabled = false;

            dragSourceIndex = listBoxStatements.IndexFromPoint(e.X, e.Y);

            ProcessLine line = dragSourceIndex >= 0 ? (ProcessLine)listBoxStatements.Items[dragSourceIndex] : null;
            selectAllLinesInStatementGroup(line, true);

            if (dragSourceIndex >= 0 && dragSourceIndex < listBoxStatements.Items.Count)
            {
                int insertionPoint = listBoxStatements.GetItemRectangle(dragSourceIndex).Top;
                updateInsertionIndicators(new Point(0, insertionPoint));
            }

            Size dragSize = SystemInformation.DragSize;

            dragBox = new Rectangle(new Point(e.X - (dragSize.Width/2), e.Y - (dragSize.Height/2)), dragSize);
        }

        private void listBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftMouseButtonPressed(e))
            {
                if (mouseMovesOutsideDragBox(e))
                {
                    DataFormats.Format processLineFormat = DataFormats.GetFormat(processLineFormatString);

                    if (dragSourceIndexIsValid())
                    {
                        ProcessLine line = Process.Lines[dragSourceIndex];

                        if (isDragable(line))
                        {
                            clearScrollTimer();

                            DragDropEffects result = initiateDragDrop(createDataObject(processLineFormat));

                            if (droppedOutsideListBox(result))
                            {
                                editStatementDetails(line);
                            }
                        }
                    }
                }
            }
        }

        private DragDropEffects initiateDragDrop(DataObject dObj)
        {
            DragDropEffects result = listBoxStatements.DoDragDrop(dObj, DragDropEffects.Move);
            return result;
        }

        private DataObject createDataObject(DataFormats.Format processLineFormat)
        {
            Object item = listBoxStatements.Items[dragSourceIndex];
            var dObj = new DataObject(processLineFormat.Name, item);
            return dObj;
        }

        private static bool isDragable(ProcessLine line)
        {
            return line.Statement != null;
        }

        private bool dragSourceIndexIsValid()
        {
            return dragSourceIndex >= 0 && dragSourceIndex < listBoxStatements.Items.Count;
        }

        private bool mouseMovesOutsideDragBox(MouseEventArgs e)
        {
            return dragBox != Rectangle.Empty && !dragBox.Contains(e.X, e.Y);
        }

        private static bool leftMouseButtonPressed(MouseEventArgs e)
        {
            return (e.Button & MouseButtons.Left) == MouseButtons.Left;
        }

        private static bool droppedOutsideListBox(DragDropEffects result)
        {
            return result == DragDropEffects.None;
        }

        private void scrollUp()
        {
            // if topmost scroll position not yet reached...
            if (listBoxStatements.TopIndex > 0)
            {
                // scroll up one item
                listBoxStatements.TopIndex--;
            }
        }

        private void scrollDown()
        {
            // establish greatest possible top item index
            int greatestTopIndex = listBoxStatements.Items.Count - (listBoxStatements.Height/listBoxStatements.GetItemHeight(0));

            // if bottommost scroll position not yet reached...
            if (listBoxStatements.TopIndex < greatestTopIndex)
            {
                // scroll down one item
                listBoxStatements.TopIndex++;
            }
        }

        private void setScrollTimer(Point point)
        {
            inUpperScrollZone = point.Y < 15;
            inLowerScrollZone = point.Y > (listBoxStatements.Height - 15);

            if (inUpperScrollZone || inLowerScrollZone)
            {
                startScrollTimer();
            }
            else
            {
                stopScrollTimer();
            }
        }

        private void stopScrollTimer()
        {
            scrollTimer.Enabled = false;
        }

        private void startScrollTimer()
        {
            scrollTimer.Enabled = true;
        }

        private void clearScrollTimer()
        {
            scrollTimer.Enabled = false;
            inUpperScrollZone = false;
            inLowerScrollZone = false;
        }

        private void scrollTimer_Tick(object sender, EventArgs e)
        {
            if (inUpperScrollZone)
            {
                scrollUp();
            }

            if (inLowerScrollZone)
            {
                scrollDown();
            }
        }

        private void listBox_DragOver(object sender, DragEventArgs e)
        {
            // indicate by default that no drop can occur
            e.Effect = DragDropEffects.None;

            var line = (ProcessLine)e.Data.GetData(processLineFormatString);

            if (isInCurrentProcess(line))
            {
                // indicate that data may be dropped
                e.Effect = DragDropEffects.Move;

                Point clientPoint = listBoxStatements.PointToClient(new Point(e.X, e.Y));
                setScrollTimer(clientPoint);
                updateInsertionIndicators(clientPoint);
            }
        }

        private bool isInCurrentProcess(ProcessLine line)
        {
            return Process.Lines.Contains(line);
        }

        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            var line = (ProcessLine)e.Data.GetData(processLineFormatString);

            if (isInCurrentProcess(line))
            {
                Debug.Assert(dragSourceIndex == Process.Lines.IndexOf(line));

                int targetIndex = listBoxStatements.InsertionIndex;
                if (targetIndex < 0 || targetIndex >= listBoxStatements.Items.Count)
                {
                    targetIndex = -1; // indicates drop at end
                }

                if (lineIsBeingMoved(e))
                {
                    clearScrollTimer();

                    undoManager.Remember(getCurrentState("Move"));

                    dropDraggedLine(targetIndex);

                    RefreshLines();

                    int selectionIndex = Process.Lines.IndexOf(line);
                    listBoxStatements.SetSelectedIndex(selectionIndex);

                    selectAllLinesInStatementGroup(line, false);

                    // set insertion indicators to line above the one just dropped
                    Rectangle rect = listBoxStatements.GetItemRectangle(selectionIndex);
                    updateInsertionIndicators(new Point(rect.Left, rect.Top));

                    listBoxStatements.Invalidate();

                    editStatementDetails(line);
                }
            }
        }

        private void dropDraggedLine(int targetIndex)
        {
            Process.Lines.DragDrop(dragSourceIndex, targetIndex);
            listBoxStatements.InsertionIndex = -1;
            dragSourceIndex = -1;
        }

        private static bool lineIsBeingMoved(DragEventArgs e)
        {
            return e.Effect == DragDropEffects.Move;
        }

        private void listBox_DragLeave(object sender, EventArgs e)
        {
            clearScrollTimer();

            if (listBoxStatements.SelectedIndex >= 0 && listBoxStatements.SelectedIndex < listBoxStatements.Items.Count)
            {
                Rectangle rect = listBoxStatements.GetItemRectangle(listBoxStatements.SelectedIndex);
                updateInsertionIndicators(new Point(rect.Left, rect.Top));

                listBoxStatements.Invalidate();
                editPanel.Invalidate();
            }
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            menuItemCut.Enabled = ((IEditMenu)this).CanCut();
            menuItemCopy.Enabled = ((IEditMenu)this).CanCopy();
            menuItemPaste.Enabled = ((IEditMenu)this).CanPaste();
            menuItemDelete.Enabled = ((IEditMenu)this).CanDelete();
        }

        private void menuItemCut_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Cut();
        }

        private void menuItemCopy_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Copy();
        }

        private void menuItemPaste_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Paste();
        }

        private void menuItemDelete_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Delete();
        }

        #endregion

        #region Edit Panel Events

        private Boolean draggingMarker;
        private Point markerPosition = new Point(0, 0);
        private int preservedInsertionIndex;

        private void editPanel_MouseDown(object sender, MouseEventArgs e)
        {
            preservedInsertionIndex = listBoxStatements.InsertionIndex;

            listBoxStatements.SelectionDisabled = true;

            process.ActiveGetStatement = null;

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                draggingMarker = true;
                updateInsertionIndicators(new Point(e.X, e.Y));
            }
        }

        private void editPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                setScrollTimer(new Point(e.X + editPanel.DockPadding.Left + 8, e.Y));
                updateInsertionIndicators(new Point(e.X, e.Y));
            }
        }

        private void editPanel_MouseUp(object sender, MouseEventArgs e)
        {
            draggingMarker = false;

            clearScrollTimer();

            listBoxStatements.Focus();

            if (detailsContainerPanel.Controls.Count != 0 && detailsContainerPanel.Controls[0] is IStatementEditor)
            {
                ((IStatementEditor)detailsContainerPanel.Controls[0]).SwitchToAddMode();
            }

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                int y = 0;

                if (listBoxStatements.InsertionIndex > listBoxStatements.Items.Count)
                {
                    listBoxStatements.InsertionIndex = listBoxStatements.Items.Count;
                }

                if (listBoxStatements.InsertionIndex != -1 && listBoxStatements.InsertionIndex <= listBoxStatements.Items.Count)
                {
                    if (listBoxStatements.InsertionIndex == listBoxStatements.Items.Count)
                    {
                        if (listBoxStatements.Items.Count != 0)
                        {
                            y = listBoxStatements.GetItemRectangle(listBoxStatements.InsertionIndex - 1).Y + listBoxStatements.ItemHeight;
                        }
                    }
                    else
                    {
                        y = listBoxStatements.GetItemRectangle(listBoxStatements.InsertionIndex).Y;
                    }

                    y = (y != 0) ? y - 2 : 2;

                    updateMarkerPosition(y);
                }
            }

            if (preservedInsertionIndex != listBoxStatements.InsertionIndex)
            {
                Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, listBoxStatements.InsertionIndex));
                Project.Events.RaiseProcessLineIndexChangedEvent(EventArgs.Empty);
            }
        }

        private void updateInsertionIndicators(Point pt)
        {
            updateMarkerPosition(pt.Y);

            updateInsertionIndicatorLine(insertionIndexFromPoint(pt));

            scrollInsertionPointIntoView();
        }

        private int insertionIndexFromPoint(Point pt)
        {
            int index = 0;
            if (listBoxStatements.Items.Count > 0)
            {
                pt.X += editPanel.DockPadding.Left + 8;

                index = listBoxStatements.IndexFromPoint(pt);

                if (index == -1)
                {
                    if (pt.Y - listBoxStatements.ItemHeight < 0)
                    {
                        index = 0;
                    }
                    else
                    {
                        index = listBoxStatements.Items.Count;
                    }
                }
                else
                {
                    // if mouse point is in bottom half of item rect
                    if (pt.Y%listBoxStatements.ItemHeight > listBoxStatements.ItemHeight/2)
                    {
                        // jump to the next point
                        index++;
                    }
                }
            }

            return index;
        }

        private void updateInsertionIndicatorLine(int index)
        {
            int oldInsertionIndex = listBoxStatements.InsertionIndex;

            if (index < 0 || index >= listBoxStatements.Items.Count)
            {
                listBoxStatements.InsertionIndex = index;
            }
            else if (Process.Lines[index].CanInsertBefore)
            {
                listBoxStatements.InsertionIndex = index;
            }

            // set up to draw line indicating insertion point
            if (oldInsertionIndex != listBoxStatements.InsertionIndex)
            {
                Rectangle oldUpdateRect = Rectangle.Empty;
                ;
                if (oldInsertionIndex >= 0 && oldInsertionIndex < listBoxStatements.Items.Count)
                {
                    oldUpdateRect = listBoxStatements.GetItemRectangle(oldInsertionIndex);
                    oldUpdateRect.Inflate(0, 2);
                }

                Rectangle updateRect = Rectangle.Empty;
                if (listBoxStatements.InsertionIndex >= 0 && listBoxStatements.InsertionIndex < listBoxStatements.Items.Count)
                {
                    updateRect = listBoxStatements.GetItemRectangle(listBoxStatements.InsertionIndex);
                    updateRect.Inflate(0, 2);
                }

                if (!oldUpdateRect.IsEmpty)
                {
                    listBoxStatements.Invalidate(oldUpdateRect);
                }
                if (!updateRect.IsEmpty)
                {
                    listBoxStatements.Invalidate(updateRect);
                }

                listBoxStatements.Update();
            }
        }

        private void updateMarkerPosition(int yPos)
        {
            if (markerPosition.Y != yPos)
            {
                markerPosition.Y = yPos;
                editPanel.Invalidate();
            }
        }

        private void editPanel_Paint(object sender, PaintEventArgs e)
        {
            // if not currently dragging the insertion marker (arrow)
            if (!draggingMarker)
            {
                // sync the marker position with the insertion line postion
                markerPosition.Y = 0;
                if (listBoxStatements.Items.Count > 0)
                {
                    if (listBoxStatements.InsertionIndex >= listBoxStatements.Items.Count)
                    {
                        markerPosition.Y = listBoxStatements.GetItemRectangle(listBoxStatements.Items.Count - 1).Bottom;
                    }
                    else if (listBoxStatements.InsertionIndex >= 0)
                    {
                        markerPosition.Y = listBoxStatements.GetItemRectangle(listBoxStatements.InsertionIndex).Top;
                    }
                }
            }

            e.Graphics.DrawImage(Resources.InsertMarker, 0, markerPosition.Y);

            // draw border across top of panel
            e.Graphics.DrawLine(Pens.DarkGray, 0, 0, editPanel.Width, 0);
        }

        #endregion

        #region IEditMenu for Main Form's Edit Menu

        private static ProcessLineList clipboardLines;

        bool IEditMenu.CanCut()
        {
            if (processLineIsSelected())
            {
                var processLine = listBoxStatements.Items[listBoxStatements.SelectedIndex] as ProcessLine;

                if (processLine != null)
                {
                    return processLine.IsDeletable;
                }
            }

            return false;
        }

        bool IEditMenu.CanCopy()
        {
            if (processLineIsSelected())
            {
                var processLine = listBoxStatements.Items[listBoxStatements.SelectedIndex] as ProcessLine;

                if (processLine != null)
                {
                    bool isStatementLine = processLine.Statement != null;
                    return (processLine.IsSelectable && isStatementLine);
                }
            }

            return false;
        }

        bool IEditMenu.CanPaste()
        {
            bool canPaste = false;

            if (editPanel.ContainsFocus)
            {
                bool atPastePosition;

                if (!processLineIsSelected())
                {
                    if (insertionPointIsVisible())
                    {
                        if (insertionPointIsPastLastLine())
                        {
                            atPastePosition = true;
                        }
                        else
                        {
                            atPastePosition = Process.Lines[listBoxStatements.InsertionIndex].CanInsertBefore;
                        }

                        canPaste = (clipboardLines != null) && atPastePosition;
                    }
                }
                else
                {
                    var processLine = listBoxStatements.Items[listBoxStatements.SelectedIndex] as ProcessLine;

                    if (processLine != null)
                    {
                        atPastePosition = processLine.CanInsertBefore;
                        canPaste = (clipboardLines != null) && atPastePosition;
                    }
                }
            }

            return (canPaste);
        }

        bool IEditMenu.CanDelete()
        {
            if (editPanel.ContainsFocus && listBoxStatements.SelectedIndex != -1)
            {
                var processLine = listBoxStatements.Items[listBoxStatements.SelectedIndex] as ProcessLine;

                if (processLine != null)
                {
                    return processLine.IsDeletable;
                }
            }

            return false;
        }

        bool IEditMenu.CanRename()
        {
            return false;
        }

        void IEditMenu.Cut()
        {
            if (((IEditMenu)this).CanCut())
            {
                ProcessLine line = Process.Lines[listBoxStatements.SelectedIndex];

                if (line.IsDeletable)
                {
                    var lines = new ProcessLineList(listBoxStatements.SelectedIndex, Process.Lines);
                    setDataObject(lines);

                    undoManager.Remember(getCurrentState("Cut"));

                    if (line.SelectsGroup)
                    {
                        Process.Lines.Remove(listBoxStatements.SelectedIndex, line.Group);
                    }
                    else
                    {
                        Process.Lines.RemoveAt(listBoxStatements.SelectedIndex);
                    }

                    listBoxStatements.SetSelectedIndex(-1);
                }
            }
        }

        void IEditMenu.Copy()
        {
            if (((IEditMenu)this).CanCopy())
            {
                var lines = new ProcessLineList(listBoxStatements.SelectedIndex, Process.Lines);
                setDataObject(lines);
            }
        }

        void IEditMenu.Paste()
        {
            if (((IEditMenu)this).CanPaste())
            {
                ProcessLineList lines = getData();

                undoManager.Remember(getCurrentState("Paste"));

                if (processLineIsSelected())
                {
                    Process.Lines.Insert(listBoxStatements.SelectedIndex, lines);
                }
                else if (insertionPointIsVisible())
                {
                    Process.Lines.Insert(listBoxStatements.InsertionIndex, lines);
                }
            }
        }

        void IEditMenu.Delete()
        {
            if (((IEditMenu)this).CanDelete())
            {
                ProcessLine line = Process.Lines[listBoxStatements.SelectedIndex];

                if (line.IsDeletable)
                {
                    undoManager.Remember(getCurrentState("Delete"));

                    if (line.SelectsGroup)
                    {
                        Process.Lines.Remove(listBoxStatements.SelectedIndex, line.Group);
                    }
                    else
                    {
                        Process.Lines.RemoveAt(listBoxStatements.SelectedIndex);
                    }

                    listBoxStatements.SetSelectedIndex(-1);
                }
            }
        }

        void IEditMenu.Rename()
        {
        }

        ToolStripMenuItem[] IEditMenu.GetAdditionalMenuItems()
        {
            return null;
        }

        bool IEditMenu.CanUndo()
        {
			// Disabled undo for now; causes memory issues with large projects such as SDT and CampaignDashboards.
			//		jdf 09/09
            //return undoManager.CanUndo;
        	return false;
        }

        bool IEditMenu.CanRedo()
        {
			// Disabled undo for now; causes memory issues with large projects such as SDT and CampaignDashboards.
			//		jdf 09/09
			//return undoManager.CanRedo;
        	return false;
        }

        void IEditMenu.Undo()
        {
            if (undoManager.CanUndo)
            {
                restoreCurrentState(undoManager.Undo(getCurrentState(undoManager.UndoActionText)));
            }
        }

        void IEditMenu.Redo()
        {
            if (undoManager.CanRedo)
            {
                restoreCurrentState(undoManager.Redo(getCurrentState(undoManager.RedoActionText)));
            }
        }

        string IEditMenu.UndoActionText { get { return undoManager.UndoActionText; } }

        string IEditMenu.RedoActionText { get { return undoManager.RedoActionText; } }

        private bool processLineIsSelected()
        {
            return editPanel.ContainsFocus && listBoxStatements.SelectedIndex != -1;
        }

        private bool insertionPointIsVisible()
        {
            return listBoxStatements.InsertionIndex != -1;
        }

        protected void setDataObject(ProcessLineList lines)
        {
            clipboardLines = lines.Copy();
        }

        protected ProcessLineList getData()
        {
            return clipboardLines.Copy();
        }

        /// <summary>
        /// Returns a memento representing the current state of the process editor
        /// </summary>
        private IMemento getCurrentState(string actionText)
        {
            return new ProcessEditorMemento(process, listBoxStatements.SelectedIndex, actionText);
        }

        /// <summary>
        /// Restores the current state of the process editor from the specified memento
        /// </summary>
        private void restoreCurrentState(IMemento memento)
        {
            var processMemento = memento as ProcessEditorMemento;

            if (processMemento != null)
            {
                process.Lines = processMemento.Lines;
                process.Lines.Process = Process;
                process.Variables = processMemento.Variables;
                RefreshLines();
                listBoxStatements.SetSelectedIndex(processMemento.SelectedIndex);
                setupStatementForEditing();
            }
        }

        public void ResetUndo()
        {
            undoManager.Reset();
        }

        [Serializable]
        private class ProcessEditorMemento : Memento
        {
            private readonly ProcessLineList lines;
            private readonly int selectedIndex;
            private readonly VariableList variables;

            public ProcessEditorMemento(Process process, int selectedLineIndex, string actionText)
                : base(actionText)
            {
                lines = process.Lines;
                variables = process.Variables;
                selectedIndex = selectedLineIndex;
            }

            public ProcessLineList Lines { get { return lines; } }

            public VariableList Variables { get { return variables; } }

            public int SelectedIndex { get { return selectedIndex; } }
        }

        #endregion

        #region Application Events

        private void application_Idle(object sender, EventArgs e)
        {
            Form f = FindForm();

            // Have we rejoined the window heirarchy?

            if (winForm == null && f != null)
            {
                PerformLayout();
            }

            winForm = f;

            if (winForm != null) // we are in the window heirarchy so do idle processing
            {
                doIdle();

                if (detailsContainerPanel.Controls.Count != 0 && detailsContainerPanel.Controls[0] is IStatementEditor)
                {
                    ((IStatementEditor)detailsContainerPanel.Controls[0]).DoIdle();
                }
            }
        }

        private void doIdle()
        {
            if (validateLines)
            {
                validateLines = false;
                Process.Lines.ValidateLines();
                listBoxStatements.Invalidate();
            }
        }

        #endregion
    }

    #region StatementsListBox class

    #endregion
}