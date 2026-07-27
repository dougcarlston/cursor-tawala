// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System.Collections.ObjectModel;
using System.Windows.Forms;
using Tawala.Interfaces;
using Tawala.Common;

namespace Tawala.ProjectExplorer
{
	public interface IProjectExplorerView
	{
		IProjectExplorerPresenter Presenter { get; }

		void ClearProject();
		void OpenProjectFile(string projectFilePath);

		void AddForm(IFormView form, bool isStartingPoint);
		bool CanPasteForm { get; }
		void CutForm(string formName);
		void PasteForm(IFormView formView, bool isStartingPoint);
		void DeleteForm(string formName);
		void SelectForm(IFormView formView);
		void DeselectForm();
		void SetStartingPoint(IFormView formView, bool isStartingPoint);
		IFormView GetFormView(string formName);
		Collection<string> GetFormNames();
		object SelectedComponent { get; }

		void AddDocument(IDocumentView documentView);
		bool CanPasteDocument { get; }
		void CutDocument(string documentName);
		void PasteDocument(IDocumentView documentView);
		void DeleteDocument(string documentName);
		void SelectDocument(IDocumentView documentView);
		void DeselectDocument();
		
		void AddProcess(IProcessView processView);
		bool CanPasteProcess { get; }
		void CutProcess(string processName);
		void PasteProcess(IProcessView processView);
		void DeleteProcess(string processName);
		void SelectProcess(IProcessView processView);
		void DeselectProcess();
		Collection<string> GetProcessNames();

		void ConnectPreProcess(IFormView formView, string processName);
		void DisconnectPreProcess(IFormView formView);

		void ConnectPostProcess(IFormView formView, string processName);
		void DisconnectPostProcess(IFormView formView);

		void EditSelectedComponent();
		DialogResult ShowMessageBox(string messageText, string caption);
		SaveFileDialogResult ShowSaveFileDialog();

		bool ProjectExplorerHasFocus { get; }
	}
}
