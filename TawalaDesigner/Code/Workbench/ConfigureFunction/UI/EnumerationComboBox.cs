using System;
using System.Windows.Forms;
using Tawala.ConfigurableFunction;

namespace Tawala.Controls
{
	public class EnumerationComboBox : IdentityComboBox
	{
		EnumerationParameter parameter;

		public EnumerationComboBox(EnumerationParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.SelectedIndexChanged += new EventHandler(selectedIndexChanged);
			this.DataSource = this.parameter.Choices;
			this.DisplayMember = "Name";
		}

		void selectedIndexChanged(object sender, EventArgs e)
		{
			forceTextChange();
			parameter.ChoiceName = Text;
		}

	}
}
