// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;

namespace Tawala.Interfaces
{
	public interface IFormItemsPalette : IDesignerItemsPalette
	{
		void UpdateChoiceIconsInFormView(Control c);
	}
}
