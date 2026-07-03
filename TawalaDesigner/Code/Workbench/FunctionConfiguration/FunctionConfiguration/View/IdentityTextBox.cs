using System;
using System.Windows.Forms;
using System.Drawing;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
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

		public object Value
		{
			get { return Tag; }
		}

		protected void textBox_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		void textBox_DragDrop(object sender, DragEventArgs e)
		{
			IPaletteField field = DraggedField(e.Data);
			Tag = field;
			Text = field.QualifiedFieldName;

			Focus();
		}

		public abstract bool AcceptsDropOf(IDataObject data);
		public abstract IPaletteField DraggedField(IDataObject data);
	}
}
