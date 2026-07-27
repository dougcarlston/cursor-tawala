using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Proj;
using Tawala.Proj.Factories;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Common;
using Tawala.Functions.Controls;
using Tawala.Functions.ViewPresenter;
using Tawala.Functions.Runtime;
using Tawala.XmlSupport;

namespace Tawala.FormDesigner
{
	public class McqChoicesPresenter : IMcqChoicesPresenter
	{
		private IMcqChoicesView view;
		private IMcqItem mcqItem;
		private IFunction dataSourceFunction;
		private int choiceSourceIndex;

		public McqChoicesPresenter(IMcqChoicesView view, IMcqItem mcqItem)
		{
			this.view = view;
			this.mcqItem = mcqItem;
			this.dataSourceFunction = mcqItem.DataSourceFunction;
			this.choiceSourceIndex = mcqItem.ChoiceSourceIndex;

			view.ChoiceStrings = createChoiceStrings(mcqItem);
			view.SetChoiceSource(mcqItem.ChoiceSourceIndex);
		}

		private static string createChoiceStrings(IMcqItem mcqItem)
		{
			StringBuilder choiceStrings = new StringBuilder();

			foreach (IChoice choice in mcqItem.Choices)
			{
				if (choice.Text.Length > 0)
				{
					choiceStrings.AppendFormat("{0}\r\n", choice.Text);
				}
			}

			return choiceStrings.ToString();
		}

		#region IMcqChoicesPresenter Members

		public void ChoicesAccepted()
		{
			if (choiceSourceIndex == NewMcqItem.DynamicChoices)
			{
				mcqItem.ChoiceContents = new DataProvider(dataSourceFunction);
			}
			else
			{
				string[] choiceStrings = view.ChoiceStrings.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				mcqItem.ChoiceContents = new NewChoiceList(choiceStrings);
			}
		
			mcqItem.ChoiceSourceIndex = choiceSourceIndex;
		}

		public void FieldsPaletteDoubleClicked(IField field)
		{
			view.InsertFieldString(field.FieldString);
		}

		public void FieldDropped(IField field)
		{
			view.InsertFieldString(field.FieldString);
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


		private void functionConfigured(object sender, FunctionConfiguredEventArgs args)
		{
			if (!args.Canceled)
			{
				dataSourceFunction = args.EditedInstance;
			}
		}

		public void ChoiceSourceChanged(int choiceSourceIndex)
		{
			this.choiceSourceIndex = choiceSourceIndex;
			view.EnableChoiceConfiguration(choiceSourceIndex == NewMcqItem.DynamicChoices);
		}

		#endregion
	}
}
