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
	public partial class HeadingOptionsView : UserControl, IHeadingOptionsView
	{
		private IHeadingOptionsPresenter presenter;

		public HeadingOptionsView(IHeadingItem headingItem)
		{
			InitializeComponent();

			presenter = new HeadingOptionsPresenter(this, headingItem);

			textBoxHeadingLabel.TextChanged += textBoxHeadingLabel_TextChanged;
		}

		#region IHeadingOptionsView Members

		public string HeadingLabel
		{
			get
			{
				return textBoxHeadingLabel.Text;
			}
			set
			{
				textBoxHeadingLabel.Text = value;
			}
		}

		public string LabelStatusText
		{
			set { labelStatus.Text = value; }
		}

		#endregion

		private void textBoxHeadingLabel_TextChanged(object sender, EventArgs e)
		{
			presenter.HeadingLabelChanged(textBoxHeadingLabel.Text);
		}

	}
}
