using System;
using System.Windows.Forms;
using Tawala.FunctionConfiguration;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public class EnumerationComboBox : IdentityComboBox
	{
		EnumerationParameter parameter;

		public EnumerationComboBox(EnumerationParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.SelectedIndexChanged += new EventHandler(selectedIndexChanged);
			addChoices();
			this.DisplayMember = "Name";
			this.SelectedItem = getSelectedItem();
		}

		private void addChoices()
		{
			foreach (Choice choice in parameter.Choices)
			{
				Items.Add(choice);
			}
		}

		private object getSelectedItem()
		{
			foreach (Choice	choice in parameter.Choices)
			{
				if (choice.Value.ToString() == parameter.Value.ToString())
				{
					return choice;
				}
			}

			return parameter.Choices[0];
		}

		void selectedIndexChanged(object sender, EventArgs e)
		{
			parameter.Value = SelectedItem as IParameterValue;
		}

	}
}
