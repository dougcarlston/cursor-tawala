using System;
using System.Windows.Forms;

namespace Tawala.Controls
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

		public string Value
		{
			get { return ""; }
		}

		protected void forceTextChange()
		{
			string tempText = Text;
			Text = "";
			Text = tempText;
		}
	}
}
