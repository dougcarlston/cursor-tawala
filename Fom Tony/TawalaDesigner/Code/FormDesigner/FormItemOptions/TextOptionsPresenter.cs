// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Text.RegularExpressions;
using Tawala.Proj;
using Tawala.Proj.Forms;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Proj.Forms.FormItemContents;

namespace Tawala.FormDesigner.FormItemOptions
{
	public class TextOptionsPresenter : ITextOptionsPresenter
	{
		public TextOptionsPresenter(ITextOptionsView view, ITextItem textItem)
		{
			this.textItem = textItem;
			this.view = view;
			view.TextLabel = textItem.FieldName;
		}

		#region ITextOptionsPresenter Members

		public void TextLabelChanged(string newLabel)
		{
			string trimmedLabel = newLabel.Trim();

			if (FormItemLabelSupport.IsLegalItemLabel(trimmedLabel, textItem))
			{
				textItem.AlternateLabel = trimmedLabel;
				view.LabelStatusText = string.Empty;
			}
			else
			{
				view.LabelStatusText = Properties.Resources.OptionsInvalidItemLabel;
			}
		}

		#endregion

		private ITextItem textItem;
		private ITextOptionsView view;
	}
}
