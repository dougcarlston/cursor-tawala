// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.FormDesigner;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Interfaces;

namespace Tawala.FormDesigner.Dialogs
{
	public partial class TextItemStylesView : Form, ITextItemStylesView
	{
		private IFormView activeView = null;

		protected TextItemStylesView()
		{
			InitializeComponent();
		}

		public TextItemStylesView(IFormView view)
			: this()
		{
			activeView = view;
		}
		
		private void formItemStylesDialog_Activated(object sender, EventArgs e)
		{
			Application.Idle += new EventHandler(application_Idle);
		}

		void application_Idle(object sender, EventArgs e)
		{
			buttonApplyAll.Enabled = anyStyleSelected();
			buttonApplySelected.Enabled = anyStyleSelected();
		}

		private bool anyStyleSelected()
		{
			return radioButtonTextNormal.Checked || radioButtonTextInstructional.Checked || radioButtonTextError.Checked;
		}

		private bool anyTextItemSelected()
		{
			if (activeView != null)
			{
				return activeView.AnyTextItemSelected();
			}

			return false;
		}

		private bool singleTextItemSelected()
		{
			return activeView.OnlyOneTextItemSelected();
		}

		private void formItemStylesDialog_Load(object sender, EventArgs e)
		{
			showApplyOptions();
			checkStyleButtons();
		}

		private void showApplyOptions()
		{
			if (!anyTextItemSelected())
			{
				buttonApplySelected.Visible = false;
				labelApplyOptions.Text = Properties.Resources.ApplyStyleToAllTextItems;
			}
		}

		private void checkStyleButtons()
		{
			if (singleTextItemSelected())
			{
				string style = activeView.GetStyleOfSelectedTextItem();
				checkRadioButton(radioButtonTextNormal, style == "normal");
				checkRadioButton(radioButtonTextInstructional, style == "instructional");
				checkRadioButton(radioButtonTextError, style == "error");
			}
		}

		private void checkRadioButton(RadioButton button, bool check)
		{
			button.Checked = check;
			if (check && button.Parent != null && button.Parent.Parent != null)
			{
				ScrollableControl sc = button.Parent.Parent as ScrollableControl;
				sc.ScrollControlIntoView(button);
			}
		}

		private bool applyToAll = true;
		private void buttonApplySelected_Click(object sender, EventArgs e)
		{
			applyToAll = false;
		}

		private void buttonApplyAll_Click(object sender, EventArgs e)
		{
			applyToAll = true;
		}


		#region ITextItemStylesView Members

		public bool TextApplyAllSpecified
		{
			get { return applyToAll; }
		}

		public bool TextNormalSpecified
		{
			get { return radioButtonTextNormal.Checked; }
		}

		public bool TextInstructionalSpecified
		{
			get { return radioButtonTextInstructional.Checked; }
		}

		public bool TextErrorSpecified
		{
			get { return radioButtonTextError.Checked; }
		}

		#endregion
	}
}