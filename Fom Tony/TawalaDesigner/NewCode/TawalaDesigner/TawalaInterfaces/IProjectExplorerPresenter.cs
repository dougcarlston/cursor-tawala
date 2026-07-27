// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System.Collections.ObjectModel;
using Tawala.Projects.Processes;

namespace Tawala.Interfaces
{
	public interface IProjectExplorerPresenter
	{
		IProjectComponentDesigner CurrentDesigner { get; }

		void StartingPointToggleRequested();
		bool SelectedFormIsStartingPoint { get; }

        void PrePopulateToggleRequested();
        bool SelectedFormIsPrePopulated { get; }

		Collection<string> GetFormNames();
		IFormView GetFormView(string formName);

		int GetConnectedFormCount(IProcess process);

		void NewFormRequested();
		void FormSelected(IFormView formView);
		void FormDeselected();
		bool FormRenameRequested(IFormView formView, string newFormName);
		bool CanCutForm(IFormView formView);
		bool CanCopyForm(IFormView formView);
		bool CanPasteForm { get; }
		bool CanDeleteForm(IFormView formView);
		void FormCutRequested(IFormView formView);
		void FormCopyRequested(IFormView formView);
		void FormPasteRequested();
		bool CanPasteComponent { get; }
		void ComponentPasteRequested();
		void FormDeleteRequested(IFormView formView);
		bool FormIsSelected { get; }
        string GetFormDataSourceName { get; }
        void RequestChangeDataSourceName(IFormView formView, string newDataSourceName);

		void NewDocumentRequested();
		void DocumentSelected(IDocumentView documentView);
		void DocumentDeselected();
		bool DocumentRenameRequested(IDocumentView documentView, string newDocumentName);
		bool CanCutDocument(IDocumentView documentView);
		bool CanCopyDocument(IDocumentView documentView);
		bool CanPasteDocument { get; }
		bool CanDeleteDocument(IDocumentView documentView);
		void DocumentCutRequested(IDocumentView documentView);
		void DocumentCopyRequested(IDocumentView documentView);
		void DocumentPasteRequested();
		void DocumentDeleteRequested(IDocumentView documentView);
		bool DocumentIsSelected { get; }

		void NewProcessRequested();
		void ProcessSelected(IProcessView processView);
		void ProcessDeselected();
		Collection<string> GetProcessNames();
		bool ProcessRenameRequested(IProcessView processView, string newProcessName);
		bool CanCutProcess(IProcessView processView);
		bool CanCopyProcess(IProcessView processView);
		bool CanPasteProcess { get; }
		bool CanDeleteProcess(IProcessView processView);
		void ProcessCutRequested(IProcessView processView);
		void ProcessCopyRequested(IProcessView processView);
		void ProcessPasteRequested();
		void ProcessDeleteRequested(IProcessView processView);
		bool ProcessIsSelected { get; }

		void PreProcessConnectionRequested(IFormView formView, string processName);
		void PreProcessDisconnectionRequested(IFormView formView);
		bool CanConnectPreProcess(IFormView formView);
		bool CanDisconnectPreProcess(IFormView formView);

		void PostProcessConnectionRequested(IFormView formView, string processName);
		void PostProcessDisconnectionRequested(IFormView formView);
		bool CanConnectPostProcess(IFormView formView);
		bool CanDisconnectPostProcess(IFormView formView);

		void ProjectOpenRequested(string projectFilePath);
		void ProjectTemplateOpenRequested(string projectFilePath);
		void ProjectSaveRequested(string projectFilePath);
		bool NewProjectWanted();
		void NewProjectRequested();
		string ProjectFolder { get; }
		string ProjectName { get; }
		string ProjectFilePath { get; set; }

		bool ProjectHasBeenModified { get; }
		bool ProjectCloseRequested();

		void EditComponentName();

		bool ProjectExplorerHasFocus { get; }
	}
}
