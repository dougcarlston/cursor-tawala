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
using Tawala.Interfaces;

namespace Tawala.FormDesigner.FormItemOptions
{
	public partial class McqOptionsView : UserControl, IMcqOptionsView
	{
		private IMcqOptionsPresenter presenter;

		public McqOptionsView(IMcqItem mcqItem)
		{
			InitializeComponent();

			presenter = new McqOptionsPresenter(this, mcqItem);

			textBoxQuestionLabel.TextChanged += textBoxQuestionLabel_TextChanged;
			checkBoxResponseRequired.CheckedChanged += checkBoxResponseRequired_CheckedChanged;
			checkBoxSelectMoreThanOne.CheckedChanged += checkBoxSelectMoreThanOne_CheckedChanged;
		}

		private void textBoxQuestionLabel_TextChanged(object sender, EventArgs e)
		{
			presenter.QuestionLabelChanged(textBoxQuestionLabel.Text);
		}

		private void checkBoxResponseRequired_CheckedChanged(object sender, EventArgs e)
		{
			presenter.ResponseRequiredChanged(checkBoxResponseRequired.Checked);
		}

		private void checkBoxSelectMoreThanOne_CheckedChanged(object sender, EventArgs e)
		{
			presenter.SelectMoreThanOneChanged(checkBoxSelectMoreThanOne.Checked);
		}

		#region IMcqOptionsView Members

		public string QuestionLabel
		{
			get { return textBoxQuestionLabel.Text; }
			set { textBoxQuestionLabel.Text = value; }
		}

		public bool SelectMoreThanOne
		{
			get { return checkBoxSelectMoreThanOne.Checked; }
			set { checkBoxSelectMoreThanOne.Checked = value; }
		}

		public bool ResponseRequired
		{
			get { return checkBoxResponseRequired.Checked; }
			set { checkBoxResponseRequired.Checked = value; }
		}

		public IFormItemsPalette GetFormItemsPalette()
		{
			Control c = this.Parent;
			while (c != null)
			{
				if (c is IFormItemsPalette)
				{
					break;
				}

				c = c.Parent;
			}

			return c as IFormItemsPalette;
		}

		public string LabelStatusText
		{
			set { labelStatus.Text = value; }
		}

		#endregion
	}
}
