// $Workfile: DerivedTxControl.cs $
// $Revision: 29 $	$Date: 4/26/07 2:14p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;

namespace Tawala.TextEditor
{
	public class DerivedTxControl : TXTextControl.TextControl
	{
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			TXTextControl.PageMargins pm = PageMargins;
			int margin = 50;

			ZoomFactor = 100;

			pm.Left = margin;
			pm.Right = margin;
			pm.Top = margin;
			pm.Bottom = margin;

			HideSelection = false;

			FieldCursor = Cursors.Default;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (fixUnexpectedScrollBehavior)
			{
				Application.Idle -= new EventHandler(application_Idle);
			}
			base.OnHandleDestroyed(e);
		}

		internal new TextEdit Parent
		{
			get { return base.Parent as TextEdit; }
			set { base.Parent = value; }
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			TXTextControl.TextField tf = TextFields.GetItem();

			if (tf != null && !tf.Editable)
			{
				TXTextControl.TextChar curPos = TextChars.GetItem(e.Location, true);
				if (curPos != null)
				{
					if (curPos.Number == tf.Start + tf.Length - 1)
					{
						SendKeys.Send("{RIGHT}");
					}
					else if (curPos.Number == tf.Start)
					{
						SendKeys.Send("{LEFT}");
					}
					else if (curPos.Number > tf.Start && curPos.Number < tf.Start + tf.Length - 1) // came from playground app: keep?
					{
						Select(tf.Start, tf.Length - 2);
					}
				}
			}

			base.OnMouseClick(e);
			Parent.HandleChildMouseClick(e);
		}

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			TXTextControl.TextField tf = TextFields.GetItem();
			bool bCallBase;

			if (tf != null && tf.Editable)
			{
				bCallBase = true;
			}
			else
			{
				if (tf != null)
				{
					bCallBase = handleInROTextField(tf, e);
				}
				else
				{
					bCallBase = handleNearROTextField(tf, e);
				}
			}

			// REVISIT: This realy should be inside the bCallBase condition below
			//			But the MCItemTextEditor need to handle KeyDown.
			Parent.HandleKeyDown(e);

			if (bCallBase)
			{
				base.OnKeyDown(e);
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			Parent.HandleKeyPress(e);
			base.OnKeyPress(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			Parent.HandleKeyUp(e);
			base.OnKeyUp(e);
		}

		private bool handleInROTextField(TXTextControl.TextField tf, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
			{
				e.SuppressKeyPress = true;
				TextFields.Remove(tf);
				return false;
			}
			else if (e.KeyData == Keys.Right)
			{
				Selection.Start = tf.Start + tf.Length - 1;
			}
			else if (e.KeyData == Keys.Left)
			{
				Selection.Start = tf.Start - 1;
				if (tf.Start != 1)
				{
					SendKeys.Send("{RIGHT}");
				}
			}
			return true;
		}

		private bool handleNearROTextField(TXTextControl.TextField tf, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == Keys.Delete)
			{
				if (Selection.Length > 0)
				{
					e.SuppressKeyPress = true;
					DeleteSelection();
					return false;
				}
				else
				{
					foreach (TXTextControl.TextField check in TextFields)
					{
						if (Selection.Start + 1 == check.Start && !check.Editable)
						{
							e.SuppressKeyPress = true;
							SendKeys.Send("{RIGHT}");
							return false;
						}
					}
				}
			}
			else if (e.KeyData == Keys.Back)
			{
				if (Selection.Length > 0)
				{
					e.SuppressKeyPress = true;
					DeleteSelection();
				}
				else
				{
					foreach (TXTextControl.TextField check in TextFields)
					{
						if (check.Editable)
							continue;

						int diff = check.Start + check.Length - Selection.Start;
						if (diff == 1 || diff == 0)
						{
							SendKeys.Send("{LEFT}");
							break;
						}
					}
				}
			}
			return true;
		}

        // Not static in case in future need object because of different dpi
        // REVISIT WARNING - is this calculation correct when OS set to 120 dpi?  If txControl honors that then this will be wrong

		private const int pixelsPerInch = 96;
		private const int twipsPerInch = 1440;

        internal int TwipsToPixels(int twips)
        {
			return twips * pixelsPerInch / twipsPerInch;
        }

        internal double TwipsToInches(int twips)
        {
			return twips / (double)twipsPerInch; 
        }

		public int PixelsToTwips(int pixels)
		{
			return pixels * twipsPerInch / pixelsPerInch;
		}

        private double[] getTabsInInches()
        {
            double[] dtwips = new double[0];

            if (Selection.ParagraphFormat != null && Selection.IsCommonValueSelected(TXTextControl.ParagraphFormat.Attribute.TabPositions))
            {
                int[] twips = Selection.ParagraphFormat.TabPositions;

                int count = 0;
                for (int i = 0; i < twips.Length; ++i)
                {
                    if (twips[i] != 0)
                        ++count;
                }

                dtwips = new double[count];

                for (int i = 0, j = 0; i < twips.Length; ++i)
                {
                    if (twips[i] != 0)
                    {
                        dtwips[j++] = TwipsToInches(twips[i]);
                    }
                }
            }

            return dtwips;
        }

        private void setTabsInInches(double[] inches)
        {
            Selection.ParagraphFormat.ResetTabPositions();
            Selection.ParagraphFormat.ResetTabTypes();

            int[] newTabs = new int[TXTextControl.ParagraphFormat.MaxTabs];

            for (int i = 0; i < inches.Length && i < newTabs.Length; ++i)
            {
               newTabs[i]  = (int)(inches[i] * 1440.0);
            }

            Selection.ParagraphFormat.TabPositions = newTabs;
        }

        internal double[] TabsInInches
        {
            get
            {
                return getTabsInInches();
            }
            set
            {
                setTabsInInches(value);
            }
        }

		internal void DeleteSelection()
		{
			deleteSelectedTextFields();
			deleteSelectedTextAndImages();
		}

		private void deleteSelectedTextFields()
		{
			TXTextControl.TextField selectedTextField = TextFields.GetItem();

			Collection<TXTextControl.TextField> selectedTextFields = new Collection<TXTextControl.TextField>();

			if (isOnlyTextFieldSelected(selectedTextField))
			{
				TextFields.Remove(selectedTextField);
				return;
			}
			else
			{
				foreach (TXTextControl.TextField textField in TextFields)
				{
					if (selectionEncompassesTextField(textField))
					{
						selectedTextFields.Add(textField);
					}
				}
			}

			foreach (TXTextControl.TextField tf in selectedTextFields)
			{
				TextFields.Remove(tf);
			}

			return;
		}

		private bool isOnlyTextFieldSelected(TXTextControl.TextField textField)
		{
			return (textField != null);
		}

		private bool selectionEncompassesTextField(TXTextControl.TextField textField)
		{
			int selStart = Selection.Start + 1;
			int selEnd = selStart + Selection.Length;
			int tfStart = textField.Start;
			int tfEnd = tfStart + textField.Length;

			if (selStart <= tfStart && selEnd > tfStart)
			{
				return true;
			}

			return false;
		}

		private void deleteSelectedText()
		{
			Selection.Text = "";
		}

		private void deleteSelectedTextAndImages()
		{
			Clear();
		}

		protected override void OnTextFieldChanged(TXTextControl.TextFieldEventArgs e)
		{
			base.OnTextFieldChanged(e);

			if (e.TextField.Text.Length == 0)
			{
				TextFields.Remove(e.TextField);
			}
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			Parent.HandleDragEnter(e);
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			Parent.HandleDragOver(drgevent);
		}

		protected override void OnDragLeave(EventArgs e)
		{
			Parent.HandleDragLeave(e);
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			Parent.HandleDragDrop(e);
		}

		protected override void OnTextFieldDoubleClicked(TXTextControl.TextFieldEventArgs e)
		{
			base.OnTextFieldDoubleClicked(e);
			Select(e.TextField.Start, e.TextField.Length - 2);
			Parent.HandleTextFieldDoubleClicked(e.TextField);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
            Parent.HandleChildMouseDown(e);
            base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
            Parent.HandleChildMouseUp(e);
            base.OnMouseUp(e);
		}
		protected override void OnMouseEnter(EventArgs e)
		{
            Parent.HandleChildMouseEnter(e);
            base.OnMouseEnter(e);
		}
		
		protected override void OnMouseLeave(EventArgs e)
		{
            Parent.HandleChildMouseLeave(e);
            base.OnMouseLeave(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
            Parent.HandleChildGotFocus(e);
            base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
            Parent.HandleChildLostFocus(e);
            base.OnLostFocus(e);
		}

		protected override void OnChanged(EventArgs e)
		{
			base.OnChanged(e);
			Parent.OnChanged(e);
		}

        protected override void OnInputPositionChanged(EventArgs e)
        {
            base.OnInputPositionChanged(e);
            Parent.OnInputPositionChanged(e);
        }
		protected override void OnImageDeleted(TXTextControl.ImageEventArgs e)
		{
			base.OnImageDeleted(e);
			Parent.OnChanged(e);
		}

		protected override void OnVScroll(EventArgs e)
		{
			if (fixUnexpectedScrollBehavior && ScrollLocation.Y != 0)
			{
				undesiredVScrollAttempted = true;
				ScrollLocation = new System.Drawing.Point(0, 0);
			}
			else
			{
				base.OnVScroll(e);
			}
		}

		private bool undesiredVScrollAttempted = false;
		private bool fixUnexpectedScrollBehavior = false;

		internal bool FixUnexpectedScrollBehavior
		{
			get { return fixUnexpectedScrollBehavior; }
			set
			{
				fixUnexpectedScrollBehavior = value;
				if (fixUnexpectedScrollBehavior)
				{
					Application.Idle += new EventHandler(application_Idle);
				}
			}
		}

		private void  application_Idle(object sender, EventArgs e)
		{
			if (undesiredVScrollAttempted)
			{
				undesiredVScrollAttempted = false;
				Invalidate();
			}
		}
	}

}
