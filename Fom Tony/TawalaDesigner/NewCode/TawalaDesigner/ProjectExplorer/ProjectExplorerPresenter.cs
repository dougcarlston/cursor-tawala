// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.DocumentDesigner;
using Tawala.FormDesigner;
using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.ProcessDesigner;
using Tawala.ProjectExplorer.Dialogs;
using Tawala.ProjectExplorer.Properties;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Processes;

namespace Tawala.ProjectExplorer
{
    public class ProjectExplorerPresenter : IProjectExplorerPresenter
    {
        private static IProjectComponentDesigner currentDesigner;
        private static ITawalaDocumentDesigner documentDesigner;
        private static ITawalaFormDesigner formDesigner;
        private static ITawalaProcessDesigner processDesigner;
        private readonly IProjectExplorerView projectExplorerView;
        private string projectFilePath;
        private string projectFolder = Config.DefaultProjectDirectory;

        public ProjectExplorerPresenter(IProjectExplorerView view)
        {
            projectExplorerView = view;
        }

        #region IProjectExplorerPresenter Members

        public IProjectComponentDesigner CurrentDesigner
        {
            get { return currentDesigner; }
        }

        public void NewFormRequested()
        {
            IForm form = Project.Current.AddForm();
            IFormView formView = new FormView(form, ApplicationPresenter.MainApplicationForm, this);
            projectExplorerView.AddForm(formView, form.StartingPoint);

            formView.Activated += projectExplorerPresenter_FormActivated;

            activateFormDesigner(formView);
        }

        public void FormSelected(IFormView formView)
        {
            activateFormDesigner(formView);
            projectExplorerView.SelectForm(formView);
        }

        public void FormDeselected()
        {
            projectExplorerView.DeselectForm();
        }

        public void StartingPointToggleRequested()
        {
            formDesigner.CurrentFormView.Presenter.Form.StartingPoint = !formDesigner.CurrentFormView.Presenter.Form.StartingPoint;
            projectExplorerView.SetStartingPoint(formDesigner.CurrentFormView, formDesigner.CurrentFormView.Presenter.Form.StartingPoint);
        }

        public bool SelectedFormIsStartingPoint
        {
            get { return (currentDesigner == formDesigner && formDesigner.CurrentFormView.Presenter.Form.StartingPoint); }
        }

        public void PrePopulateToggleRequested()
        {
            formDesigner.CurrentFormView.Presenter.Form.DataEntryOnly = !formDesigner.CurrentFormView.Presenter.Form.DataEntryOnly;
            //           projectExplorerView.SetStartingPoint(formDesigner.CurrentFormView, formDesigner.CurrentFormView.Presenter.Form.StartingPoint);
        }

        public bool SelectedFormIsPrePopulated
        {
            get { return (currentDesigner == formDesigner && formDesigner.CurrentFormView.Presenter.Form.DataEntryOnly); }
        }

        public Collection<string> GetFormNames()
        {
            return projectExplorerView.GetFormNames();
        }

        public IFormView GetFormView(string formName)
        {
            return projectExplorerView.GetFormView(formName);
        }

        public int GetConnectedFormCount(IProcess process)
        {
            return Project.Current.GetFormConnectionCount(process);
        }

        public bool FormRenameRequested(IFormView formView, string newFormName)
        {
            bool renameSuccessful = false;

            if (!string.IsNullOrEmpty(newFormName))
            {
                renameSuccessful = Project.Current.RenameForm(formView.Presenter.Form.Name, newFormName);

                if (renameSuccessful)
                {
                    formView.SetFormName(newFormName);
                }
            }

            return renameSuccessful;
        }

        public bool CanCutForm(IFormView formView)
        {
            return FormIsSelected;
        }

        public bool CanCopyForm(IFormView formView)
        {
            return FormIsSelected;
        }

        public bool CanPasteForm
        {
            get { return Clipboard.GetDataObject().GetDataPresent(typeof(IForm)) && projectExplorerView.CanPasteForm; }
        }

        public bool CanDeleteForm(IFormView formView)
        {
            return FormIsSelected;
        }

        public void FormCutRequested(IFormView formView)
        {
            formView.Presenter.SynchronizeFormItems();
            cutFormToClipboard(formView);

            string formName = formView.Presenter.Form.Name;
            projectExplorerView.CutForm(formName);
            Project.Current.RemoveForm(formName);
        }

        public void FormCopyRequested(IFormView formView)
        {
            formView.Presenter.SynchronizeFormItems();
            copyFormToClipboard(formView);
        }

        public void FormPasteRequested()
        {
            IDataObject clipData = Clipboard.GetDataObject();

            if (clipData.GetDataPresent(typeof(IForm)))
            {
                var form = clipData.GetData(typeof(IForm)) as IForm;

                if (Project.Current.PasteForm(form))
                {
                    IFormView formView = new FormView(form, ApplicationPresenter.MainApplicationForm, this);
                    projectExplorerView.PasteForm(formView, form.StartingPoint);

                    formView.Activated += projectExplorerPresenter_FormActivated;

                    activateFormDesigner(formView);
                }
            }
        }

        public bool CanPasteComponent
        {
            get { return CanPasteForm || CanPasteDocument || CanPasteProcess; }
        }

        public void ComponentPasteRequested()
        {
            IDataObject clipData = Clipboard.GetDataObject();

            if (clipData.GetDataPresent(typeof(IForm)))
            {
                FormPasteRequested();
                return;
            }

            if (clipData.GetDataPresent(typeof(NewDocument)))
            {
                DocumentPasteRequested();
                return;
            }

            if (clipData.GetDataPresent(typeof(Process)))
            {
                ProcessPasteRequested();
            }
        }

        public void FormDeleteRequested(IFormView formView)
        {
            string formName = formView.Presenter.Form.Name;
            projectExplorerView.DeleteForm(formName);
            Project.Current.RemoveForm(formName);
        }

        public void EditComponentName()
        {
            projectExplorerView.EditSelectedComponent();
        }

        public bool ProjectExplorerHasFocus
        {
            get { return projectExplorerView.ProjectExplorerHasFocus; }
        }

        public bool FormIsSelected
        {
            get { return (projectExplorerView.SelectedComponent as IFormView) != null; }
        }

        public string GetFormDataSourceName
        {
            get { return ((IFormView)projectExplorerView.SelectedComponent).Presenter.Form.DataSourceName; }
        }

        public void RequestChangeDataSourceName(IFormView formView, string newDataSourceName)
        {
            formView.Presenter.Form.DataSourceName = newDataSourceName;
        }

        public void ProcessDeleteRequested(IProcessView processView)
        {
            string processName = processView.Presenter.Process.Name;
            projectExplorerView.DeleteProcess(processName);
            Project.Current.RemoveProcess(processName);
        }

        public bool ProcessIsSelected
        {
            get { return (projectExplorerView.SelectedComponent as IProcessView) != null; }
        }

        public void DocumentPasteRequested()
        {
            IDataObject clipData = Clipboard.GetDataObject();

            if (clipData.GetDataPresent(typeof(NewDocument)))
            {
                var document = clipData.GetData(typeof(NewDocument)) as IDocument;

                if (Project.Current.PasteDocument(document))
                {
                    IDocumentView documentView = new DocumentView(document, ApplicationPresenter.MainApplicationForm, this);
                    projectExplorerView.PasteDocument(documentView);

                    documentView.Activated += projectExplorerPresenter_DocumentActivated;

                    activateDocumentDesigner(documentView);
                }
            }
        }

        public void DocumentDeleteRequested(IDocumentView documentView)
        {
            string documentName = documentView.Presenter.Document.Name;
            projectExplorerView.DeleteDocument(documentName);
            Project.Current.RemoveDocument(documentName);
        }

        public bool DocumentIsSelected
        {
            get { return (projectExplorerView.SelectedComponent as IDocumentView) != null; }
        }

        public bool ProcessRenameRequested(IProcessView processView, string newProcessName)
        {
            bool renameSuccessful = false;

            if (!string.IsNullOrEmpty(newProcessName))
            {
                renameSuccessful = Project.Current.RenameProcess(processView.Presenter.Process.Name, newProcessName);

                if (renameSuccessful)
                {
                    processView.SetProcessName(newProcessName);
                }
            }

            return renameSuccessful;
        }

        public bool CanCutProcess(IProcessView processView)
        {
            return ProcessIsSelected;
        }

        public bool CanCopyProcess(IProcessView processView)
        {
            return ProcessIsSelected;
        }

        public bool CanPasteProcess
        {
            get { return Clipboard.GetDataObject().GetDataPresent(typeof(Process)) && projectExplorerView.CanPasteProcess; }
        }

        public bool CanDeleteProcess(IProcessView processView)
        {
            return ProcessIsSelected;
        }

        public void ProcessCutRequested(IProcessView processView)
        {
            placeProcessOnClipboard(processView);

            string processName = processView.Presenter.Process.Name;
            projectExplorerView.CutProcess(processName);
            Project.Current.RemoveProcess(processName);
        }

        public void ProcessCopyRequested(IProcessView processView)
        {
            placeProcessOnClipboard(processView);
        }

        public void ProcessPasteRequested()
        {
            IDataObject clipData = Clipboard.GetDataObject();

            if (clipData.GetDataPresent(typeof(Process)))
            {
                var process = clipData.GetData(typeof(Process)) as Process;

                if (Project.Current.PasteProcess(process))
                {
                    IProcessView processView = new ProcessView(process, ApplicationPresenter.MainApplicationForm, this);
                    projectExplorerView.PasteProcess(processView);

                    processView.Activated += projectExplorerPresenter_ProcessActivated;

                    activateProcessDesigner(processView);
                }
            }
        }

        public bool DocumentRenameRequested(IDocumentView documentView, string newDocumentName)
        {
            bool renameSuccessful = false;

            if (!string.IsNullOrEmpty(newDocumentName))
            {
                renameSuccessful = Project.Current.RenameDocument(documentView.Presenter.Document.Name, newDocumentName);

                if (renameSuccessful)
                {
                    documentView.SetDocumentName(newDocumentName);
                }
            }

            return renameSuccessful;
        }

        public bool CanCutDocument(IDocumentView documentView)
        {
            return DocumentIsSelected;
        }

        public bool CanCopyDocument(IDocumentView documentView)
        {
            return DocumentIsSelected;
        }

        public bool CanPasteDocument
        {
            get { return Clipboard.GetDataObject().GetDataPresent(typeof(NewDocument)) && projectExplorerView.CanPasteDocument; }
        }

        public bool CanDeleteDocument(IDocumentView documentView)
        {
            return DocumentIsSelected;
        }

        public void DocumentCutRequested(IDocumentView documentView)
        {
            documentView.Presenter.SynchonizeModelWithView();
            placeDocumentOnClipboard(documentView);

            string documentName = documentView.Presenter.Document.Name;
            projectExplorerView.CutDocument(documentName);
            Project.Current.RemoveDocument(documentName);
        }

        public void DocumentCopyRequested(IDocumentView documentView)
        {
            documentView.Presenter.SynchonizeModelWithView();
            placeDocumentOnClipboard(documentView);
        }

        public void NewDocumentRequested()
        {
            IDocumentView documentView = new DocumentView(Project.Current.AddDocument(), ApplicationPresenter.MainApplicationForm, this);
            projectExplorerView.AddDocument(documentView);

            activateDocumentDesigner(documentView);

            documentView.Activated += projectExplorerPresenter_DocumentActivated;
        }

        public void DocumentSelected(IDocumentView documentView)
        {
            activateDocumentDesigner(documentView);
            projectExplorerView.SelectDocument(documentView);
        }

        public void DocumentDeselected()
        {
            projectExplorerView.DeselectDocument();
        }

        public void NewProcessRequested()
        {
            IProcessView processView = new ProcessView(Project.Current.AddProcess(), ApplicationPresenter.MainApplicationForm, this);
            projectExplorerView.AddProcess(processView);

            activateProcessDesigner(processView);

            processView.Activated += projectExplorerPresenter_ProcessActivated;
        }

        public void ProcessSelected(IProcessView processView)
        {
            activateProcessDesigner(processView);
            projectExplorerView.SelectProcess(processView);
        }

        public void ProcessDeselected()
        {
            projectExplorerView.DeselectProcess();
        }

        public Collection<string> GetProcessNames()
        {
            return projectExplorerView.GetProcessNames();
        }

        public void PreProcessConnectionRequested(IFormView formView, string processName)
        {
            IProcess process = Project.Current.GetProcess(processName);
            IForm form = formView.Presenter.Form;
            form.ConnectedPreProcess = process;

            projectExplorerView.ConnectPreProcess(formView, processName);
        }

        public void PreProcessDisconnectionRequested(IFormView formView)
        {
            IForm form = formView.Presenter.Form;
            form.ConnectedPreProcess = null;

            projectExplorerView.DisconnectPreProcess(formView);
        }

        public bool CanConnectPreProcess(IFormView formView)
        {
            return (Project.Current.ProcessList.Count > 0 && formView.Presenter.Form.ConnectedPreProcess == null);
        }

        public bool CanDisconnectPreProcess(IFormView formView)
        {
            return (formView.Presenter.Form.ConnectedPreProcess != null);
        }

        public void PostProcessConnectionRequested(IFormView formView, string processName)
        {
            IProcess process = Project.Current.GetProcess(processName);
            IForm form = formView.Presenter.Form;
            form.ConnectedProcess = process;

            projectExplorerView.ConnectPostProcess(formView, processName);
        }

        public void PostProcessDisconnectionRequested(IFormView formView)
        {
            IForm form = formView.Presenter.Form;
            form.ConnectedProcess = null;

            projectExplorerView.DisconnectPostProcess(formView);
        }

        public bool CanConnectPostProcess(IFormView formView)
        {
            return (Project.Current.ProcessList.Count > 0 && formView.Presenter.Form.ConnectedProcess == null);
        }

        public bool CanDisconnectPostProcess(IFormView formView)
        {
            return (formView.Presenter.Form.ConnectedProcess != null);
        }

        public void ProjectOpenRequested(string projectFilePath)
        {
            projectFolder = Path.GetDirectoryName(projectFilePath);
            this.projectFilePath = projectFilePath;

            ApplicationPresenter.SetProjectNameInTitleBar(Path.GetFileNameWithoutExtension(projectFilePath));

            openProject(projectFilePath);
        }

        public void ProjectTemplateOpenRequested(string projectFilePath)
        {
            openProject(projectFilePath);

            this.projectFilePath = null;
            ApplicationPresenter.SetProjectNameInTitleBar(Path.GetFileNameWithoutExtension("Untitled"));
            Project.Current.Name = "Untitled";
        }

        public bool NewProjectWanted()
        {
            if (ProjectHasBeenModified)
            {
                DialogResult messageBoxResult = showProjectModifiedMessageBox();

                if (messageBoxResult == DialogResult.Cancel)
                {
                    return false;
                }

                if (messageBoxResult == DialogResult.Yes)
                {
                    SaveFileDialog saveFileDialog = ApplicationPresenter.View.CreateSaveFileDialog();

                    DialogResult saveFileDialogResult = saveFileDialog.ShowDialog(ApplicationPresenter.MainApplicationForm);

                    if (saveFileDialogResult == DialogResult.Cancel)
                    {
                        return false;
                    }

                    ProjectSaveRequested(saveFileDialog.FileName);
                }
            }

            return true;
        }

        public void NewProjectRequested()
        {
            projectExplorerView.ClearProject();
            ApplicationPresenter.SetProjectNameInTitleBar("");

            Project.CreatingProject = true;
            Project.New();
            Project.CreatingProject = false;

            projectFilePath = null;
        }

        public string ProjectFolder
        {
            get { return projectFolder; }
        }

        public string ProjectName
        {
            get { return Project.Current.Name; }
        }

        public string ProjectFilePath
        {
            get { return projectFilePath; }
            set { projectFilePath = value; }
        }

        public void ProjectSaveRequested(string filePath)
        {
            projectFolder = Path.GetDirectoryName(filePath);
            projectFilePath = filePath;
            Project.Current.Name = Path.GetFileNameWithoutExtension(projectFilePath);
            ApplicationPresenter.SetProjectNameInTitleBar(Path.GetFileNameWithoutExtension(projectFilePath));

            if (validateCurrentProject(filePath))
            {
                Project.Save(projectFilePath);
            }
        }

        public bool ProjectCloseRequested()
        {
            if (ProjectHasBeenModified)
            {
                DialogResult messageBoxResult = showProjectModifiedMessageBox();

                if (messageBoxResult == DialogResult.Cancel)
                {
                    return false;
                }

                if (messageBoxResult == DialogResult.Yes)
                {
                    if (projectFilePath == null)
                    {
                        SaveFileDialogResult saveFileDialogResult = showSaveFileDialog();

                        if (saveFileDialogResult.DialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }

                        ProjectSaveRequested(saveFileDialogResult.FileName);
                    }
                    else
                    {
                        ProjectSaveRequested(projectFilePath);
                    }
                }
            }

            return true;
        }

        public bool ProjectHasBeenModified
        {
            get { return Project.Current.Modified; }
        }

        private bool validateCurrentProject(string filePath)
        {
            var validator = new ProjectXmlValidator();
            if (validator.ValidateXML())
                return true;

            string debugFilePath = filePath + "-debug-" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            Project.SaveWithDebugInfo(debugFilePath, validator.Message);

            using (var dialog = new SavedProjectInvalid(debugFilePath))
            {
                dialog.ShowDialog(ApplicationPresenter.MainApplicationForm);
            }

            return false;
        }

        #endregion

        private void projectExplorerPresenter_FormActivated(object sender, EventArgs e)
        {
            activateFormDesigner(sender as IFormView);
        }

        private void activateFormDesigner(IFormView formView)
        {
            if (formDesigner == null)
            {
                formDesigner = new TawalaFormDesigner();
            }

            formDesigner.CurrentFormView = formView;

            currentDesigner = formDesigner;
        }

        private void cutFormToClipboard(IFormView formView)
        {
            var dataObject = new DataObject();
            dataObject.SetData(typeof(IForm), formView.Presenter.Form);
            Clipboard.SetDataObject(dataObject);
        }

        private void copyFormToClipboard(IFormView formView)
        {
            var dataObject = new DataObject();

            IForm sourceForm = formView.Presenter.Form;
            IForm clonedForm = Cloner.Clone(sourceForm);

            clonedForm.ItemList.Clear();

            foreach (IFormItem sourceFormItem in sourceForm.ItemList)
            {
                IFormItem clonedFormItem = Cloner.Clone(sourceFormItem);
                clonedFormItem.ClearId();
                clonedForm.ItemList.Add(clonedFormItem);
            }

            dataObject.SetData(typeof(IForm), clonedForm);
            Clipboard.SetDataObject(dataObject);
        }

        private void placeProcessOnClipboard(IProcessView processView)
        {
            var dataObject = new DataObject();
            dataObject.SetData(processView.Presenter.Process.GetType(), processView.Presenter.Process);
            Clipboard.SetDataObject(dataObject);
        }

        private void placeDocumentOnClipboard(IDocumentView documentView)
        {
            var dataObject = new DataObject();
            dataObject.SetData(documentView.Presenter.Document.GetType(), documentView.Presenter.Document);
            Clipboard.SetDataObject(dataObject);
        }

        private void projectExplorerPresenter_DocumentActivated(object sender, EventArgs e)
        {
            activateDocumentDesigner(sender as IDocumentView);
        }

        private static void activateDocumentDesigner(IDocumentView documentView)
        {
            if (documentDesigner == null)
            {
                documentDesigner = new TawalaDocumentDesigner();
            }

            documentDesigner.CurrentDocumentView = documentView;

            currentDesigner = documentDesigner;
        }

        private void projectExplorerPresenter_ProcessActivated(object sender, EventArgs e)
        {
            activateProcessDesigner(sender as IProcessView);
        }

        private static void activateProcessDesigner(IProcessView processView)
        {
            if (processDesigner == null)
            {
                processDesigner = new TawalaProcessDesigner();
            }

            processDesigner.SetCurrentProcessView(processView);

            currentDesigner = processDesigner;
        }

        private void openProject(string filePath)
        {
            projectExplorerView.ClearProject();

            Project.Open(filePath);

            insertFormFieldsIntoFieldMapByName();

            addFormsToView();
            addDocumentsToView();
            addProcessesToView();
            connectPreProcessesToForms();
            connectPostProcessesToForms();
        }

        private DialogResult showProjectModifiedMessageBox()
        {
            return projectExplorerView.ShowMessageBox(Resources.SaveOnExitNewLoadMessage, Application.ProductName);
        }

        private void insertFormFieldsIntoFieldMapByName()
        {
            foreach (IForm form in Project.Current.FormList)
            {
                insertBlanksIntoFieldMapByName(form);
                insertHiddenFieldsIntoFieldMapByName(form);
                insertMcqItemsIntoFieldMapByName(form);
            }
        }

        private SaveFileDialogResult showSaveFileDialog()
        {
            return projectExplorerView.ShowSaveFileDialog();
        }

        private static void insertBlanksIntoFieldMapByName(IForm form)
        {
            foreach (IFormItem formItem in form.ItemList)
            {
                var fibItem = formItem as IFibItem;

                if (fibItem != null)
                {
                    fibItem.InsertBlanksIntoFieldMapByName();
                }
            }
        }

        private static void insertHiddenFieldsIntoFieldMapByName(IForm form)
        {
            foreach (IFormItem formItem in form.ItemList)
            {
                var hiddenField = formItem as IHiddenField;

                if (hiddenField != null)
                {
                    Project.FieldMapByName.AddUnique(hiddenField);
                }
            }
        }

        private static void insertMcqItemsIntoFieldMapByName(IForm form)
        {
            foreach (IFormItem formItem in form.ItemList)
            {
                var mcqItem = formItem as IMcqItem;

                if (mcqItem != null)
                {
                    Project.FieldMapByName.AddUnique(mcqItem);
                }
            }
        }

        private void addFormsToView()
        {
            foreach (IForm form in Project.Current.FormList)
            {
                IFormView formView = new FormView(form, ApplicationPresenter.MainApplicationForm, this);
                projectExplorerView.AddForm(formView, form.StartingPoint);
            }
        }

        private void connectPreProcessesToForms()
        {
            foreach (IForm form in Project.Current.FormList)
            {
                if (form.ConnectedPreProcess != null)
                {
                    IFormView formView = projectExplorerView.GetFormView(form.Name);
                    projectExplorerView.ConnectPreProcess(formView, form.ConnectedPreProcess.Name);
                }
            }
        }

        private void connectPostProcessesToForms()
        {
            foreach (IForm form in Project.Current.FormList)
            {
                if (form.ConnectedProcess != null)
                {
                    IFormView formView = projectExplorerView.GetFormView(form.Name);
                    projectExplorerView.ConnectPostProcess(formView, form.ConnectedProcess.Name);
                }
            }
        }

        private void addDocumentsToView()
        {
            foreach (IDocument document in Project.Current.DocumentList)
            {
                IDocumentView documentView = new DocumentView(document, ApplicationPresenter.MainApplicationForm, this);
                projectExplorerView.AddDocument(documentView);
            }
        }

        private void addProcessesToView()
        {
            foreach (IProcess process in Project.Current.ProcessList)
            {
                IProcessView processView = new ProcessView(process, ApplicationPresenter.MainApplicationForm, this);
                projectExplorerView.AddProcess(processView);
            }
        }
    }
}