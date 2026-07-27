using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects.Components;

namespace Tawala.ProjectUI
{
	public interface IDesignerView : IWin32Window
	{
		IDesignerPresenter Presenter { get; }

		void ComponentRemoved(IProjectComponent component);
		void CurrentComponentSet(IProjectComponent component);

		void DestroyMdiChildren();
		void SetUIEnableState(bool enable);
		void ShowProjectPane(bool show);
		void ShowWaitCursor(bool bShow);
		Size Size { get; }
		string Text { get; set; }
	}
}
