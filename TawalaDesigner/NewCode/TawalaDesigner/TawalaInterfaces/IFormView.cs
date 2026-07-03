// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Tawala.Interfaces
{
	public interface IFormView : IWin32Window
	{
		IFormPresenter Presenter { get; set; }
		IApplicationView ParentView { get; }
		Form MdiParent { get; set; }
		void Show();
		void Activate();
		void Close();
		event EventHandler Activated;

		string GetSelection();
		void SetSelection(string html);

		int GetInsertionIndex();

		void InsertHeadingItem(string contentsXhtml, int index, int formItemId, string headingType);
		void InsertTextItem(string contentsXhtml, int index, int formItemId);
		void InsertFibItem(string contentsXhtml, int index, int formItemId);
		void InsertMcqItem(string questionXhtml, string choicesXhtml, int index, int formItemId);
		void InsertFieldItem(string name, int index, int formItemId);
		void InsertSkipItem(int index, int formItemId);
		void InsertBreakItem(int index, int formItemId);

		string CreateBlank();
		void InsertBlank(string blankHtml);
		void InsertField(int id, string text);

		void InsertFunction(int id, string text);
		void InsertImage(string imageName);

		void CopySelectedItems();
		void CutSelectedItems();
		void DeleteSelectedItems();
		void PasteItems();

		bool AnyTextItemSelected();
		bool OnlyOneTextItemSelected();
		string GetStyleOfSelectedTextItem();

		bool AnyFibItemSelected();
		bool OnlyOneFibItemSelected();
		bool SelectedFibItemsHaveSameStyle();
		string GetStyleOfFirstSelectedFibItem();

		bool AnyMcqItemSelected();
		bool OnlyOneMcqItemSelected();
		string GetStyleOfSelectedMcqItem();
		int GetColumnCountOfSelectedMcqItem();

		string GetAttribute(string htmlId, string name);
		void SetAttribute(string htmlId, string name, string value);

		void NotifyHtmlDragDropEnded();

		Collection<string> GetFormItemIds();

		HtmlElement GetHtmlElementById(string id);
		HtmlElementCollection GetElementsByTagName(string tagname);
		HtmlElement ActiveFormItem { get; }
		void SetLabelText(int id, string labelText);

		void SetFormName(string formName);
		bool FormItemIsActive { get; set; }
	}
}
