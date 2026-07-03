using System;
using System.Windows.Forms;
using Tawala.ConfigurableFunction;
using Tawala.Proj;

namespace Tawala.Controls
{
	public class MCQTextBox : IdentityTextBox
	{
		MCQParameter parameter;

		public MCQTextBox(MCQParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.TextChanged += new EventHandler(textChanged);
		}

		public override bool AcceptsDropOf(IDataObject data)
		{
			return data.GetDataPresent(typeof(MCItem));
		}

		public override IPaletteField DraggedField(IDataObject data)
		{
			IPaletteField field = null;

			if (data.GetDataPresent(typeof(MCItem)))
			{
				field = (IPaletteField)data.GetData(typeof(MCItem));
			}

			return field;
		}

		void textChanged(object sender, EventArgs e)
		{
			parameter.MCQName = Text;
		}
	}
}
