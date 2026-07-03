using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

using Tawala.Interfaces;
using Tawala.MainApplication;

namespace Tawala.ComponentDesigner
{
	public abstract class ProjectComponentDesigner : IProjectComponentDesigner
	{
		protected void setDesignerPalette()
		{
			if (ApplicationPresenter.DesignerPalette != null)
			{
				if (ItemsPalette != null)
				{
					ApplicationPresenter.DesignerPalette.SuspendLayout();
					ApplicationPresenter.DesignerPalette.Controls.Clear();
					ApplicationPresenter.DesignerPalette.Controls.Add(ItemsPalette as Control);
					ApplicationPresenter.DesignerPalette.ResumeLayout();
					ApplicationPresenter.DesignerPalette.Show();
				}
				else
				{
					ApplicationPresenter.DesignerPalette.Hide();
					ApplicationPresenter.DesignerPalette.Controls.Clear();
				}
			}
		}

		#region IProjectComponentDesigner Members

		public IDesignerItemsPalette ItemsPalette
		{
			get;
			protected set;
		}

		#endregion
	}
}
