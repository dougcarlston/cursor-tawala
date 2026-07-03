// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Interfaces;

namespace Tawala.FormDesigner.Dialogs
{
	public partial class McqItemStylesView : Form, IMcqItemStylesView
	{
		private IFormView activeView = null;

		protected McqItemStylesView()
		{
			InitializeComponent();
		}

		public McqItemStylesView(IFormView view)
			: this()
		{
			activeView = view;
		}

		public bool McqApplyAllSpecified
		{
			get { return applyToAll; }
		}

		public bool MCQVerticalSpecified
		{
			get { return radioButtonMCQVertical.Checked; }
		}

		public bool MCQHorizontalSpecified
		{
			get { return radioButtonMCQHorizontal.Checked; }
		}

		public bool MCQMultiColumnSpecified
		{
			get { return radioButtonMCQMultiColumn.Checked; }
		}

		public int MCQMultiColumnCount
		{
			get
			{
				return ((String)comboBoxColumnCount.SelectedItem == "Auto" ? 0 : Convert.ToInt32((String)comboBoxColumnCount.SelectedItem));
			}
		}

		private void FormItemStylesDialog_Activated(object sender, EventArgs e)
		{
			Application.Idle += new EventHandler(application_Idle);
		}

		void application_Idle(object sender, EventArgs e)
		{
			buttonApplyAll.Enabled = anyStyleSelected();
			buttonApplySelected.Enabled = anyStyleSelected();

			comboBoxColumnCount.Enabled = radioButtonMCQMultiColumn.Checked;
		}

		private bool anyStyleSelected()
		{
			return radioButtonMCQVertical.Checked || radioButtonMCQHorizontal.Checked || radioButtonMCQMultiColumn.Checked;
		}

		private bool anyMCQItemSelected()
		{
			if (activeView != null)
			{
				return activeView.AnyMcqItemSelected();
			}

			return false;
		}

		private bool singleMCQItemSelected()
		{
			int count = 0;

			if (activeView != null)
			{
				return activeView.OnlyOneMcqItemSelected();
			}

			return (count == 1);
		}

		private void FormItemStylesDialog_Load(object sender, EventArgs e)
		{
			ShowApplyOptions();
			CheckStyleButtons();
		}

		private void ShowApplyOptions()
		{
			if (!anyMCQItemSelected())
			{
				buttonApplySelected.Visible = false;
				labelApplyOptions.Text = Properties.Resources.ApplyStyleToAllMCQItems;
			}
		}

		private void CheckStyleButtons()
		{
			if (singleMCQItemSelected())
			{
				string style = activeView.GetStyleOfSelectedMcqItem();
				checkRadioButton(radioButtonMCQVertical, style == "vertical");
				checkRadioButton(radioButtonMCQHorizontal, style == "horizontal");
				checkRadioButton(radioButtonMCQMultiColumn, style == "multicolumn");
				setMultiColumnCount();
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

		private void setMultiColumnCount()
		{
			comboBoxColumnCount.SelectedIndex = 0;

			if (activeView.GetStyleOfSelectedMcqItem() == "multicolumn")
			{
				if (activeView.GetColumnCountOfSelectedMcqItem() > 0)
				{
					comboBoxColumnCount.SelectedIndex = activeView.GetColumnCountOfSelectedMcqItem() - 1;
				}
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
	}
}