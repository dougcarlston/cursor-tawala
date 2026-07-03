using System;
using System.Windows.Forms;
using Tawala.ConfigurableFunction;
using Tawala.Proj;

namespace Tawala.Controls
{
	public class BlankTextBox : IdentityTextBox
	{
		BlankParameter parameter;

		public BlankTextBox(BlankParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.TextChanged += new EventHandler(textChanged);
		}

		public override bool AcceptsDropOf(IDataObject data)
		{
			return data.GetDataPresent(typeof(Blank));
		}

		public override IPaletteField DraggedField(IDataObject data)
		{
			IPaletteField field = null;

			if (data.GetDataPresent(typeof(Blank)))
			{
				field = (IPaletteField)data.GetData(typeof(Blank));
			}

			return field;
		}

		void textChanged(object sender, EventArgs e)
		{
			parameter.BlankName = Text;
		}
	}
}
