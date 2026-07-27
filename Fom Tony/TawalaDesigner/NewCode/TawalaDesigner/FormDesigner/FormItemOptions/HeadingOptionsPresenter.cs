// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Text.RegularExpressions;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.FormDesigner.FormItemOptions
{
	public class HeadingOptionsPresenter : IHeadingOptionsPresenter
	{
		private IHeadingItem headingItem;
		private IHeadingOptionsView view;

		public HeadingOptionsPresenter(IHeadingOptionsView view, IHeadingItem headingItem)
		{
			this.view = view;
			this.headingItem = headingItem;
			view.HeadingLabel = headingItem.FieldName;
            view.HeadingType = headingItem.HeadingType;
		}

		#region IHeadingOptionsPresenter Members

		public void HeadingLabelChanged(string newLabel)
		{
			string trimmedLabel = newLabel.Trim();

			if (FormItemLabelSupport.IsLegalItemLabel(trimmedLabel, headingItem))
			{
				headingItem.AlternateLabel = trimmedLabel;
				view.LabelStatusText = string.Empty;
			}
			else
			{
				view.LabelStatusText = Properties.Resources.OptionsInvalidItemLabel;
			}
		}

        public void HeadingTypeChanged()
        {
            headingItem.HeadingType = view.HeadingType;
        }

		#endregion
	}
}
