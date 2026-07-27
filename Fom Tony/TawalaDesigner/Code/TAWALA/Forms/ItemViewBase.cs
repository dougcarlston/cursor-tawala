// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// The base class for a form item control.  Items derived from this type are added
    /// to the FormItemContainer control when building a form.
    /// NOTICE! There are issues with derived controls and the Form Designer if the class or any members are abstract.  So none are.
    /// </summary>
    public class ItemViewBase : UserControl
    {
        public const int MinWidth = 300;
        private static readonly StringFormat labelFormat = new StringFormat();
        private static readonly Dictionary<Type, Type> mapProjectTypesToUITypes = new Dictionary<Type, Type>();
        private static SolidBrush brushLabel;
        private static SolidBrush brushLabelSel;
        private static EditItemLabel editLabel;

        private static int labelWidth = 80;
        private static ToolTip styleTip;
        private string defaultLabel = string.Empty;

        private Rectangle dragBox = Rectangle.Empty;
        private Image dragInsertionMark;
        private IFormItem formItem;
        private Point screenOffset;
        private bool selected;

        /// <summary>
        /// Static constructor for class (rather than instance) initialization
        /// </summary>
        static ItemViewBase()
        {
            mapProjectTypesToUITypes[typeof(TextItem)] = typeof(TextItemView);
            mapProjectTypesToUITypes[typeof(HeadingItem)] = typeof(HeadingView);
            mapProjectTypesToUITypes[typeof(FibItem)] = typeof(FibItemView);
            mapProjectTypesToUITypes[typeof(McqItem)] = typeof(McqItemView);
            mapProjectTypesToUITypes[typeof(BreakItem)] = typeof(BreakItemView);
            mapProjectTypesToUITypes[typeof(SkipInstructionsItem)] = typeof(SkipItemView);
            mapProjectTypesToUITypes[typeof(HiddenField)] = typeof(HiddenFieldView);
            mapProjectTypesToUITypes[typeof(FileUploadItem)] = typeof(FileUploadItemView);

            labelFormat.Alignment = StringAlignment.Center;
            labelFormat.LineAlignment = StringAlignment.Center;

            styleNames.Add("freeform", "Freeform");
            styleNames.Add("leftAlignLabels", "Left justified");
            styleNames.Add("rightAlignLabels", "Right justified");
            styleNames.Add("leftAlignLabelsJustified", "Left justified, Align right side");
            styleNames.Add("rightAlignLabelsJustified", "Right justified, Align right side");
            styleNames.Add("topLabels", "Above");

            // For future use - Edit a label; not implemented yet
        }

        protected ItemViewBase()
        {
            if (editLabel == null)
            {
                editLabel = new EditItemLabel();
            }

            Name = "ItemViewBase";

            BackColor = Color.FromKnownColor(KnownColor.Control);

            // Enable double-buffering for this control (reduces flicker)

            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        public static int LabelWidth { get { return labelWidth; } set { labelWidth = value; } }

        public virtual Brush LabelTextBrush { get { return SystemBrushes.WindowText; } }

        public virtual Font LabelTextFont { get { return Font; } }

        /// <summary>
        /// The default label is assigned by the UI based on position
        /// and item type.  Or in the case of skip and break items
        /// it is hard coded and cannot be changed.
        /// </summary>
        public virtual string DefaultLabel
        {
            get { return defaultLabel; }
            set
            {
                if (defaultLabel != value)
                {
                    defaultLabel = value;
                    Invalidate();
                }
            }
        }

        public Image DragInsertionMark { get { return dragInsertionMark; } set { dragInsertionMark = value; } }

        /// <summary>
        /// The alternate label is assigned by the user and is initially empty.
        /// Ultimately this would be stored as part of the Proj.FormItem.
        /// </summary>
        public virtual string AlternateLabel
        {
            get { return formItem != null ? formItem.AlternateLabel : string.Empty; }
            set
            {
                if (formItem.AlternateLabel != value)
                {
                    formItem.AlternateLabel = value;
                    Invalidate();
                }
            }
        }

        public virtual bool AlternateLabelEditable { get { return true; } }

        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    OnSelectedChanged(EventArgs.Empty);

                    formItem.Selected = selected;
                }
            }
        }

        public IFormItem FormItem { get { return formItem; } }

        public new FormItemContainer Parent { get { return base.Parent as FormItemContainer; } }

        public MDIFormView ParentView { get { return ParentForm as MDIFormView; } }

        /// <summary>
        /// DefaultSize is overriden so that all ItemViewBase derived objects
        //  have the same size in the designer.  At runtime it ultimately becomes irrelevant		
        /// </summary>
        protected override Size DefaultSize { get { return new Size(336, 128); } }

        protected override Padding DefaultMargin { get { return new Padding(0); } }

        protected override Padding DefaultPadding { get { return new Padding(0); } }

        #region ItemViewBase Events and Overrides

        private static readonly Dictionary<string, string> styleNames = new Dictionary<string, string>();

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control != editLabel)
            {
                e.Control.GotFocus += child_GotFocus;
                e.Control.LostFocus += child_LostFocus;
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            if (e.Control != editLabel)
            {
                e.Control.GotFocus -= child_GotFocus;
                e.Control.LostFocus -= child_LostFocus;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            Invalidate(false);
            //selected = false;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Invalidate(false);
            //selected = true;
        }

        /// <summary>
        /// On DoubleClick (or F2) edit the label
        /// </summary>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (AlternateLabelEditable && ModifierKeys == Keys.None)
            {
                editLabel.Show(this);
            }
        }

        /// <summary>
        /// Change the active form item control based on where the user clicks in the container.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            ((MDIFormView)ParentForm).TargetTextEditor = null;

            Size dragSize = SystemInformation.DragSize;
            dragBox = Rectangle.Empty;

            if (Selected && e.Button == MouseButtons.Left)
            {
                dragBox = new Rectangle(new Point(e.X - (dragSize.Width/2), e.Y - (dragSize.Height/2)), dragSize);
            }

            base.OnMouseDown(e);

            if (Selected && e.Button == MouseButtons.Right)
            {
                return;
            }

            ParentForm.ActiveControl = null;
            Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBox != Rectangle.Empty && !dragBox.Contains(e.X, e.Y))
                {
                    // drag only single objects so it should be anchor
                    // deselect all except this when dragging
                    Parent.SetAnchor(this);

                    // The screenOffset is used to account for any desktop bands 
                    // that may be at the top or left side of the screen when 
                    // determining when to cancel the drag drop operation.
                    screenOffset = SystemInformation.WorkingArea.Location;

                    // Proceed with the drag and drop, passing in clone of form item

                    var dataObject = new DataObject();
                    dataObject.SetData(typeof(FormItem), formItem);

                    if (DragDropEffects.Move == DoDragDrop(dataObject, DragDropEffects.Move))
                    {
                        //						Delete();
                    }
                    else
                    {
                        Parent.PerformLayout(); // make sure insertion cursor goes away
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            dragBox = Rectangle.Empty;
            base.OnMouseUp(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            createStyleTip();
            base.OnMouseEnter(e);
        }

        private void createStyleTip()
        {
            destroyStyleTip();

            if (formItem.Style != null)
            {
                styleTip = new ToolTip();
                styleTip.ShowAlways = true;
                styleTip.IsBalloon = true;

                if (styleNames.ContainsKey(formItem.Style))
                {
                    styleTip.SetToolTip(this, string.Format("Style = '{0}'", styleNames[formItem.Style]));
                }
                else
                {
                    string text = formItem.Style;
                    styleTip.SetToolTip(this, string.Format("Style = '{0}{1}'", Char.ToUpperInvariant(text[0]), text.Substring(1)));
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            destroyStyleTip();
            base.OnMouseLeave(e);
        }

        private static void destroyStyleTip()
        {
            if (styleTip != null)
            {
                styleTip.Dispose();
                styleTip = null;
            }
        }

        /// <summary>
        /// Draw the label
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // initialize static for first time if not initialized when painting
            if (brushLabelSel == null)
            {
                brushLabelSel = new SolidBrush(Color.FromArgb(244, 184, 76));
                Color c = SystemColors.Control;
                brushLabel = new SolidBrush(Color.FromArgb(c.R - 16, c.G - 16, c.B - 16));
            }

            Graphics g = e.Graphics;

            var r = new Rectangle(new Point(0, 0), new Size(labelWidth, Height - 1));

            Brush fillBrush = selected ? brushLabelSel : brushLabel;

            FormItemContainer parent = Parent;

            g.FillRectangle(fillBrush, r);

            // Draw focus rectangle around label if this actual control is focused (not a child)

            if (parent != null && parent.ActiveControl == this)
            {
                if (ActiveControl == null || !ActiveControl.Focused)
                {
                    Rectangle inset = r;
                    inset.Inflate(-1, -1);
                    inset.Width -= 1;
                    ControlPaint.DrawFocusRectangle(g, inset);
                }
            }

            // Draw label string if not displaying the label edit control
            if (!Controls.Contains(editLabel))
            {
                string label = AlternateLabel;

                if (AlternateLabel.Length == 0)
                {
                    label = DefaultLabel;
                }

                if (FormItem.HasDisplayConditions)
                {
                    label = "{ " + label + " }";
                }

                g.DrawString(label, LabelTextFont, LabelTextBrush, r, labelFormat);
            }

            // Draw control bottom border including label bottom border
            g.DrawLine(SystemPens.ControlDarkDark, 0, Height - 1, Width, Height - 1);

            // Draw vertical label border
            g.DrawLine(Pens.Black, labelWidth - 1, 0, labelWidth - 1, Height - 2);

            if (dragInsertionMark != null)
            {
                g.DrawImage(dragInsertionMark, 0, 0);
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// When one of this control's children has focus ignore command keys 
        /// for context menu shortcuts, etc.  This prevents keys like Del from being processed
        /// by the FormItemContainer's menu handlers.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ActiveControl != null && (ActiveControl.Focused || ActiveControl.ContainsFocus))
            {
                return false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// OnSelectedChanged is raised when the selected property changes.
        /// This base implementation simply invalidates the control.  
        /// </summary>
        protected virtual void OnSelectedChanged(EventArgs e)
        {
//            ParentView.SetPalette(null);
            Invalidate();
        }

        #endregion

        #region Focus munging

        private bool allowSetFocus;

        protected bool AllowSetFocus { get { return allowSetFocus; } set { allowSetFocus = value; } }

        public void SetFocusToChild(Control c)
        {
            allowSetFocus = true;
            c.Focus();
        }

        protected override void WndProc(ref Message m)
        {
            // Eat WM_SETFOCUS as default implementation will in turn set focus to child control as this is a container control

            if (m.Msg == Win32.WM_SETFOCUS) // serious offender (KM 20060727)
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

        #endregion

        #region Child Control Events

        private void child_GotFocus(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                (Parent).SetAnchor(this);

                Invalidate(false); // to clear focus rectangle
            }
        }

        private void child_LostFocus(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Invalidate(false); // to restore focus rectangle if necessary
            }
        }

        #endregion

        public static ItemViewBase Create(IFormItem fi, Control parent, int order)
        {
            Type t = mapProjectTypesToUITypes[fi.GetType()];
            var newItem = (ItemViewBase)Activator.CreateInstance(t);
            newItem.formItem = fi;

            // FormItemContainer relies on all form items have these properties
            // Z-order determines position on form, in Controls collection and tab order, not TabIndex property
            // This is here and not in the constructor because the derived classes constructor calls designer code
            // which might inadvertently override it.

            newItem.TabStop = true;
            newItem.TabIndex = 0;

            parent.Controls.Add(newItem);
            parent.Controls.SetChildIndex(newItem, order);

            return newItem;
        }

        public void Delete()
        {
            MDIFormView oldView = ParentView;
            Selected = false;
            Parent.Form.ItemList.Remove(formItem);
            formItem.Eliminate();

            if (oldView != null)
            {
                oldView.PerformDelayedLayout();
            }

            Dispose();
        }

        public virtual void AfterAddedToFormByUser()
        {
        }
    }
}