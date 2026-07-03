using System;
using System.Windows.Forms;
using Tawala.FunctionConfiguration;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public class StringTextBox : IdentityTextBox
	{
		StringParameter parameter;

		public StringTextBox(StringParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.TextChanged += new EventHandler(textChanged);
		}

		public override bool AcceptsDropOf(IDataObject data)
		{
			return false;
		}

		public override IPaletteField DraggedField(IDataObject data)
		{
			return null;
		}

		void textChanged(object sender, EventArgs e)
		{
			parameter.Value = Tag as IParameterValue;
		}
	}
}
