// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Functions.Controls;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.FormDesigner
{
	public class McqChoicesPresenter : IMcqChoicesPresenter
	{
		private readonly IMcqItem mcqItem;
		private readonly IMcqChoicesView view;
		private int choiceSourceType;
		private IFunction dataSourceFunction;

		public static IMcqChoicesPresenter NULL = new NullMcqChoicesPresenter();

		public McqChoicesPresenter(IMcqChoicesView view, IMcqItem mcqItem)
		{
			this.view = view;
			this.mcqItem = mcqItem;
			dataSourceFunction = mcqItem.DataSourceFunction;
			choiceSourceType = mcqItem.ChoiceSourceType;

			view.ChoicesHtml = createChoicesHtml(mcqItem);

			view.SetChoiceSource(mcqItem.ChoiceSourceType);
		}

		#region IMcqChoicesPresenter Members

		public void ChoicesAccepted()
		{
			if (choiceSourceType == NewMcqItem.DynamicChoices)
			{
				mcqItem.ChoiceContents = new DataProvider(dataSourceFunction);
			}
			else
			{
				mcqItem.ChoiceContents = new NewChoiceList(view.ChoicesHtml);
			}

			mcqItem.ChoiceSourceType = choiceSourceType;
		}

		public void FieldsPaletteDoubleClicked(IField field)
		{
			view.InsertField(field.QualifiedFieldName);
		}

		public void FieldDropped(IField field)
		{
			view.InsertField(field.FieldString);
		}

		public void ConfigurationRequested()
		{
			if (dataSourceFunction == null)
			{
				ConfigureFunctionDialog.Presenter.CreateFunction(FunctionLoader.Current.Functions["dynamic-mcq"], functionConfigured);
			}
			else
			{
				ConfigureFunctionDialog.Presenter.EditFunction(dataSourceFunction, functionConfigured);
			}
		}


		public void ChoiceSourceChanged(int choiceSourceType)
		{
			this.choiceSourceType = choiceSourceType;
			view.EnableChoiceConfiguration(choiceSourceType == NewMcqItem.DynamicChoices);
		}

		#endregion

		private static string createChoicesHtml(IMcqItem mcqItem)
		{
			var choicesHtml = new StringBuilder();

			foreach (IChoice choice in mcqItem.Choices)
			{
				if (choice.Text.Length > 0)
				{
					string choiceXhtml = choice.ToXhtml(mcqItem);
					string spanInnerHtml = Regex.Match(choiceXhtml, "<span>(.+)</span>").Groups[1].Value;
					string choiceText = spanInnerHtml;
					choicesHtml.AppendFormat("<P>{0}</P>", choiceText);
				}
			}

			return choicesHtml.ToString();
		}

		private void functionConfigured(object sender, FunctionConfiguredEventArgs args)
		{
			if (!args.Canceled)
			{
				dataSourceFunction = args.EditedInstance;
			}
		}
	}

	public class NullMcqChoicesPresenter : IMcqChoicesPresenter
	{
		public void ChoicesAccepted()
		{
		}

		public void FieldsPaletteDoubleClicked(IField field)
		{
		}

		public void FieldDropped(IField field)
		{
		}

		public void ConfigurationRequested()
		{
		}

		public void ChoiceSourceChanged(int choiceSourceType)
		{
		}
	}
}