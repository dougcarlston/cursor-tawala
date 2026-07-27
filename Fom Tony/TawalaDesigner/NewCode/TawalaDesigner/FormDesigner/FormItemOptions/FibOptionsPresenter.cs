// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Text.RegularExpressions;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.FormDesigner.FormItemOptions
{
	public class FibOptionsPresenter : IFibOptionsPresenter
	{
		private IFibOptionsView view;
		private IFibItem fibItem;
		private IBlank blank;

		public FibOptionsPresenter(IFibOptionsView view, IFibItem fibItem, IBlank selectedBlank)
		{
			this.view = view;
			this.fibItem = fibItem;
			this.blank = selectedBlank;

			view.QuestionLabel = fibItem.FieldName;

			if (selectedBlank != null)
			{
				view.SelectedBlankLabel = blank.FieldName;
				view.SelectedBlankRequired = blank.Required;
			}
			else
			{
				view.SelectedBlankLabel = string.Empty;
				view.SelectedBlankRequired = false;
			}
		}

		#region IFibOptionsPresenter Members

		public void QuestionLabelChanged(string newLabel)
		{
			string trimmedLabel = newLabel.Trim();

			if (FormItemLabelSupport.IsLegalItemLabel(trimmedLabel, fibItem as IFormItem))
			{
				fibItem.AlternateLabel = trimmedLabel;
				view.LabelStatusText = string.Empty;
			}
			else
			{
				view.LabelStatusText = Properties.Resources.OptionsInvalidQuestionLabel;
			}
		}

		public void BlankLabelChanged(string newLabel)
		{
			if (blank != null)
			{
				string trimmedLabel = newLabel.Trim();

				if (FormItemLabelSupport.IsLegalBlankLabel(trimmedLabel, blank))
				{
					blank.AlternateLabel = trimmedLabel;
					view.LabelStatusText = string.Empty;
				}
				else
				{
					view.LabelStatusText = Properties.Resources.OptionsInvalidBlankLabel;
				}
			}
		}

		public void BlankResponseRequiredChanged(bool required)
		{
			if (blank != null)
			{
				blank.Required = required;
			}
		}

		#endregion
	}
}
