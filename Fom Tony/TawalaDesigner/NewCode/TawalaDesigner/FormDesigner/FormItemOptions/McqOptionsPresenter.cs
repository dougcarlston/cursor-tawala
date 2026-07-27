// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Interfaces;

namespace Tawala.FormDesigner.FormItemOptions
{
	public class McqOptionsPresenter : IMcqOptionsPresenter
	{
		private IMcqItem mcqItem;
		private IMcqOptionsView view;

		public McqOptionsPresenter(IMcqOptionsView view, IMcqItem mcqItem)
		{
			this.view = view;
			this.mcqItem = mcqItem;
			view.QuestionLabel = mcqItem.FieldName;
			view.ResponseRequired = mcqItem.RequireAtLeastOne;
			view.SelectMoreThanOne = !mcqItem.SelectOnlyOne;
		}

		#region IMcqOptionsPresenter Members

		public void QuestionLabelChanged(string newLabel)
		{
			string trimmedLabel = newLabel.Trim();

			if (FormItemLabelSupport.IsLegalItemLabel(trimmedLabel, mcqItem))
			{
				mcqItem.AlternateLabel = trimmedLabel;
				view.LabelStatusText = string.Empty;
			}
			else
			{
				view.LabelStatusText = Properties.Resources.OptionsInvalidQuestionLabel;
			}
		}

		public void ResponseRequiredChanged(bool required)
		{
			mcqItem.RequireAtLeastOne = required;
		}

		public void SelectMoreThanOneChanged(bool selectMoreThanOne)
		{
			mcqItem.SelectOnlyOne = !selectMoreThanOne;

			IFormItemsPalette palette = view.GetFormItemsPalette();
			if (palette != null)
			{
				palette.UpdateChoiceIconsInFormView(view as McqOptionsView);
			}
		}

		#endregion
	}
}
