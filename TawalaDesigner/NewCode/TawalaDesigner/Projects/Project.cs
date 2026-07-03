// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Images;
using Tawala.Projects.Processes;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    public class Project
    {
        private const string xmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n";
        public const string XmlFormatVersion = "1.11";

        private const string xmlRequestEndTag = "</request>\r\n";
        private const string xmlRequestStartTag = "<request type=\"uploadProject\" protocol=\"1.0\">\r\n";

        private static readonly ModifiedState modifiedState;
        private static readonly ProjectEvents projectEvents = new ProjectEvents();

        private static int nextUniqueID = 1;

        private readonly Variable privateInvitationInviteeID = new Variable(Resources.PrivateInvitationVariableLabel, true);

        private readonly DocumentList realOrVirtualDocumentList = new DocumentList();
        private readonly DocumentList virtualDocumentList = new DocumentList();

        private FormList allForms;
        private DocumentList documentList = new DocumentList();
        private FormList formList = new FormList();
        private int nextDocument = 1;
        private int nextForm = 1;
        private int nextProcess = 1;
        private PageHeader pageHeader = new PageHeader();
        private ProcessList processList = new ProcessList();
        private string themePath = "default";

        /// <summary>
        /// static class constructor
        /// </summary>
        static Project()
        {
            FieldMapById = new ProjectFieldMapById();
            FieldMapByName = new ProjectFieldMapByName();
            InvitationMapById = new ProjectInvitationMapById();
            FunctionMapById = new ProjectFunctionMapById();
            NewImages = new ImageDefinitionCollection();
            modifiedState = new ModifiedState();
        }

        /// <summary>
        /// Private Project constructor.  Use static methods.
        /// </summary>
        private Project()
        {
            Current = this;

            Name = Resources.ProjectDefaultName;

            formList.EnableEvents = true;
            documentList.EnableEvents = true;
            processList.EnableEvents = true;

            // a new project is empty so have old and new hashes the same as we don't need to save the new
            // empty project -- we only care if the user modifies it.

            modifiedState.ResetToUnmodified();
        }

        public static ProjectFieldMapById FieldMapById { get; private set; }
        public static ProjectFieldMapByName FieldMapByName { get; private set; }
        public static ProjectFunctionMapById FunctionMapById { get; private set; }
        public static ProjectInvitationMapById InvitationMapById { get; private set; }

        public static Project Current { get; private set; }

        public static bool CreatingProject { get; set; }

        public static ProjectEvents Events
        {
            get { return projectEvents; }
        }

        public bool Modified
        {
            get { return modifiedState.Test(); }
        }

        public static int NextUniqueID
        {
            get { return nextUniqueID++; }
        }

        public PageHeader PageHeader
        {
            get { return pageHeader; }
        }

        public ReadOnlyCollection<IForm> FormList
        {
            get { return formList.AsReadOnly(); }
        }

        public FormList StartingPointList
        {
            get
            {
                var startingPointList = new FormList();

                foreach (IForm form in formList)
                {
                    if (form.StartingPoint)
                    {
                        startingPointList.Add(form);
                    }
                }

                return startingPointList;
            }
        }

        public FormList AllForms
        {
            get
            {
                if (allForms == null)
                {
                    allForms = createCombinedFormList();
                }
                else if (allForms.Count == 0)
                {
                    allForms = createCombinedFormList();
                }

                return allForms;
            }
        }

        public string ThemePath
        {
            get { return themePath; }
            set
            {
                bool changed = themePath.CompareTo(value) != 0;
                themePath = value;

                if (changed)
                {
                    Events.RaiseThemeChangedEvent();
                }
            }
        }

        public IComponent CurrentComponent { get; private set; }

        public string GlobalFibItemStyle { get; set; }
        public string GlobalMCItemStyle { get; set; }
        public int GlobalMCItemColumnCount { get; set; }
        public string GlobalTextItemStyle { get; set; }
        public string Name { get; set; }

        public IList<IDocument> DocumentList
        {
            get { return documentList.AsReadOnly(); }
        }

        public IList<IDocument> RealOrVirtualDocumentList
        {
            get { return realOrVirtualDocumentList.AsReadOnly(); }
        }

        public static IImageDefinitionCollection NewImages { get; private set; }

        /// <summary>
        /// Return a readonly IList wrapper around the processList.
        /// </summary>
        public ReadOnlyCollection<Process> ProcessList
        {
            get { return processList.AsReadOnly(); }
        }

        public VariableList AllVariables
        {
            get
            {
                var allVariables = new VariableList();

                foreach (Process proc in ProcessList)
                {
                    foreach (Variable var in proc.Variables)
                    {
                        allVariables.AddUnique(var);
                    }
                }

                foreach (IForm form in FormList)
                {
                    foreach (IFormItem item in form.ItemList)
                    {
                        if (item is ISkipInstructionsItem)
                        {
                            foreach (Variable var in ((ISkipInstructionsItem)item).Instructions.Variables)
                            {
                                allVariables.AddUnique(var);
                            }
                        }
                    }
                }

                allVariables.AddUnique(privateInvitationInviteeID);

                return allVariables;
            }
        }

        public static void New()
        {
            resetStaticMembers();

            new Project();

            raiseNewProjectEvents();
        }

        public static void Create(IXmlElement element)
        {
            resetStaticMembers();

            new Project();

            Current.Name = element.GetAttribute("name");
            Current.ThemePath = element.GetAttribute("themePath");

            NewImages = new ImageDefinitionCollection(element.GetChild("images"));

            Current.SetFormList(new FormList(element.GetChild("forms")));

            foreach (IForm form in Current.FormList)
            {
                form.ItemList.ResolveFunctionReferences();
            }

            Current.SetDocumentList(new DocumentList(element.GetChild("documents")));
            Current.updateRealOrVirtualDocumentList();

            Current.SetProcessList(new ProcessList(element.GetChild("processes")));

            foreach (IForm form in Current.FormList)
            {
                form.ResolveProcessReferences();
                form.ItemList.ResolveFieldReferences();
            }

            foreach (IForm form in Current.FormList)
            {
                form.ItemList.ResolveFieldReferences();
            }

            Current.EnableComponentEvents(true);

            Current.ResetModifiedState();
        }

        private static void resetStaticMembers()
        {
            nextUniqueID += 1000;

            FieldMapById = new ProjectFieldMapById();
            FieldMapByName = new ProjectFieldMapByName();
            FunctionMapById = new ProjectFunctionMapById();
            InvitationMapById = new ProjectInvitationMapById();
            NewImages = new ImageDefinitionCollection();
        }

        private static void raiseNewProjectEvents()
        {
            projectEvents.RaiseNewProjectEvent(new ProjectEventArgs(Current.Name));
            projectEvents.RaiseCurrentComponentSetEvent(new ComponentEventArgs(null));
        }

        public static bool Open(string filePath)
        {
            CreatingProject = true;
            New();
            Create(new XmlElement(File.ReadAllText(filePath)));
            CreatingProject = false;
            return true;
        }

        /// <summary>
        /// When saving to a file, use this method.
        /// </summary>
        public static void Save(string path)
        {
            string oldName = Current.Name;
            string newName = Path.GetFileNameWithoutExtension(path);

            //if (File.Exists(path))
            //{
            //    string backupPath = Path.ChangeExtension(path, Properties.Resources.BackupFileExtension);
            //    backupPath += Path.GetExtension(path);
            //    try
            //    {
            //        File.Copy(path, backupPath, true);
            //    }
            //    catch
            //    {
            //        // do nothing if backup fails; it's not critical
            //    }
            //}

            Current.Name = newName;

            using (Stream stream = File.Create(path))
            {
                save(stream);
            }

            var args = new SaveProjectEventArgs(oldName, newName);

            projectEvents.RaiseSaveProjectEvent(args);

            modifiedState.ResetToUnmodified();
        }

        public static void SaveWithDebugInfo(string path, string debugInfo)
        {
            Save(path);

            using (Stream stream = File.OpenWrite(path))
            {
                stream.Seek(0, SeekOrigin.End);
                byte[] byteArray = Encoding.UTF8.GetBytes("\r\nDEBUG INFO:\r\n" + debugInfo);
                stream.Write(byteArray, 0, byteArray.Length);
            }
        }

        private static void save(Stream stream)
        {
            if (Current != null)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(Current.ToXmlForSaving());
                stream.Write(byteArray, 0, byteArray.Length);
            }
        }

        public void ResetModifiedState()
        {
            modifiedState.ResetToUnmodified();
        }

        // REVISIT: Remove this after Project conversion from XML is in place.
        public void SetFormList(FormList forms)
        {
            formList = forms;
            allForms = null;
        }

        public void SetProcessList(ProcessList processes)
        {
            processList = processes;
        }

        public void SetDocumentList(DocumentList documents)
        {
            documentList = documents;
        }

        private FormList createCombinedFormList()
        {
            var combinedForms = new FormList();
            var localDataSourceNames = new Collection<string>();

            foreach (IForm form in FormList)
            {
                combinedForms.Add(form);
                if (form.IsDataSource)
                {
                    localDataSourceNames.Add(form.DataSourceName);
                }
            }

            foreach (IForm externalForm in FieldProviders.ExternalForms)
            {
                if (!localDataSourceNames.Contains(externalForm.Name))
                {
                    combinedForms.Add(externalForm);
                }
            }

            return combinedForms;
        }

        /// <summary>
        /// Enables or disables raising of Component events
        /// </summary>
        public void EnableComponentEvents(bool enable)
        {
            formList.EnableEvents = enable;
            documentList.EnableEvents = enable;
            processList.EnableEvents = enable;
        }

        /// <summary>
        /// Create new form and add to end of form list.
        /// </summary>
        public IForm AddForm()
        {
            string name;
            do
            {
                // create a name consisting of the base name plus the Form ID
                int formNum = nextForm++;
                name = Resources.FormDefaultBaseName + " " + formNum;
            } while (formList.IndexOf(name) >= 0); // making sure it doesn't already exist

            IForm form = ComponentMaker.MakeFormObject(name);

            // if only form in project, make it a starting point
            if (formList.Count == 0)
            {
                form.StartingPoint = true;
            }

            allForms = null;

            formList.Add(form);

            return form;
        }

        public IForm GetForm(string name)
        {
            return formList[name];
        }

        public IForm GetFormContaining(IFormItem item)
        {
            foreach (IForm form in AllForms)
            {
                if (form.ItemList.Contains(item))
                {
                    return form;
                }
            }

            return NullObjects.Form;
        }

        public bool PasteForm(IForm form)
        {
            bool successful = formList.Paste(form);

            if (successful)
            {
                allForms = null;
            }

            return successful;
        }

        public bool RenameForm(string oldName, string newName)
        {
            return formList.Rename(oldName, newName);
        }

        public void SetCurrentComponent(IComponent component)
        {
            if (CurrentComponent != component)
            {
                IComponent newComp = null;

                if (formList.ContainsComponent(component))
                {
                    newComp = component;
                }
                else if (processList.ContainsComponent(component))
                {
                    newComp = component;
                }
                else if (documentList.ContainsComponent(component))
                {
                    newComp = component;
                }

                CurrentComponent = newComp;

                Events.RaiseCurrentComponentSetEvent(new ComponentEventArgs(CurrentComponent));
            }
        }

        public void RemoveForm(string name)
        {
            IForm form = formList[name];

            if (form != null)
            {
                allForms = null;
                formList.Remove(form);
            }
        }

        public string ToXmlForSaving()
        {
            var xmlString = new StringBuilder();

            xmlString.Append(xmlDeclaration);

            xmlString.Append(toLeanXml());

            return xmlString.ToString();
        }

        public void SetAllFibStyles(string style)
        {
            GlobalFibItemStyle = style;

            foreach (IForm form in AllForms)
            {
                foreach (IFormItem formItem in form.ItemList)
                {
                    var fibItem = formItem as IFibItem;

                    if (fibItem != null)
                    {
                        fibItem.Style = style;
                    }
                }
            }

            refreshFormPreview();
        }

        /// <summary>
        /// Sets the styles of all MCQs in all forms in the project to the specified style.
        /// </summary>
        public void SetAllMCQStyles(string style)
        {
            GlobalMCItemStyle = style;

            foreach (IMcqItem mcItem in getAllMCItems())
            {
                mcItem.Style = style;
                mcItem.ColumnCount = 0;
            }

            refreshFormPreview();
        }

        /// <summary>
        /// Sets the styles of all MCQs in all forms in the project to the specified style and column count.
        /// </summary>
        public void SetAllMCQStyles(string style, int columnCount)
        {
            GlobalMCItemStyle = style;
            GlobalMCItemColumnCount = columnCount;

            foreach (IMcqItem mcItem in getAllMCItems())
            {
                mcItem.Style = style;
                mcItem.ColumnCount = columnCount;
            }

            refreshFormPreview();
        }

        private Collection<IMcqItem> getAllMCItems()
        {
            var mcItems = new Collection<IMcqItem>();

            foreach (IForm form in AllForms)
            {
                foreach (IFormItem formItem in form.ItemList)
                {
                    var mcItem = formItem as IMcqItem;

                    if (mcItem != null)
                    {
                        mcItems.Add(mcItem);
                    }
                }
            }

            return mcItems;
        }

        /// <summary>
        /// Sets the styles of all text Items in all forms in the project to the specified style.
        /// </summary>
        public void SetAllTextItemStyles(string style)
        {
            GlobalTextItemStyle = style;

            foreach (IForm form in AllForms)
            {
                foreach (IFormItem formItem in form.ItemList)
                {
                    var textItem = formItem as ITextItem;

                    if (textItem != null)
                    {
                        textItem.Style = style;
                    }
                }
            }

            refreshFormPreview();
        }

        /// <summary>
        /// Sets the styles of selected FIBs in all forms in the project to the specified style.
        /// </summary>
        public void SetSelectedFibStyles(string style)
        {
            foreach (IForm form in AllForms)
            {
                foreach (IFormItem formItem in form.ItemList)
                {
                    var fibItem = formItem as IFibItem;

                    if (fibItem != null)
                    {
                        if (fibItem.Selected)
                        {
                            fibItem.Style = style;
                        }
                    }
                }
            }

            refreshFormPreview();
        }

        public void SetSelectedMCQStyles(string style)
        {
            foreach (IMcqItem mcItem in getSelectedMCItems())
            {
                mcItem.Style = style;
                mcItem.ColumnCount = 0;
            }

            refreshFormPreview();
        }

        public void SetSelectedMCQStyles(string style, int columnCount)
        {
            foreach (IMcqItem mcItem in getSelectedMCItems())
            {
                mcItem.Style = style;
                mcItem.ColumnCount = columnCount;
            }

            refreshFormPreview();
        }

        private IList<IMcqItem> getSelectedMCItems()
        {
            var mcItems = new Collection<IMcqItem>();

            foreach (IForm form in AllForms)
            {
                foreach (IFormItem formItem in form.ItemList)
                {
                    var mcItem = formItem as IMcqItem;

                    if (mcItem != null)
                    {
                        if (mcItem.Selected)
                        {
                            mcItems.Add(mcItem);
                        }
                    }
                }
            }

            return mcItems;
        }

        public void SetSelectedTextItemStyles(string style)
        {
            foreach (IForm form in AllForms)
            {
                foreach (IFormItem formItem in form.ItemList)
                {
                    var textItem = formItem as ITextItem;

                    if (textItem != null)
                    {
                        if (textItem.Selected)
                        {
                            textItem.Style = style;
                        }
                    }
                }
            }

            refreshFormPreview();
        }

        private static void refreshFormPreview()
        {
            Events.RaiseThemeChangedEvent();
        }

        public string ToXmlForUpload(string credentialsXml)
        {
            var xmlString = new StringBuilder();

            xmlString.Append(xmlDeclaration);
            xmlString.Append(xmlRequestStartTag);

            xmlString.Append(credentialsXml);

            xmlString.Append(toLeanXml());

            xmlString.Append(xmlRequestEndTag);

            return xmlString.ToString();
        }

        public string ToXml()
        {
            return toLeanXml();
        }

        private string stylesXml()
        {
            var xmlString = new StringBuilder();

            if (GlobalFibItemStyle != null || GlobalMCItemStyle != null || GlobalTextItemStyle != null)
            {
                xmlString.Append(@"<styles");

                if (GlobalFibItemStyle != null)
                {
                    xmlString.AppendFormat(@" fibItemStyle=""{0}""", GlobalFibItemStyle);
                }

                if (GlobalMCItemStyle != null)
                {
                    xmlString.AppendFormat(@" mcItemStyle=""{0}""", GlobalMCItemStyle);
                }

                if (GlobalTextItemStyle != null)
                {
                    xmlString.AppendFormat(@" textItemStyle=""{0}""", GlobalTextItemStyle);
                }

                xmlString.Append(@" />");
            }

            return xmlString.ToString();
        }

        private string toLeanXml()
        {
            projectEvents.RaiseSynchronizeProjectEvent();

            const string xmlProjectEndTag = "</project>\r\n";
            const string xmlProjectStartTag =
                "<project name=\"{0}\" themePath=\"{1}\" format=\"" + XmlFormatVersion + "\" designerBuild=\"{2}\">\r\n";

            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlProjectStartTag, XMLStringFormatter.EscapeAttributeText(Name),
                                   XMLStringFormatter.EscapeAttributeText(themePath), Config.Build);

            xmlString.Append(pageHeader.ToXml());

            xmlString.Append(stylesXml());

            xmlString.Append(formList.ToXml());

            xmlString.Append(processList.ToXml());
            xmlString.Append(documentList.ToXml());

            xmlString.Append(NewImages.ToXml());

            xmlString.Append(xmlProjectEndTag);

            return xmlString.ToString();
        }

        public IDocument AddDocument()
        {
            string name;
            do
            {
                // create a name consisting of the base name plus the Process ID
                int docID = nextDocument++;
                name = Resources.DocumentDefaultBaseName + " " + docID;
            } while (documentList.IndexOf(name) >= 0); // making sure it doesn't already exist

            IDocument doc = ComponentMaker.MakeDocumentObject(name);

            documentList.Add(doc);
            updateRealOrVirtualDocumentList();

            return doc;
        }

        public void AddDocument(IDocument doc)
        {
            documentList.Add(doc);
            updateRealOrVirtualDocumentList();
        }

        public IDocument GetDocument(string name)
        {
            return (documentList[name] ?? NullObjects.Document);
        }

        public bool PasteDocument(IDocument doc)
        {
            bool result = documentList.Paste(doc);
            if (result)
            {
                updateRealOrVirtualDocumentList();
            }
            return result;
        }

        public void RemoveDocument(string name)
        {
            // remove document from list
            IDocument doc = documentList[name];
            if (doc != null)
            {
                documentList.Remove(doc);
                updateRealOrVirtualDocumentList();
            }
        }

        public void RemoveDocument(IDocument doc)
        {
            documentList.Remove(doc);
            updateRealOrVirtualDocumentList();
        }

        public bool RenameDocument(string oldName, string newName)
        {
            bool result = documentList.Rename(oldName, newName);
            if (result)
            {
                updateRealOrVirtualDocumentList();
            }
            return result;
        }

        public IDocument GetRealOrVirtualDocument(string name, bool createNewVirtual)
        {
            IDocument doc = documentList[name] ?? virtualDocumentList[name];

            if (doc == null && createNewVirtual)
            {
                doc = ComponentMaker.MakeDocumentObject(name);
                virtualDocumentList.Add(doc);
                updateRealOrVirtualDocumentList();
            }

            return doc;
        }

        private void updateRealOrVirtualDocumentList()
        {
            Debug.Assert(realOrVirtualDocumentList != null);

            realOrVirtualDocumentList.Clear();

            foreach (IDocument doc in documentList)
            {
                realOrVirtualDocumentList.Add(doc);
            }
            foreach (IDocument doc in virtualDocumentList)
            {
                realOrVirtualDocumentList.Add(doc);
            }

            Events.RaiseDocumentChangedEvent(new ComponentEventArgs(NullObjects.Document));
        }

        public Process AddProcess()
        {
            string name;
            do
            {
                // create a name consisting of the base name plus the Process ID
                int procID = nextProcess++;
                name = Resources.ProcessDefaultBaseName + " " + procID;
            } while (processList.IndexOf(name) >= 0); // making sure it doesn't already exist

            var process = new Process(name);

            // add process to list
            processList.Add(process);

            return (process);
        }

        public Process AddProcess(Process process)
        {
            if (processList.IndexOf(process.Name) == -1)
            {
                processList.Add(process);
            }

            return (process);
        }

        public Process GetProcess(string name)
        {
            return processList[name] ?? Process.NULL;
        }

        public Process GetProcessOrSkipInstructions(ProcessStatement statement)
        {
            Process process = processList[statement];

            if (process == null)
            {
                foreach (IForm form in formList)
                {
                    if ((process = form.GetSkipInstructions(statement)) != null)
                    {
                        break;
                    }
                }
            }

            return process;
        }

        public bool PasteProcess(Process process)
        {
            return processList.Paste(process);
        }

        public void RemoveProcess(string name)
        {
            Process process = processList[name];
            if (process != null)
            {
                processList.Remove(process);

                formList.DisconnectProcessFromAllForms(process);
            }
        }

        public bool RenameProcess(string oldName, string newName)
        {
            return processList.Rename(oldName, newName);
        }

        public void ConnectProcessToForm(IProcess process, string form)
        {
            formList.ConnectProcessToForm(process, form);
        }

        public void DisconnectProcessFromForm(string form)
        {
            formList.DisconnectProcessFromForm(form);
        }

        public void ConnectPreProcessToForm(Process process, string formName)
        {
            IForm form = GetForm(formName);

            if (form != null)
            {
                form.ConnectedPreProcess = process;
            }
        }

        public void DisconnectPreProcessFromForm(string formName)
        {
            IForm form = GetForm(formName);

            if (form != null)
            {
                form.ConnectedPreProcess = null;
            }
        }

        public FormList GetFormList(Process process)
        {
            var fList = new FormList();

            foreach (IForm f in formList)
            {
                if (f.ConnectedProcess == process)
                {
                    fList.Add(f);
                }
            }

            return fList;
        }

        public FormList GetFormList(string processName)
        {
            var fList = new FormList();

            foreach (IForm f in formList)
            {
                if (f.ConnectedProcess != null)
                {
                    if (f.ConnectedProcess.Name == processName)
                    {
                        fList.Add(f);
                    }
                }
            }

            return fList;
        }

        public string GetDefaultLabel(IFormItem formItem)
        {
            IForm form = GetFormContaining(formItem);

            return form.GetDefaultLabel(formItem);
        }

        public IFormItem GetFormItem(int formItemId)
        {
            foreach (IForm f in formList)
            {
                IFormItem item = f.GetFormItem(formItemId);

                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        public int GetFormConnectionCount(IProcess process)
        {
            int count = 0;

            if (process != null)
            {
                foreach (IForm f in formList)
                {
                    if (f.ConnectedProcess == process)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public bool ValidComponentName(string name)
        {
            string testName = name.Trim();

            if (testName.Length == 0)
            {
                return false;
            }

            return ValidLabelFormat(testName);
        }

        public bool ValidDocumentNameForAppendStatement(string name)
        {
            return ValidComponentName(name);
        }

        public bool AlternateLabelExists(string name)
        {
            foreach (IForm form in formList)
            {
                foreach (IFormItem compItem in form.ItemList)
                {
                    if (compItem.AlternateLabel.Length > 0 &&
                        compItem.AlternateLabel == name)
                    {
                        return true;
                    }

                    var fibItem = compItem as IFibItem;
                    if (fibItem != null)
                    {
                        foreach (IBlank compBlank in fibItem.BlankList)
                        {
                            if (compBlank.AlternateLabel.Length > 0 &&
                                compBlank.AlternateLabel == name)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool ValidLabelFormat(string label)
        {
            bool validFormat = true;

            if (label.Length > 0)
            {
                // the following formats are illegal:
                //	 1) leading double underscores (these denote reserved internal labels)
                //	 2) labels that contain a colon (:)
                //	 3) positive or negative integer or floating point numbers
                //			e.g. "3", "37.5", "-90"
                //	 4) positive or negative floating point numbers of the form "n."
                //			e.g. "5.", "-5."
                //
                validFormat =
                    /*1*/ !(label.StartsWith("__")) &&
                          /*2*/ !(label.Contains(":")) &&
                          /*3*/ !(Regex.Match(label, @"^-?\d*\.?\d+$").Success) &&
                          /*4*/ !(Regex.Match(label, @"^-?\d+\.?$").Success);
            }

            return validFormat;
        }

        public bool ValidFieldLabelFormat(string label)
        {
            if (ValidLabelFormat(label) && label.Length > 0)
            {
                // default Question or Text label formats (including lower case, to avoid confusion) are illegal
                //		e.g., "Q1", "T2", "q22", "t1"
                //
                return !(Regex.Match(label, @"^[QqTt]\d+$").Success);
            }

            return false;
        }

        #region Nested type: ModifiedState

        /// <summary>
        /// Encapsulates the managing the modification state of the Project
        /// </summary>
        private class ModifiedState
        {
            private static readonly SHA1Managed sha1Hasher = new SHA1Managed();
            private byte[] newHash;
            private byte[] oldHash;

            internal void ResetToUnmodified()
            {
                oldHash = newHash = hash();
            }

            internal bool Test()
            {
                // we only check the hashes to see if they have changed if they are currently equal
                if (hashEqual())
                {
                    newHash = hash();
                }

                return !hashEqual();
            }

            /// <summary>
            /// Force Test() to always return true (for instance after XML conversion)
            /// </summary>
            internal void ForceTrue()
            {
                oldHash = new byte[1] {1};
                newHash = hash();
            }

            /// <summary>
            /// Compute is 160 bit hash for the binary stream -- used as a modification flag
            /// Hopefully its not too expensive
            /// </summary>
            private byte[] hash()
            {
                using (var ms = new MemoryStream())
                {
                    save(ms);
                    ms.Position = 0;
                    return sha1Hasher.ComputeHash(ms);
                }
            }

            private bool hashEqual()
            {
                if (oldHash.Length != newHash.Length)
                {
                    return false;
                }

                for (int i = 0; i < oldHash.Length; ++i)
                {
                    if (oldHash[i] != newHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        #endregion
    }
}