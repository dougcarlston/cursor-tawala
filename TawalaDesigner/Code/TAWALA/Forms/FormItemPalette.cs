// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// The Palette for Form Items that the user drags to the View's Items control.
    /// This appears as part of Tawala.Forms.View.
    /// </summary>
    public partial class FormItemPalette : UserControl
    {
        public static readonly string DragFormat = DataFormats.GetFormat("PaletteDrag").Name;

        // When dragging a Palette item, dragObject refers to an instance of PaletteDrag that represents the item being dragged.
        // It comes from the Label that was clicked on Tag property.
        private static FormItemCreator dragObject;

        private static EventHandler<FormItemEventArgs> singleDoubleClickListener;

        public FormItemPalette()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Use the Tag property of the Label control to hold info about what is being dragged.

            labelTextItem.Tag = new FormItemCreator(typeof(TextItem));
            labelHeadingItem.Tag = new FormItemCreator(typeof(HeadingItem));
            labelBlankItem.Tag = new FormItemCreator(typeof(FibItem));
            labelChoiceItem.Tag = new FormItemCreator(typeof(McqItem));
            labelBreakItem.Tag = new FormItemCreator(typeof(BreakItem));
            labelSkipItem.Tag = new FormItemCreator(typeof(SkipInstructionsItem));
            labelHiddenFieldItem.Tag = new FormItemCreator(typeof(HiddenField));
            labelFileUploadItem.Tag = new FormItemCreator(typeof(FileUploadItem));

            TabStop = false;
        }

        public static FormItemCreator DragObject { get { return dragObject; } }

        public static event EventHandler<FormItemEventArgs> FormItemDoubleClick
        {
            add { singleDoubleClickListener = value; }
            remove
            {
                if (singleDoubleClickListener == value)
                {
                    singleDoubleClickListener = null;
                }
            }
        }

        /// <summary>
        /// As a drag occurs over other windows, the drag source, in this case the Palette receives GiveFeedback events.  
        /// </summary>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
        {
            Point pt = PointToClient(Cursor.Position);

            // if still over the Palette or the window we are dragging over allows DragDropEffects.Copy
            // then display our custom drag cursor.

            if (ClientRectangle.Contains(pt) || gfbevent.Effect == DragDropEffects.Copy)
            {
                Point parentPt = Cursor.Position;
                parentPt.Offset(16, 16);
                parentPt = ParentForm.PointToClient(parentPt);
                DragImage.Enter();
                DragImage.Move(parentPt.X, parentPt.Y);
                // don't use the default cursor
                gfbevent.UseDefaultCursors = false;
                Cursor.Current = Cursors.Arrow;
            }
            else
            {
                DragImage.Leave();
            }
            base.OnGiveFeedback(gfbevent);
        }

        /// <summary>
        /// This method is called to notify the source (the Palette in this case) of the
        /// ongoing status of the drag, if ESC has been pressed, if a drop occurred, etc
        /// The base class handles the ESC case and sets qcdevent.Action to DragAction.Cancel in
        /// that case.  
        /// </summary>
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
        {
            if (qcdevent.Action != DragAction.Continue)
            {
                DragImage.Leave();
                DragImage.End();
                Cursor.Current = Cursors.Default;
            }
            base.OnQueryContinueDrag(qcdevent);
        }

        private FormItemContainer getItemContainer()
        {
            var form = ParentForm.ActiveMdiChild as MDIFormView;

            if (form != null)
            {
                foreach (Control c in form.Controls)
                {
                    if (c is FormItemContainer)
                    {
                        // make sure drag insertion cursor always goes away
                        return c as FormItemContainer;
                    }
                }
            }
            return null;
        }

        #region Label Events

        private Rectangle dragBox = Rectangle.Empty;

        /// <summary>
        /// All the Labels MouseDown events are routed to this one function which initiates a drag operation
        /// by calling DoDragDrop.  The Tag property of the Label must not be null.
        /// </summary>
        private void label_MouseDown(object sender, MouseEventArgs e)
        {
            Size dragSize = SystemInformation.DragSize;
            dragBox = new Rectangle(new Point(e.X - (dragSize.Width/2), e.Y - (dragSize.Height/2)), dragSize);
        }

        /// <summary>
        /// Label Double Clicked.  Just add a new item
        /// </summary>
        private void label_DoubleClick(object sender, EventArgs e)
        {
            var dragInfo = ((Label)sender).Tag as FormItemCreator;
            FormItem newItem = dragInfo.CreateItem();

            if (singleDoubleClickListener != null)
            {
                singleDoubleClickListener(this, new FormItemEventArgs(newItem));
            }
        }

        private void label_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBox != Rectangle.Empty && !dragBox.Contains(e.X, e.Y))
                {
                    // Label will be null if the sender wasn't a Label (the "as" operator doesn't throw whereas casting can if the cast fails)
                    var label = sender as Label;
                    if (label != null)
                    {
                        dragObject = label.Tag as FormItemCreator;
                        if (dragObject != null)
                        {
                            // since we are just dragging within the application we don't 
                            // need the PaletteDrag object to be serializable.  Just say that
                            // the data format is of the type Palette.DragFormat but don't supply
                            // any data.  Any of our controls which see that this format is present
                            // can simple get the Palette's static DragObject property which is non-null
                            // for the duration of the drag.  
                            var dataObj = new DataObject();
                            // Use a DataObject to encapsulate the DataFormat
                            // and specify false for the autoconvert parameter to indicate there are no other
                            // possible formats that the drag data can be converted to.	
                            dataObj.SetData(DragFormat, false, null);
                            // This functions returns when drag and drop has ended.

                            using (var b = (Bitmap)label.Image.Clone())
                            {
                                Focus();

                                b.MakeTransparent(b.GetPixel(0, 0));
                                DragImage.Begin(ParentForm, b, 0, 0);

                                DragImage.Enter();

                                DoDragDrop(dataObj, DragDropEffects.Copy);
                                // no longer dragging anything
                                dragObject = null;

                                DragImage.End();
                            }

                            FormItemContainer ic = getItemContainer();

                            if (ic != null)
                            {
                                // make sure drag insertion cursor always goes away
                                ic.PerformLayout();
                            }
                        }
                    }
                }
            }
        }

        private void label_MouseUp(object sender, MouseEventArgs e)
        {
            dragBox = Rectangle.Empty;
        }

        #endregion

        #region Nested type: FormItemEventArgs

        public class FormItemEventArgs : EventArgs
        {
            private readonly FormItem item;

            public FormItemEventArgs(FormItem newItem)
            {
                item = newItem;
            }

            public FormItem Item { get { return item; } }
        }

        #endregion
    }
}