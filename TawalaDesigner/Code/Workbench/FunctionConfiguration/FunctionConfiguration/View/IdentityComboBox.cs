using System;
using System.Windows.Forms;

namespace Tawala.FunctionConfiguration
{
	public abstract class IdentityComboBox : ComboBox, IIdentityControl
	{
		string id;

		public IdentityComboBox(string id) : base()
		{
			this.id = id;
			this.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public object Value
		{
			get { return ""; }
		}

	}
}
