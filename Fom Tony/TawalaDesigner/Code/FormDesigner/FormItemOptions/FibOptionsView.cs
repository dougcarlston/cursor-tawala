// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Proj.Forms.FormItemContents;

namespace Tawala.FormDesigner.FormItemOptions
{
	public partial class FibOptionsView : UserControl, IFibOptionsView
	{
		private IFibOptionsPresenter presenter;

		public FibOptionsView(IFibItem fibItem, IBlank blank)
		{
			InitializeComponent();

			presenter = new FibOptionsPresenter(this, fibItem, blank);

			textBoxQuestionLabel.TextChanged += textBoxQuestionLabel_TextChanged;

			if (blank != null)
			{
				labelBlank.Enabled = true;
				textBoxBlankLabel.Enabled = true;
				checkBoxResponseRequired.Enabled = true;

				textBoxBlankLabel.TextChanged += textBoxBlankLabel_TextChanged;
				checkBoxResponseRequired.CheckedChanged += checkBoxResponseRequired_CheckedChanged;
			}
			else
			{
				labelBlank.Enabled = false;
				textBoxBlankLabel.Enabled = false;
				checkBoxResponseRequired.Enabled = false;
			}
		}

		#region IFibOptionsView Members

		public string QuestionLabel
		{
			get { return textBoxQuestionLabel.Text; }
			set { textBoxQuestionLabel.Text = value; }
		}

		public string SelectedBlankLabel
		{
			get { return textBoxBlankLabel.Text; }
			set { textBoxBlankLabel.Text = value; }
		}

		public bool SelectedBlankRequired
		{
			get { return checkBoxResponseRequired.Checked; }
			set { checkBoxResponseRequired.Checked = value; }
		}

		public string LabelStatusText
		{
			set { labelStatus.Text = value; }
		}

		#endregion

		private void textBoxBlankLabel_TextChanged(object sender, EventArgs e)
		{
			presenter.BlankLabelChanged(textBoxBlankLabel.Text);
		}

		private void textBoxQuestionLabel_TextChanged(object sender, EventArgs e)
		{
			presenter.QuestionLabelChanged(textBoxQuestionLabel.Text);
		}

		void checkBoxResponseRequired_CheckedChanged(object sender, EventArgs e)
		{
			presenter.BlankResponseRequiredChanged(checkBoxResponseRequired.Checked);
		}

	}
}
