// Copyright © 2005 - 2008  Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;

namespace TawalaDesigner.Dialogs
{
	public interface INewProjectView
	{
		TreeNode CategoryRootNode { get; }
		ListView TemplateView { get; }
		string TemplateDescription { get; set; }
		string SelectedTemplateFile { get; }
	}
}
