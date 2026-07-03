using System;
using System.Windows.Forms;
using Tawala.Proj;
using Tawala.FunctionConfiguration;

namespace Tawala.FunctionConfiguration
{
	public class FormComboBox : IdentityComboBox
	{
		FormParameter parameter;

		public FormComboBox(FormParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.SelectedIndexChanged += new EventHandler(selectedIndexChanged);
			addForms();
			this.SelectedItem = getSelectedItem();
		}

		private void addForms()
		{
			foreach (Tawala.Proj.Form form in Project.Current.FormList)
			{
				Items.Add(form);
			}
		}

		private object getSelectedItem()
		{
			foreach (Tawala.Proj.Form form in Project.Current.FormList)
			{
				if (parameter.Value != null)
				{
					if (form == parameter.Value)
					{
						return form;
					}
				}
			}

			return Project.Current.FormList[0];
		}

		void selectedIndexChanged(object sender, EventArgs e)
		{
			parameter.Value = SelectedItem as IParameterValue;
		}

	}
}
