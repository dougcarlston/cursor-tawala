using System;
using System.Windows.Forms;
using Tawala.Proj;
using Tawala.ConfigurableFunction;

namespace Tawala.Controls
{
	public class FormComboBox : IdentityComboBox
	{
		FormParameter parameter;

		public FormComboBox(FormParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.SelectedIndexChanged += new EventHandler(selectedIndexChanged);
			this.DataSource = Project.Current.FormList;
			this.parameter.FormName = Project.Current.FormList[0].Name;
		}

		void selectedIndexChanged(object sender, EventArgs e)
		{
			forceTextChange();
			parameter.FormName = Text;
		}

	}
}
