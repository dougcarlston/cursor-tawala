// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.Forms.Dialogs;
using Tawala.Forms.Properties;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;

namespace Tawala.Forms
{
    /// <summary>
    /// This control contains Form Item controls derived from ItemViewBase.
    /// It itself is contained in Tawala.Forms.View to the right of the Palette
    /// </summary>
    public partial class FormItemContainer : UserControl, IEditMenu
    {
        // Selection anchor 
        // -1 no item is the anchor, 0 and up its the index of the anchor item

        private const string anchor = "#anchor-";
        private static readonly StringFormat drawFormat = new StringFormat();
        private static readonly Font emptyMsgFont;
        private bool doIdleDragMarkerCleanup;

        /// <summary>
        /// -1 = no drag insertion marker
        /// 0 to Controls.Count -1 = insertion marker on ItemViewBase control
        /// Controls.Count = insertion marker after last control
        /// </summary>
        private int dragInsertBefore = -1;

        private IForm form;
        private int oldDragInsertBefore = -1;
        private int selectionAnchor = -1;

        static FormItemContainer()
        {
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;

            emptyMsgFont = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Bold);
        }

        public FormItemContainer()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public IForm Form { get { return form; } }

        public int DragInsertBefore
        {
            get { return dragInsertBefore; }
            set
            {
                oldDragInsertBefore = dragInsertBefore;
                dragInsertBefore = Math.Max(value, -1);

                if (dragInsertBefore == oldDragInsertBefore)
                {
                    return; // ignore - no change
                }

                Image dragMark = Resources.Form_ItemDragInsert;
                var r = new Rectangle(0, 0, dragMark.Width, dragMark.Height);

                DragImage.Leave();

                if (oldDragInsertBefore >= 0 && oldDragInsertBefore < Controls.Count)
                {
                    var ib = Controls[oldDragInsertBefore] as ItemViewBase;
                    ib.DragInsertionMark = null;
                    // invalidate previous marker image location
                    ib.Invalidate(r, false);
                }

                if (dragInsertBefore >= 0 && dragInsertBefore < Controls.Count)
                {
                    var ib = Controls[dragInsertBefore] as ItemViewBase;
                    // invalidate previous marker image location
                    ib.Invalidate(r, false);
                    ib.DragInsertionMark = dragMark;
                }

                if (dragInsertBefore >= 0)
                {
                    // this will cause the idle handler to check and see if a drag left the
                    // item container and make sure the dragInsertBefore gets set back to -1.
                    // Other mechanisms are supposed to handle this but they don't always work.
                    doIdleDragMarkerCleanup = true;
                }

                Invalidate(false);
                Update();

                DragImage.Enter();
            }
        }

        #region IEditMenu Members

        /// <summary>
        /// Delete selected items
        /// </summary>
        public void Delete()
        {
            var list = new ArrayList();

            foreach (Control c in Controls)
            {
                var item = c as ItemViewBase;

                if (item != null && item.Selected)
                {
                    list.Add(item);
                }
            }

            int newSel = -1;

            foreach (ItemViewBase item in list)
            {
                newSel = Controls.GetChildIndex(item);
                item.Delete();
            }

            if (newSel >= Controls.Count)
            {
                newSel = Controls.Count - 1;
            }

            if (newSel >= 0 && newSel < Controls.Count)
            {
                var item = Controls[newSel] as ItemViewBase;
                SetAnchor(item);
                setFocusToChild(item);
                ((MDIFormView)ParentForm).TargetTextEditor = null;
                ScrollControlIntoView(item);
            }

            Invalidate();
        }

        #endregion

        /// <summary>
        /// Do some work on idle cycles
        /// </summary>
        private void appOnIdle(object sender, EventArgs e)
        {
            if (doIdleDragMarkerCleanup)
            {
                if (DragInsertBefore >= 0 && (MouseButtons & MouseButtons.Left) == 0)
                {
                    doIdleDragMarkerCleanup = false;
                    DragInsertBefore = -1;
                }
            }
        }

        public void SetForm(IForm form)
        {
            Debug.Assert(form != null);
            Debug.Assert(this.form == null);

            this.form = form;

            SuspendLayout();

            for (int order = 0; order < form.ItemList.Count; ++order)
            {
                ItemViewBase.Create(form.ItemList[order], this, order);
            }

            syncDefaultLabels();
            ResumeLayout(false);
            PerformLayout();
        }

        public bool CanInsertImage(ItemTextEditor itemTextEditor)
        {
            if (itemTextEditor == null)
            {
                return false;
            }

            TextItemView selItemView = null;
            int selCount = 0;

            foreach (Control c in Controls)
            {
                var ib = c as ItemViewBase;

                if (ib.Selected)
                {
                    if (ib is TextItemView)
                    {
                        selItemView = ib as TextItemView;
                    }
                    selCount++;
                }
            }

            if (selCount != 1 || selItemView == null)
            {
                return false;
            }

            if (!selItemView.Contains(itemTextEditor))
            {
                return false;
            }

            return true;
        }

        public void InsertImage(ItemTextEditor itemTextEditor)
        {
            if (CanInsertImage(itemTextEditor))
            {
                itemTextEditor.InsertImage();
            }
        }

        /// <summary>
        /// As palette item continues to be dragged over container invalidate control if insertion index
        /// changes so that the drag insertion cursor is updated.
        /// </summary>
        public void PublicDragOver(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(FormItemPalette.DragFormat) || drgevent.Data.GetDataPresent(typeof(FormItem)))
            {
                DragInsertBefore = calcDragPosition(drgevent.X, drgevent.Y);
            }
        }

        /// <summary>
        /// Something was dropped on the control
        /// In this case we assume that it is the Type object for something derived from ItemViewBase;
        /// a Form Item from the Palette
        /// </summary>
        public void PublicDragDrop(DragEventArgs drgevent)
        {
            DragInsertBefore = -1;

            int currentDragInsertBefore = calcDragPosition(drgevent.X, drgevent.Y);

            if (drgevent.Data.GetDataPresent(FormItemPalette.DragFormat))
            {
                FormItemCreator dragged = FormItemPalette.DragObject;

                if (dragged != null)
                {
                    FormItem item = dragged.CreateItem();
                    addedToFormByUser = true;
                    form.ItemList.Insert(currentDragInsertBefore, item);
                }
            }
            else if (drgevent.Data.GetDataPresent(typeof(FormItem)))
            {
                var item = (FormItem)drgevent.Data.GetData(typeof(FormItem));
                //				form.ItemList.Paste(currentDragInsertBefore, item);
                form.ItemList.Move(item, currentDragInsertBefore);
            }
            Invalidate();
        }

        /// <summary>
        /// Dragged object is leaving us; re-layout and invalidate to remove insertion cursor
        /// </summary>
        public void PublicDragLeave(EventArgs e)
        {
            DragInsertBefore = -1;
        }

        /// <summary>
        /// Cut the currently selected items to the clipboard
        /// </summary>
        public FormItemList Cut()
        {
            var cutList = new FormItemList();

            foreach (Control c in Controls)
            {
                var item = c as ItemViewBase;

                if (item != null && item.Selected)
                {
                    cutList.Add(item.FormItem);
                }
            }

            Delete();

            return cutList;
        }

        /// <summary>
        /// Copy the currently selected items to the clipboard
        /// </summary>
        public FormItemList Copy()
        {
            var copyList = new FormItemList();

            foreach (Control c in Controls)
            {
                var item = c as ItemViewBase;

                if (item != null && item.Selected)
                {
                    // make a deep copy of the item
                    FormItem clonedItem = Cloner.Clone((FormItem)item.FormItem);

                    // indicate that this is a copy rather than a cut
                    // (setting id to 0 causes new id to be created upon deserialization)
                    clonedItem.ClearId();

                    // add duplicate item to copy list
                    copyList.Add(clonedItem);
                }
            }

            return copyList;
        }

        /// <summary>
        /// Deselect all items except for possibly one
        /// </summary>
        public void DeselectAll(ItemViewBase except)
        {
            foreach (Control c in Controls)
            {
                if (c != except && c is ItemViewBase)
                {
                    ((ItemViewBase)c).Selected = false;
                }
            }
        }

        public void SetAnchor(ItemViewBase item)
        {
            selectionAnchor = -1;

            if (item != null)
            {
                DeselectAll(item);
                item.Selected = true;
                selectionAnchor = Controls.GetChildIndex(item, false);
            }
        }

        public string GetPreviewAnchorName()
        {
            for (int order = 0; order < Controls.Count; ++order)
            {
                var ib = Controls[order] as ItemViewBase;
                int y = ib.Location.Y;
                if (y >= -10 && y < ClientSize.Height)
                {
                    if ((ib is HeadingView || ib.FormItem.IsTextItem || ib.FormItem.IsQuestionItem) && order == 0)
                    {
                        break;
                    }

                    if (ib is HeadingView || ib is TextItemView || ib is McqItemView)
                    {
                        return anchor + ib.FormItem.FieldName;
                    }

                    if (ib is FibItemView)
                    {
                        var fib = ib as FibItemView;

                        if (fib.ProjectFibItem.BlankList.Count > 0)
                        {
                            return anchor + fib.ProjectFibItem.BlankList[0].FieldName;
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Based on drag (screen) coordinates, calculate the index where a new item would be inserted
        /// </summary>
        private int calcDragPosition(int x, int y)
        {
            Point pt = PointToClient(new Point(x, y));
            int dragY = pt.Y;
            int order = Controls.Count;

            // Loop through all this container's Controls (which should all be derived from ItemViewBase)
            // and check to see if the user dropped the new item above or below the middle of an existing
            // item.

            for (order = 0; order < Controls.Count; ++order)
            {
                var ib = Controls[order] as ItemViewBase;

                int endy = ib.Location.Y + ib.Height;
                if (dragY >= ib.Location.Y && dragY < endy)
                {
                    if (dragY <= (endy - ib.Height/2))
                    {
                        return order;
                    }

                    return ++order;
                }
            }
            return order;
        }

        /// <summary>
        /// Synchronize the defaults labels based on position / z-order
        /// </summary>
        /// <remarks>
        /// This is related to the the order in the Controls collection and z-order
        /// and is modified by calls to Controls.SetChildIndex or by adding or removing
        /// controls.  Moved it out of OnLayout as the labels should be setup before layout
        /// occurs in cases we someday decide to make space for label display wider or something.
        /// Layout operations caused by resizing, etc, don't need to cause the label to be regenerated.
        /// </remarks>
        private void syncDefaultLabels()
        {
            for (int index = 0; index < Controls.Count; ++index)
            {
                var ib = Controls[index] as ItemViewBase;

                if (ib.FormItem is IDefaultLabel)
                {
                    ib.DefaultLabel = ib.FormItem.Form.GetDefaultLabel(ib.FormItem);
                }
            }
        }

        private void displayConditionallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFormItem selectedItem = null;

            foreach (Control c in Controls)
            {
                var ib = c as ItemViewBase;

                if (ib.Selected)
                {
                    selectedItem = ib.FormItem;
                    break;
                }
            }

            if (selectedItem != null)
            {
                var dialog = new FormItemConditionalDisplayView(selectedItem);
                dialog.Show(this);
            }
        }

        #region FormItemContainer Events And Overrides

        private static readonly string emptyMsg = "Drag items from the palette on the left to create your form";
        private bool allowSetFocus;

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is ItemViewBase)
            {
                e.Control.MouseDown += itemBase_MouseDown;

                if (addedToFormByUser)
                {
                    e.Control.Layout += userAddControlLayout;
                }
            }
        }

        private void userAddControlLayout(object sender, EventArgs e)
        {
            addedToFormByUser = false;

            var itemBase = sender as ItemViewBase;

            if (itemBase != null)
            {
                itemBase.Layout -= userAddControlLayout;
                itemBase.AfterAddedToFormByUser();
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            if (e.Control is ItemViewBase)
            {
                e.Control.MouseDown -= itemBase_MouseDown;
            }
        }

        /// <summary>
        /// Something being dragged just entered the control, if we can convert the data
        /// to something recognizable then respond that we allow a Drag copy operation.
        /// </summary>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(FormItemPalette.DragFormat))
            {
                drgevent.Effect = DragDropEffects.Copy;
            }
            else if (drgevent.Data.GetDataPresent(typeof(FormItem)))
            {
                drgevent.Effect = DragDropEffects.Move;
            }

            if (drgevent.Effect != DragDropEffects.None)
            {
                // reset the scroll position
                if (AutoScrollPosition.X != 0)
                {
                    AutoScrollPosition = new Point(0, AutoScrollPosition.Y);
                    PerformLayout();
                }
            }
            base.OnDragEnter(drgevent);
        }

        /// <summary>
        /// Something continues to be dragged over the control, if we can convert the data
        /// to something recognizable then respond that we allow a Drag copy operation.
        /// </summary>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            PublicDragOver(drgevent);
            base.OnDragOver(drgevent);
        }

        /// <summary>
        /// Dragged object is leaving us; re-layout and invalidate to remove insertion cursor
        /// </summary>
        protected override void OnDragLeave(EventArgs e)
        {
            PublicDragLeave(e);
            base.OnDragLeave(e);
        }

        /// <summary>
        /// Something was dropped on the control
        /// </summary>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            PublicDragDrop(drgevent);
            base.OnDragDrop(drgevent);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            int verticalScrollBarWidth = SystemInformation.VerticalScrollBarWidth;

            var newBounds = new ItemBounds(getMaximumItemSize(ClientSize.Width), this);

            if (needToAddVerticalScrollbar(newBounds, this))
            {
                newBounds = new ItemBounds(getMaximumItemSize(ClientSize.Width - verticalScrollBarWidth), this);
            }
            else if (needToRemoveVerticalScrollbar(newBounds, this))
            {
                newBounds = new ItemBounds(getMaximumItemSize(ClientSize.Width + verticalScrollBarWidth), this);
            }

            setItemBounds(newBounds);

            base.OnLayout(e);

            // REVISIT: Without this extra call to setItemBounds, a Form item following a TextItem can get "compressed" vertically
            // when entering text into the TextItem with scroll bars present. Don't know why this fixes that. - SB 11/08/2006
            setItemBounds(newBounds);
        }

        private Size getMaximumItemSize(int requiredWidth)
        {
            return new Size(Math.Max(requiredWidth, ItemViewBase.MinWidth), 0);
        }

        private bool needToAddVerticalScrollbar(ItemBounds itemBounds, ScrollableControl scrollableControl)
        {
            return (!scrollableControl.VerticalScroll.Visible && (itemBounds.TotalHeight > scrollableControl.Height));
        }

        private bool needToRemoveVerticalScrollbar(ItemBounds itemBounds, ScrollableControl scrollableControl)
        {
            return (scrollableControl.VerticalScroll.Visible && (itemBounds.TotalHeight <= scrollableControl.Height));
        }

        private void setItemBounds(ItemBounds newBounds)
        {
            for (int index = 0; index < Controls.Count; ++index)
            {
                var ib = Controls[index] as ItemViewBase;
                ib.Bounds = newBounds.BoundingRectangles[index];
            }
        }

        /// <summary>
        /// User clicked outside of any item so make sure none is active
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            ParentForm.ActiveControl = null;
            ActiveControl = null;

            DeselectAll(null);

            Focus(); // so the Item Container can get Focus for handling keystrokes like Ctrl+V

            ((MDIFormView)ParentForm).TargetTextEditor = null;

            pasteToolStripMenuItem.Enabled = true; // reset this so Ctrl+V keystroke will still work
        }

        /// <summary>
        /// Overridden to draw informational text if there are no form items (the Controls collection is empty)
        /// or draw drag insertion points if necessary
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            base.OnPaint(e);

            g.FillRectangle(SystemBrushes.Control, ClientRectangle);

            if (Controls.Count == 0)
            {
                RectangleF r = ClientRectangle;
                r.Inflate(-8, -8);
                g.DrawString(emptyMsg, emptyMsgFont, Brushes.Blue, r, drawFormat);
            }

            if (dragInsertBefore == Controls.Count)
            {
                // 3 cases for drawing insertion marker
                // draw on FormItemContainer after last control
                // draw on ItemViewBase control (formItem) directly (property set on ItemViewBase control)
                // draw on FormItemContainer when there are no controls (at y=0) the default

                int y = 0;
                if (Controls.Count > 0)
                {
                    Control c = Controls[DragInsertBefore - 1];
                    y = c.Location.Y + c.Height;
                }

                g.DrawImage(Resources.Form_ItemDragInsert, 0, y);
            }
        }

        protected override void Select(bool directed, bool forward)
        {
            // do not select child
            // NOTE:  subverting directed breaks standard Container behavior for child controls
            directed = false;
            base.Select(directed, forward);
        }

        private void setFocusToChild(ItemViewBase item)
        {
            allowSetFocus = true;
            item.Focus();
        }

        protected override void WndProc(ref Message m)
        {
            // Eat WM_SETFOCUS as default implementation will in turn set focus to child control as this is a container control

            if (m.Msg == Win32.WM_SETFOCUS) // serious offender (KM 20060802)
            {
                if (allowSetFocus)
                {
                    allowSetFocus = false;
                }
                else
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Class to calculate and maintain bounding rectangles for all items in item container.
        /// </summary>
        private class ItemBounds
        {
            private readonly Rectangle[] boundingRectangles;
            private readonly int totalHeight;

            public ItemBounds(Size maxSize, ScrollableControl scrollableControl)
            {
                boundingRectangles = new Rectangle[scrollableControl.Controls.Count];

                int y = scrollableControl.AutoScrollPosition.Y;

                int index = 0;

                foreach (ItemViewBase item in scrollableControl.Controls)
                {
                    Size size = item.GetPreferredSize(maxSize);
                    boundingRectangles[index++] = new Rectangle(new Point(0, y), size);
                    totalHeight += size.Height;
                    y += size.Height;
                }

                totalHeight += scrollableControl.AutoScrollMargin.Height;
            }

            public Rectangle[] BoundingRectangles { get { return boundingRectangles; } }

            public int TotalHeight { get { return totalHeight; } }
        }

        #endregion

        #region ItemViewBase Events

        private void itemBase_MouseDown(object sender, MouseEventArgs e)
        {
            pasteToolStripMenuItem.Enabled = true; // reset this so Ctrl+V keystroke will still work

            var item = ((ItemViewBase)sender);
            int index = Controls.GetChildIndex(item, false);

            if (e.Button == MouseButtons.Right && item.Selected)
            {
                return;
            }

            if (ModifierKeys == Keys.Control)
            {
                if (selectionAnchor < 0 || selectionAnchor >= Controls.Count)
                {
                    selectionAnchor = index;
                }

                // don't change other selections
                item.Selected = !item.Selected;
            }
            else if (ModifierKeys == Keys.Shift)
            {
                if (selectionAnchor < 0 || selectionAnchor >= Controls.Count)
                {
                    selectionAnchor = index;
                }

                // select from the anchor to here
                int selTo = index;
                int selFrom = selectionAnchor;

                if (selFrom > selTo)
                {
                    int tmp = selTo;
                    selTo = selFrom;
                    selFrom = tmp;
                }

                for (int i = 0; i < Controls.Count; ++i)
                {
                    Control c = Controls[i];
                    if (c is ItemViewBase)
                    {
                        ((ItemViewBase)c).Selected = (i >= selFrom && i <= selTo);
                    }
                }
            }
            else
            {
                // deselect all but this and set Anchor
                SetAnchor(item);
                item.Refresh();
            }
        }

        #endregion

        #region Context Menu Events

        /// <summary>
        /// Set the enable state of the context menu items when the context menu pops up.
        /// </summary>
        private void contextMenuStrip_Popup(object sender, EventArgs e)
        {
            cutToolStripMenuItem.Enabled = copyToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = ((IEditMenu)this).CanCopy();

            pasteToolStripMenuItem.Enabled = false;

            IDataObject dataObject = Clipboard.GetDataObject();

            if (dataObject != null && dataObject.GetDataPresent(typeof(FormItemList)))
            {
                pasteToolStripMenuItem.Enabled = true;
            }

            displayConditionallyToolStripMenuItem.Enabled = false;

            foreach (Control c in Controls)
            {
                var ib = c as ItemViewBase;

                if (ib.Selected)
                {
                    if (ib is McqItemView || ib is FibItemView || ib is TextItemView || ib is HeadingView)
                    {
                        displayConditionallyToolStripMenuItem.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Cut one or more form items to the clipboard
        /// </summary>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a FormItemList to hold the cut form items
            FormItemList cutList = Cut();

            // Put the FormItemList on the clipboard
            if (cutList.Count > 0)
            {
                Clipboard.SetDataObject(cutList);
                pasteToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Copy one or more form items to the clipboard
        /// </summary>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a FormItemList to hold the copied form items
            FormItemList copyList = Copy();

            // Put the FormItemList on the clipboard
            if (copyList.Count > 0)
            {
                Clipboard.SetDataObject(copyList);
                pasteToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Paste a form item from the clipboard
        /// </summary>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject dataObject = Clipboard.GetDataObject();

            if (dataObject != null && dataObject.GetDataPresent(typeof(FormItemList)))
            {
                var list = (FormItemList)dataObject.GetData(typeof(FormItemList));

                int pasteBefore = Controls.Count;

                if (ActiveControl != null)
                {
                    pasteBefore = Controls.GetChildIndex(ActiveControl, false);
                }

                foreach (FormItem item in list)
                {
                    form.ItemList.Paste(pasteBefore, item);
                    ++pasteBefore;
                }
            }
        }

        /// <summary>
        /// Handle delete menu item click.
        /// Since this handler may also be invoked by its keyboard shortcut (DEL)
        /// we need to still check ActiveControl even though normally contextMenu_Popup wouldn't
        /// allow the menu item to be enabled if ActiveControl was null.
        /// </summary>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete();
        }

        #endregion

        #region Project Events

        private void project_FormItemAdded(object sender, FormItemEventArgs e)
        {
            if (e.Form == form)
            {
                // If there were zero items on the form, invalidate the control
                // so the message goes away

                if (Controls.Count == 0)
                {
                    Invalidate();
                }

                SuspendLayout();

                ItemViewBase uiItem = ItemViewBase.Create(e.Item, this, e.Order);

                DeselectAll(uiItem);
                uiItem.Focus();

                SetAnchor(uiItem);
                ScrollControlIntoView(uiItem);

                syncDefaultLabels();
                ResumeLayout(true);
                PerformLayout();
            }
        }

        private void project_FormItemRemoved(object sender, FormItemEventArgs e)
        {
            if (e.Form == form)
            {
                var ib = Controls[e.Order] as ItemViewBase;
                Debug.Assert(ib.FormItem == e.Item);
                Controls.Remove(ib);
                syncDefaultLabels();
                PerformLayout();
            }
        }

        #endregion

        #region IEditMenu for Main Form's Edit Menu

        // explicit implementation hides these unless instance cast to interface

        bool IEditMenu.CanCut()
        {
            return ((IEditMenu)this).CanCopy();
        }

        bool IEditMenu.CanCopy()
        {
            foreach (Control c in Controls)
            {
                if (c is ItemViewBase && ((ItemViewBase)c).Selected)
                {
                    return true;
                }
            }
            return false;
        }

        bool IEditMenu.CanPaste()
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            return dataObject != null && dataObject.GetDataPresent(typeof(FormItemList));
        }

        bool IEditMenu.CanDelete()
        {
            return ((IEditMenu)this).CanCopy();
        }

        bool IEditMenu.CanRename()
        {
            return false;
        }

        void IEditMenu.Cut()
        {
            cutToolStripMenuItem_Click(cutToolStripMenuItem, EventArgs.Empty);
        }

        void IEditMenu.Copy()
        {
            copyToolStripMenuItem_Click(copyToolStripMenuItem, EventArgs.Empty);
        }

        void IEditMenu.Paste()
        {
            pasteToolStripMenuItem_Click(pasteToolStripMenuItem, EventArgs.Empty);
        }

        void IEditMenu.Delete()
        {
            Delete();
        }

        void IEditMenu.Rename()
        {
        }

        bool IEditMenu.CanUndo()
        {
            return false;
        }

        bool IEditMenu.CanRedo()
        {
            return false;
        }

        void IEditMenu.Undo()
        {
        }

        void IEditMenu.Redo()
        {
        }

        string IEditMenu.UndoActionText { get { return ""; } }

        string IEditMenu.RedoActionText { get { return ""; } }

        ToolStripMenuItem[] IEditMenu.GetAdditionalMenuItems()
        {
            return null;
        }

        #endregion

        #region Handle Created / Destroyed

        private bool addedToFormByUser;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!DesignMode)
            {
                Project.Events.FormItemAdded += project_FormItemAdded;
                Project.Events.FormItemRemoved += project_FormItemRemoved;

                ParentForm.Activated += ParentForm_Activated;

                Application.Idle += appOnIdle;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!DesignMode)
            {
                Application.Idle -= appOnIdle;

                ParentForm.Deactivate -= ParentForm_Deactivated;

                Project.Events.FormItemAdded -= project_FormItemAdded;
                Project.Events.FormItemRemoved -= project_FormItemRemoved;
            }

            base.OnHandleDestroyed(e);
        }

        private void ParentForm_Activated(object sender, EventArgs e)
        {
            FormItemPalette.FormItemDoubleClick += FormItemPalette_FormItemDoubleClick;
        }

        private void ParentForm_Deactivated(object sender, EventArgs e)
        {
            FormItemPalette.FormItemDoubleClick -= FormItemPalette_FormItemDoubleClick;
        }

        private void FormItemPalette_FormItemDoubleClick(object sender, FormItemPalette.FormItemEventArgs e)
        {
            UserAddFormItem(e.Item);
        }

        internal void UserAddFormItem(FormItem newItem)
        {
            addedToFormByUser = true;
            form.ItemList.Add(newItem);
        }

        #endregion
    }
}