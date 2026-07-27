using System;
using System.Windows.Forms;
using System.Drawing;
using Tawala.Proj;

namespace Tawala.Controls
{
	public abstract class IdentityTextBox : TextBox, IIdentityControl
	{
		string id;

		public IdentityTextBox(string id) : base()
		{
			this.id = id;

			this.AllowDrop = true;
			this.ReadOnly = true;
			this.BackColor = Color.White;

			this.DragEnter += new DragEventHandler(textBox_DragEnter);
			this.DragDrop += new DragEventHandler(textBox_DragDrop);
		}

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Value
		{
			get { return Text; }
		}

		protected void textBox_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		void textBox_DragDrop(object sender, DragEventArgs e)
		{
			Text = DraggedField(e.Data).QualifiedFieldName;
			Focus();
		}

		public abstract bool AcceptsDropOf(IDataObject data);
		public abstract IPaletteField DraggedField(IDataObject data);
	}
}
