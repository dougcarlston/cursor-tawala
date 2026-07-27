// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.FormDesigner.FormItemOptions
{
	public partial class TextOptionsView : UserControl, ITextOptionsView
	{
		private ITextOptionsPresenter presenter;

		public TextOptionsView(ITextItem textItem)
		{
			InitializeComponent();

			presenter = new TextOptionsPresenter(this, textItem);

			textBoxTextLabel.TextChanged += textBoxTextLabel_TextChanged;
		}

		#region ITextOptionsView Members

		public string TextLabel
		{
			get
			{
				return textBoxTextLabel.Text;
			}
			set
			{
				textBoxTextLabel.Text = value;
			}
		}

		public string LabelStatusText
		{
			set { labelStatus.Text = value; }
		}

		#endregion

		private void textBoxTextLabel_TextChanged(object sender, EventArgs e)
		{
			if (presenter != null)
			{
				presenter.TextLabelChanged(textBoxTextLabel.Text);
			}
		}
	}
}
