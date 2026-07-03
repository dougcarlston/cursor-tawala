// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;

namespace Tawala.Interfaces
{
	public interface IDocumentView
	{
		IDocumentPresenter Presenter { get; set; }
		IApplicationView ParentView { get; }
		Form MdiParent { get; set; }
		void Show();
		void Activate();
		void Close();
		event EventHandler Activated;
		string Contents { get; set; }

		void InsertFunction(int id, string text);
		void UpdateFunction(int oldId, int newId, string text);

        void InsertLink(int id, string text);
        void UpdateLink(int id, string text);

		void SetDocumentName(string documentName);
	}
}
