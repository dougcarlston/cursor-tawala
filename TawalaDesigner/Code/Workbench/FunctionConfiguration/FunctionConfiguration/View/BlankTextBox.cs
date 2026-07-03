using System;
using System.Windows.Forms;
using Tawala.FunctionConfiguration;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public class BlankTextBox : IdentityTextBox
	{
		BlankParameter parameter;

		public BlankTextBox(BlankParameter parameter) : base(parameter.Id)
		{
			this.parameter = parameter;
			this.TextChanged += new EventHandler(textChanged);

			Tag = this.parameter.Value;
			Text = this.parameter.Value.ToString();
		}

		public override bool AcceptsDropOf(IDataObject data)
		{
			bool acceptsDrop = false;

			if (data.GetDataPresent(typeof(IPaletteField)))
			{
				acceptsDrop = (IPaletteField)data.GetData(typeof(IPaletteField)) is Blank;
			}

			return acceptsDrop;
		}

		public override IPaletteField DraggedField(IDataObject data)
		{
			IPaletteField field = null;

			if (data.GetDataPresent(typeof(IPaletteField)))
			{
				field = (IPaletteField)data.GetData(typeof(IPaletteField)) as Blank;
			}

			return field;
		}

		void textChanged(object sender, EventArgs e)
		{
			parameter.Value = Tag as IParameterValue;
		}
	}
}
