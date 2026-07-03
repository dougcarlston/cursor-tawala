// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Collections.ObjectModel;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Functions.Runtime;

namespace Tawala.Interfaces
{
	public interface IFormPresenter 
	{
		IFormView View { get; }
		IForm Form { get; set; }

		void ViewInitializationCompleted();
		void ViewClosed();

		void InsertHeadingItem(int index);
		
		void InsertTextItem(int index);
		void InsertFibItem(int index);
		void InsertBlank(string fibHtmlId);
		void InsertMcqItem(int index);
		void InsertFieldItem(int index);
		void InsertBreakItem(int index);
		void InsertSkipItem(int index);

		void InsertField(IField field);
		void InsertFunction(IFunction function);
		void InsertImage(string imageName);

        string CreateFormPreviewAnchor(string firstVisibleFormItemId);

        void CopyFormItems(Collection<string> ids);
		void CutFormItems(Collection<string> ids);
		void DeleteFormItems(Collection<string> ids);

		bool CanPasteFormItems { get; }
		void PasteFormItems();

		bool ContainsTextItem(Collection<string> ids);
		bool ContainsOnlyOneTextItem(Collection<string> ids);
		void SetStyleOfTextItems(Collection<string> ids, string style);
		void SetStyleOfAllTextItems(string style);
		string GetStyleOfSelectedTextItem(Collection<string> ids);

		bool ContainsFibItem(Collection<string> ids);
		bool ContainsOnlyOneFibItem(Collection<string> ids);
		bool SelectedFibItemsHaveSameStyle(Collection<string> ids);
		void SetStyleOfFibItems(Collection<string> ids, string style);
		void SetStyleOfAllFibItems(string style);
		string GetStyleOfFirstSelectedFibItem(Collection<string> ids);

		bool ContainsMcqItem(Collection<string> ids);
		bool ContainsOnlyOneMcqItem(Collection<string> ids);
		void SetStyleOfMcqItems(Collection<string> ids, string style);
		void SetStyleOfMcqItems(Collection<string> ids, string style, int columnCount);
		void SetStyleOfAllMcqItems(string style);
		void SetStyleOfAllMcqItems(string style, int columnCount);
		string GetStyleOfSelectedMcqItem(Collection<string> ids);
		int GetColumnCountOfSelectedMcqItem(Collection<string> ids);

		void FormItemsMoved();

		void UpdateMcqChoicesInView(string htmlId);

		void SetItemOptions(string htmlId, Control c);
		void SetMcqItemOptions(IMcqItem mcqItem, IMcqOptionsView mcqOptionsView);
		void UpdateMcqOptionsInView(Control c);

		IFormItem GetFormItem(string htmlId);

		string GetUniqueId();
		string GetBlankId(string blankHtml);

		void MakeFormActiveComponent();
		void SynchronizeFormItems();
	}
}
