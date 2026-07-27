// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Tawala.Projects.Documents;
using Tawala.Projects.Exceptions;
using Tawala.Projects.Expressions;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;
using XmlElement=Tawala.XmlSupport.XmlElement;

namespace Tawala.Projects
{
    public class TawalaProjectConverter
    {
        private static readonly Factory<Condition> conditionFactory = new Factory<Condition>();
        private static Factory<SendStatement> sendStatementFactory = new Factory<SendStatement>();
        private readonly FieldMapper mappedFields = new FieldMapper("TawalaProjectConverter");

        // stream for reading XML
        private readonly Stream xmlStream;

        private ForEachStatement enclosingForEachStatement;

        static TawalaProjectConverter()
        {
            conditionFactory.Register("equals", typeof(FibCondition));
            conditionFactory.Register("doesNotEqual", typeof(FibCondition));
            conditionFactory.Register("contains", typeof(FibCondition));
            conditionFactory.Register("doesNotContain", typeof(FibCondition));
            conditionFactory.Register("beginsWith", typeof(FibCondition));
            conditionFactory.Register("endsWith", typeof(FibCondition));
            conditionFactory.Register("isLessThan", typeof(FibCondition));
            conditionFactory.Register("isLessThanOrEqualTo", typeof(FibCondition));
            conditionFactory.Register("isGreaterThan", typeof(FibCondition));
            conditionFactory.Register("isGreaterThanOrEqualTo", typeof(FibCondition));

            conditionFactory.Register("isBlank", typeof(FibNoExpressionCondition));
            conditionFactory.Register("isNotBlank", typeof(FibNoExpressionCondition));

            conditionFactory.Register("mcEquals", "value", typeof(MCValueCondition));
            conditionFactory.Register("mcDoesNotEqual", "value", typeof(MCValueCondition));
            conditionFactory.Register("mcContains", "value", typeof(MCValueCondition));
            conditionFactory.Register("mcDoesNotContain", "value", typeof(MCValueCondition));
            conditionFactory.Register("mcEquals", typeof(MCFieldCondition));
            conditionFactory.Register("mcDoesNotEqual", typeof(MCFieldCondition));
            conditionFactory.Register("mcContains", typeof(MCFieldCondition));
            conditionFactory.Register("mcDoesNotContain", typeof(MCFieldCondition));

            conditionFactory.Register("mcIsBlank", typeof(MCFieldNoExpressionCondition));
            conditionFactory.Register("mcIsNotBlank", typeof(MCFieldNoExpressionCondition));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TawalaProjectConverter(Stream xmlStream)
        {
            throwIfNotLoadable(xmlStream);
            this.xmlStream = LegacyVariableConverter.Convert(xmlStream);
        }

        /// <summary>
        /// Indicates whether or not the XML file contains forms
        /// </summary>
        public bool HasForms
        {
            get
            {
                XmlTextReader reader = getXmlReader();

                return (reader.ReadToFollowing("forms"));
            }
        }

        private void throwIfNotLoadable(Stream xmlStream)
        {
            xmlStream.Position = 0;

            var xml = new XmlDocument();
            xml.PreserveWhitespace = true;
            xml.Load(xmlStream);

            xmlStream.Position = 0;

            XmlNodeList nodes = xml.SelectNodes("//*[@externalSharedData='true']");

            if (nodes.Count > 0)
            {
                var exception = new ProjectMissingDataSourcesException();
                foreach (XmlNode node in nodes)
                {
                    string name = node.Attributes["name"].Value;
                    if (!FieldProviders.ExternalForms.ContainsComponentNamed(name) && !exception.MissingDataSourceNames.Contains(name))
                    {
                        exception.MissingDataSourceNames.Add(name);
                    }
                }

                if (exception.MissingDataSourceNames.Count > 0)
                {
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Returns an XML reader for reading XML from either a file or a stream
        /// </summary>
        private XmlTextReader getXmlReader()
        {
            XmlTextReader reader = null;
            xmlStream.Seek(0, SeekOrigin.Begin);
            reader = new XmlTextReader(xmlStream);

            return reader;
        }

        /// <summary>
        /// Converts the XML file or stream to a Tawala project
        /// </summary>
        public bool ConvertXmlToProject()
        {
            string projectString = GetProjectString();

            IXmlElement projectXml = new XmlElement(projectString);

            if (xmlFormatNewerThanCurrent(projectXml))
            {
                return false;
            }

            Project.New(projectXml);

			// kludge to reduce load time of large projects
			// jdf - 5/13/10
			Project.LoadingProjectXml = true;

            Project.Current.EnableComponentEvents(false);

            string xmlFormsString = GetFormsString();

            if (xmlFormsString != "")
            {
                IXmlElement formsElement = new XmlElement(xmlFormsString);
                var formList = new FormList(formsElement);

                Project.Current.SetFormList(formList);
            }

            //			ProcessList processes = GetProcesses();

            Collection<string> processNames = GetProcessNames();

            // for each process in XML file...
            for (int i = 0; i < processNames.Count; i++)
            {
                // add process to project
                Process process = Project.Current.AddProcess();
                process.Name = processNames[i];
            }

            Project.Current.EnableComponentEvents(true);

            // for each form in project...
            for (int i = 0; i < Project.Current.FormList.Count; i++)
            {
                // connect process to form
                var form = (Form)Project.Current.FormList[i];
                string processName = GetConnectedProcess(form.Name);
                if (processName != "" && processName != "Null Process")
                {
                    form.ConnectedPostProcess = Project.Current.GetProcess(processName);
                }

                string preProcessName = GetConnectedPreProcess(form.Name);
                if (preProcessName != "" && preProcessName != "Null Process")
                {
                    form.ConnectedPreProcess = Project.Current.GetProcess(preProcessName);
                }
            }

            // for each process in project...
            for (int i = 0; i < Project.Current.ProcessList.Count; i++)
            {
                // build its Variable list now so variables will be available to everyone
                Process process = Project.Current.ProcessList[i];
                setProcessVariables(process);

                addNativeFieldsToMap(process);

                addForeignFieldsToMap(process);

                addVariablesToMap(process);

                // preserve mapped variables for use in Skip Instructions
                mappedFields.Fields.Add(process.Variables);
            }

            mappedFields.Fields.AddUnique(new Variable(Resources.PrivateInvitationVariableLabel));

            // for each form in project...
            foreach (Form form in Project.Current.FormList)
            {
                SetFormItems(form);
            }

            // for each form in project...
            for (int i = 0; i < Project.Current.FormList.Count; i++)
            {
                var form = (Form)Project.Current.FormList[i];

                // get a list of all the Skip Instructions items in the Form
                var skipItems = new FormItemList();
                for (int j = 0; j < form.ItemList.Count; j++)
                {
                    if (form.ItemList[j] is SkipInstructionsItem)
                    {
                        skipItems.Add(form.ItemList[j]);
                    }
                }

                // for each Skip Instructions Item
                for (int k = 0; k < skipItems.Count; k++)
                {
                    // build its Variable list now so variables will be available to everyone
                    var instructions = ((SkipInstructionsItem)skipItems[k]).Instructions as SkipInstructions;
                    setSkipInstructionsVariables(i, k, instructions);

                    instructions.MappedFields.Fields.Add(instructions.Variables);
                    instructions.MappedFields.Fields.Add(form.GetFormItemFieldsAndRecordVariables());
                    instructions.MappedFields.Fields.Add(form.SkipToDestinations);
                    instructions.MappedFields.Map();

                    // preserve mapped variables for use in Processes
                    mappedFields.Fields.Add(instructions.Variables);
                }

                // for each Skip Instructions Item
                for (int k = 0; k < skipItems.Count; k++)
                {
                    var instructions = ((SkipInstructionsItem)skipItems[k]).Instructions as SkipInstructions;

                    instructions.MappedFields.Fields.Add(mappedFields.Fields);
                    instructions.MappedFields.Map();
                }
            }

            // for each process in project...
            for (int i = 0; i < Project.Current.ProcessList.Count; i++)
            {
                Process process = Project.Current.ProcessList[i];

                process.MappedFields.Fields.Add(mappedFields.Fields);
                process.MappedFields.Map();
            }

            initializeFieldMapByName();

            Collection<string> documentNames = GetDocumentNames();

            // for each document in XML file...
            Project.Current.EnableComponentEvents(false);

            for (int i = 0; i < documentNames.Count; i++)
            {
                // get document from XML file
                RtfDocument fileDocument = GetDocument(documentNames[i]);

                Project.Current.AddDocument(fileDocument);
            }

            Project.Current.EnableComponentEvents(true);

            // for each process in project...
            for (int i = 0; i < Project.Current.ProcessList.Count; i++)
            {
                Process process = Project.Current.ProcessList[i];
                process.Lines.EnableEvents = false;
                process.Lines = GetProcessLines(process.Name);
                process.Lines.EnableEvents = true;
            }

            // for each form in project...
            for (int i = 0; i < Project.Current.FormList.Count; i++)
            {
                var form = (Form)Project.Current.FormList[i];

                var skipItems = new FormItemList();
                for (int j = 0; j < form.ItemList.Count; j++)
                {
                    if (form.ItemList[j] is SkipInstructionsItem)
                    {
                        skipItems.Add(form.ItemList[j]);
                    }
                }

                for (int k = 0; k < skipItems.Count; k++)
                {
                    var instructions = ((SkipInstructionsItem)skipItems[k]).Instructions as SkipInstructions;
                    ProcessLineList processLines = GetSkipInstructionsLines(form.Name, k, instructions);
                    instructions.Lines = processLines;
                    processLines.Process = instructions;
                }
            }

			// kludge to reduce load time of large projects
			// jdf - 5/13/10
			Project.LoadingProjectXml = false;

            return true;
        }

        private static bool xmlFormatNewerThanCurrent(IXmlElement projectXml)
        {
            string projectFormatVersionString = projectXml.GetAttribute("format");
            int dotIndex = projectFormatVersionString.IndexOf('.');
            int projectMajorFormatVersion = Convert.ToInt32(projectFormatVersionString.Substring(0, dotIndex));
            int projectMinorFormatVersion = Convert.ToInt32(projectFormatVersionString.Substring(dotIndex + 1));

            dotIndex = Project.XmlFormatVersion.IndexOf('.');
            int currentMajorFormatVersion = Convert.ToInt32(Project.XmlFormatVersion.Substring(0, dotIndex));
            int currentMinorFormatVersion = Convert.ToInt32(Project.XmlFormatVersion.Substring(dotIndex + 1));

            if (projectMajorFormatVersion > currentMajorFormatVersion)
            {
                return true;
            }
            if (projectMajorFormatVersion == currentMajorFormatVersion && projectMinorFormatVersion > currentMinorFormatVersion)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Places all of the fields contained in the project's "id" field map into the project's "name" field map.
        /// </summary>
        private static void initializeFieldMapByName()
        {
            Project.FieldMapByName.Clear();

            foreach (IPaletteField field in Project.FieldMapById.Values)
            {
                Project.FieldMapByName.AddUnique(field);
            }
        }

        /// <summary>
        /// Adds fields from the Form connected to a Process to the Process's field mapper
        /// </summary>
        private static void addNativeFieldsToMap(Process process)
        {
            foreach (Form form in Project.Current.GetFormList(process))
            {
                process.MappedFields.Fields.Add(form.GetFields());
            }
        }

        /// <summary>
        /// Adds fields from all Forms to the Process's field mapper
        /// </summary>
        private static void addForeignFieldsToMap(Process process)
        {
            foreach (Form form in Project.Current.FormList)
            {
                process.MappedFields.Fields.Add(form.GetFields());
            }
        }

        /// <summary>
        /// Adds a Process's variables to its field mapper
        /// </summary>
        private static void addVariablesToMap(Process process)
        {
            process.MappedFields.Fields.Add(process.Variables);
            process.MappedFields.Map();
        }

        /// <summary>
        /// Reads the specified attribute from the element at the specified reader's current position
        /// </summary>
        private string getAttributeValue(XmlReader reader, string attributeName)
        {
            string attributeValue = "";

            // while there's another attribute to be read...
            while (reader.MoveToNextAttribute())
            {
                // if it's the specified attribute...
                if (reader.Name == attributeName)
                {
                    // capture attribute value
                    attributeValue = reader.Value;
                    break;
                }
            }

            // restore original reader position
            reader.MoveToElement();

            return attributeValue;
        }

        /// <summary>
        /// Reads the specified attribute from the specified element
        /// </summary>
        public string GetAttributeValue(string elementName, string attributeName)
        {
            XmlTextReader reader = getXmlReader();

            string attributeValue = "";

            // find the specified element node
            reader.ReadToFollowing(elementName);

            // while there's another attribute to read...
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == attributeName)
                {
                    return (reader.Value);
                }
            }

            return attributeValue;
        }

        public string GetProjectString()
        {
            XmlTextReader reader = getXmlReader();

            if (reader.ReadToFollowing("project"))
            {
                return reader.ReadOuterXml();
            }

            return "";
        }

        public string GetFormsString()
        {
            XmlTextReader reader = getXmlReader();

            // advance reader to the "forms" element
            if (reader.ReadToFollowing("forms"))
            {
                return reader.ReadOuterXml();
            }

            return "";
        }

        public string GetDocumentsString()
        {
            XmlTextReader reader = getXmlReader();

            // advance reader to the "documents" element
            if (reader.ReadToFollowing("documents"))
            {
                return reader.ReadOuterXml();
            }

            return "";
        }

        /// <summary>
        /// Returns a collection of form names from the XML file.
        /// </summary>
        public Collection<string> GetFormNames()
        {
            var formNames = new Collection<string>();

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "form" element after the first "forms" element
            reader.ReadToFollowing("forms");
            reader.ReadToFollowing("form");

            // process while there are more "form" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // add form name to collection
                        formNames.Add(reader.Value);
                    }
                }
            } while (reader.ReadToNextSibling("form"));

            return (formNames);
        }

        /// <summary>
        /// Returns a collection of document names from the XML file.
        /// </summary>
        public Collection<string> GetDocumentNames()
        {
            var documentNames = new Collection<string>();

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "document" element after the first "documents" element
            reader.ReadToFollowing("documents");
            reader.ReadToFollowing("document");

            // process while there are more "document" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // add form name to collection
                        documentNames.Add(reader.Value);
                    }
                }
            } while (reader.ReadToNextSibling("document"));

            return (documentNames);
        }

        /// <summary>
        /// Creates and returns a Text item from the text element at the specified reader's current position
        /// </summary>
        private static TextItem getTextItem(XmlReader reader)
        {
            //TextItem textItem = new TextItem();
            //textItem.AlternateLabel = getAttributeValue(reader, "alternateLabel");
            //textItem.Text = getTextItemText(reader);

            // create new reader to read descendant nodes
            XmlReader childReader = reader.ReadSubtree();

            // consume the text node
            childReader.Read();

            string xmlString = childReader.ReadOuterXml();
            //IXmlElement element = new Tawala.XmlSupport.XmlElement(xmlString, true);
            IXmlElement element = new XmlElement(xmlString);
            var textItem = new TextItem(element);

            // get rid of the temporary reader
            childReader.Close();

            return textItem;
        }

        private static HeadingItem getHeadingItem(XmlReader reader)
        {
            string xmlString = reader.ReadOuterXml();
            IXmlElement element = new XmlElement(xmlString);
            var heading = new HeadingItem(element);

            return heading;
        }

        private static FibItem getFibItem(XmlReader reader)
        {
            string xmlString = reader.ReadOuterXml();
            IXmlElement element = new XmlElement(xmlString, true);
            var fibItem = new FibItem(element);

            return (fibItem);
        }

        private static FileUploadItem getFileUploadItem(XmlReader reader)
        {
            string xmlString = reader.ReadOuterXml();
            IXmlElement element = new XmlElement(xmlString);
            return new FileUploadItem(element);
        }

        /// <summary>
        /// Creates and returns a MC item from the mc element at the specified reader's current position
        /// </summary>
        private static McqItem getMCItem(XmlReader reader)
        {
            string xmlString = reader.ReadOuterXml();
            IXmlElement element = new XmlElement(xmlString);
            var mcItem = new McqItem(element);

            return mcItem;
        }

        private static HiddenField getHiddenField(XmlReader reader)
        {
            string xmlString = reader.ReadOuterXml();
            IXmlElement element = new XmlElement(xmlString, true);
            var hiddenField = new HiddenField(element);

            return hiddenField;
        }

        /// <summary>
        /// Creates and returns a Break item from the break element at the specified reader's current position
        /// </summary>
        private static BreakItem getBreakItem(XmlReader reader)
        {
            var breakItem = new BreakItem();

            return (breakItem);
        }

        /// <summary>
        /// Creates and returns a SkipIstructionsItem from the skipInstructions element at the specified reader's current position
        /// </summary>
        private static SkipInstructionsItem getSkipInstructionsItem(XmlReader reader)
        {
            var skipInstructionsItem = new SkipInstructionsItem();

            return (skipInstructionsItem);
        }

        /// <summary>
        /// Converts form items from the specified form in the XML file to a form item list.
        /// </summary>
        public void SetFormItems(Form form)
        {
            XmlTextReader reader = getXmlReader();

            // advance reader to the first "form" element after the first "forms" element
            reader.ReadToFollowing("forms");
            reader.ReadToFollowing("form");

            // process while there are more "form" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // if it's the correct form...
                        if (reader.Value == form.Name)
                        {
                            // move reader from attribute node back to form element
                            reader.MoveToElement();

                            // create new reader to read descendant nodes
                            XmlReader childReader = reader.ReadSubtree();

                            // consume the form node
                            childReader.Read();

                            // while any descendant nodes exist...

                            bool done = false;

                            do
                            {
                                if (childReader.NodeType == XmlNodeType.Element)
                                {
                                    // depending on the node name...
                                    switch (childReader.Name)
                                    {
                                        case "text":
                                            TextItem textItem = getTextItem(childReader);
                                            form.ItemList.Add(textItem);
                                            break;

                                        case "heading":
                                            HeadingItem headingItem = getHeadingItem(childReader);
                                            form.ItemList.Add(headingItem);
                                            break;

                                        case "fib":
                                            FibItem fibItem = getFibItem(childReader);
                                            form.ItemList.Add(fibItem);
                                            break;

                                        case "file":
                                            form.ItemList.Add(getFileUploadItem(childReader));
                                            break;

                                        case "mc":
                                            McqItem mcItem = getMCItem(childReader);
                                            form.ItemList.Add(mcItem);
                                            break;

                                        case "break":
                                            BreakItem breakItem = getBreakItem(childReader);
                                            form.ItemList.Add(breakItem);
                                            childReader.Read();
                                            break;

                                        case "skipInstructions":
                                            SkipInstructionsItem skipToItem = getSkipInstructionsItem(childReader);
                                            form.ItemList.Add(skipToItem);
                                            childReader.Read();
                                            break;

                                        case "field":
                                            HiddenField hiddenField = getHiddenField(childReader);
                                            form.ItemList.Add(hiddenField);
                                            break;

                                        default:
                                            // consume non-form item element
                                            childReader.Read();
                                            break;
                                    }
                                }
                                else
                                {
                                    // consume non-Element node
                                    childReader.Read();
                                }

                                done = ((childReader.NodeType == XmlNodeType.EndElement && childReader.Name == "items") ||
                                        childReader.NodeType == XmlNodeType.None);
                            } while (!done);

                            // get rid of the temporary reader
                            childReader.Close();
                        }
                    }
                }
            } while (reader.ReadToNextSibling("form"));
        }

        /// <summary>
        /// Retrieves the name of the process  in the XML file connected to the specified form.
        /// </summary>
        public string GetConnectedProcess(string formName)
        {
            string processName = "";

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "form" element after the first "forms" element
            reader.ReadToFollowing("forms");
            reader.ReadToFollowing("form");

            // process while there are more "form" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // if it's the correct form...
                        if (reader.Value == formName)
                        {
                            // move reader from attribute node back to form element
                            reader.MoveToElement();

                            processName = getAttributeValue(reader, "process");
                            break;
                        }
                    }
                }
            } while (reader.ReadToNextSibling("form"));

            return processName;
        }

        /// <summary>
        /// Retrieves the name of the preProcess in the XML file connected to the specified form.
        /// </summary>
        public string GetConnectedPreProcess(string formName)
        {
            string processName = "";

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "form" element after the first "forms" element
            reader.ReadToFollowing("forms");
            reader.ReadToFollowing("form");

            // process while there are more "form" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // if it's the correct form...
                        if (reader.Value == formName)
                        {
                            // move reader from attribute node back to form element
                            reader.MoveToElement();

                            processName = getAttributeValue(reader, "preProcess");
                            break;
                        }
                    }
                }
            } while (reader.ReadToNextSibling("form"));

            return processName;
        }

        /// <summary>
        /// Converts the specified document in the XML file to a document object.
        /// </summary>
        public RtfDocument GetDocument(string documentName)
        {
            IDocument document = Document.NULL;

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "document" element after the first "documents" element
            reader.ReadToFollowing("documents");
            reader.ReadToFollowing("document");

            // process while there are more "document" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // if it's the specified document...
                        if (reader.Value == documentName)
                        {
                            // move reader back to document element
                            reader.MoveToElement();

                            string xmlString = reader.ReadOuterXml();
                            IXmlElement element = new XmlElement(xmlString);
                            document = new RtfDocument(element);

                            break;
                        }
                    }
                }
            } while (reader.ReadToNextSibling("document"));

            return (RtfDocument)document;
        }

        /// <summary>
        /// Returns a collection of process names from the XML file.
        /// </summary>
        public Collection<string> GetProcessNames()
        {
            var processNames = new Collection<string>();

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "process" element after the first "processes" element
            reader.ReadToFollowing("processes");
            reader.ReadToFollowing("process");

            // process while there are more "process" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // add form name to collection
                        processNames.Add(reader.Value);
                    }
                }
            } while (reader.ReadToNextSibling("process"));

            return (processNames);
        }

        /// <summary>
        /// Creates and returns a document name from the document attribute at the specified reader's current position
        /// </summary>
        private string getDocumentName(XmlReader reader)
        {
            return (getAttributeValue(reader, "document"));
        }

        /// <summary>
        /// Creates and returns a document name from the appendage attribute at the specified reader's current position
        /// </summary>
        private string getAppendageName(XmlReader reader)
        {
            return (getAttributeValue(reader, "appendage"));
        }

        /// <summary>
        /// Creates and returns a form name from the form attribute at the specified reader's current position
        /// </summary>
        private string getFormName(XmlReader reader)
        {
            return (getAttributeValue(reader, "form"));
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the show element at the specified reader's current position
        /// </summary>
        private ProcessLineList getShowList(XmlReader reader, string processName)
        {
            ProcessLineList showList;
            ShowStatement showStatement = null;

            if (!string.IsNullOrEmpty(getDocumentName(reader)))
            {
                string xmlString = reader.ReadOuterXml();
                IXmlElement element = new XmlElement(xmlString);
                showStatement = new ShowDocumentStatement(element, "No Process");
            }
            else if (!string.IsNullOrEmpty(getFormName(reader)))
            {
                string xmlString = reader.ReadOuterXml();
                IXmlElement element = new XmlElement(xmlString);
                showStatement = new ShowFormStatement(element, "No Process");
            }
            else
            {
                string xmlString = reader.ReadOuterXml();
                IXmlElement element = new XmlElement(xmlString);

                showStatement = new ShowUrlStatement(element);
            }

            showList = new ProcessLineList(showStatement);

            return showList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the show element at the specified reader's current position
        /// </summary>
        private ProcessLineList getShowRecordList(XmlReader reader, string processName)
        {
            string xmlString = reader.ReadOuterXml();
            IXmlElement element = new XmlElement(xmlString);

            ShowStatement showStatement = new ShowRecordStatement(element, processName);

            var showRecordList = new ProcessLineList(showStatement);

            return showRecordList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the append element at the specified reader's current position
        /// </summary>
        private ProcessLineList getAppendList(XmlReader reader, string processName)
        {
            ProcessLineList appendList;
            var appendStatement = new AppendStatement();

            string componentName;

            // if append element has a document attribute...
            if ((componentName = getDocumentName(reader)) != "")
            {
                // get document from Project
                appendStatement.Document = Project.Current.GetRealOrVirtualDocument(componentName, true);
            }

            // if append element has a document attribute...
            if ((componentName = getAppendageName(reader)) != "")
            {
                // get document from Project
                appendStatement.Appendage = Project.Current.GetRealOrVirtualDocument(componentName, true);
            }

            // create process line list
            appendList = new ProcessLineList(appendStatement);

            return appendList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the if element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at an if element inside a process element</remarks>
        private ProcessLineList getIfList(XmlReader reader, string processName)
        {
            var ifList = new ProcessLineList();

            string xmlString = getXmlStringWithSpaceCharacters(reader);

            IXmlElement element = new XmlElement(xmlString, true);
            var ifStatement = new IfStatement(element, processName);

            ifList.Add(new ProcessLineList(ifStatement));

            return ifList;
        }

        private static string getXmlStringWithSpaceCharacters(XmlReader reader)
        {
            string xmlString = reader.ReadOuterXml();
            return Regex.Replace(xmlString, "[\r\n\t]", "");
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the if element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at an if element inside a skipInstructions element</remarks>
        private ProcessLineList getSkipInstructionsIfList(XmlReader reader, string formName, Process instructions)
        {
            var ifList = new ProcessLineList();

            string xmlString = reader.ReadOuterXml();
            IXmlElement element = new XmlElement(xmlString);
            var ifStatement = new IfStatement(element, instructions);

            // create process line list from if statement
            ifList.Add(new ProcessLineList(ifStatement));

            return ifList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList from the skip to element at the specified reader's current position
        /// </summary>
        private ProcessLineList getSkipToList(XmlReader reader, string formName)
        {
            SkipToDestinationItem destinationItem;

            // get value of to attribute
            string formItemLabel = getAttributeValue(reader, "to");

            if (formItemLabel == "__EndOfForm__")
            {
                // create destination item
                destinationItem = new SkipToDestinationItem();
            }
            else
            {
                IForm form = Project.Current.GetForm(formName);
                IFormItem formItem = form.GetFormItem(formItemLabel);

                // create destination item
                destinationItem = new SkipToDestinationItem(formItem);
            }

            // create process line list
            var skipToStatement = new SkipToStatement(destinationItem);
            var skipToList = new ProcessLineList(skipToStatement);

            return skipToList;
        }

        /// <summary>
        /// Creates and returns an expression string from the arithmetic operator element at the specified reader's current position
        /// </summary>
        /// <remarks>This recursive method accumulates an operator and 2 operands, then creates and returns a representative expression string</remarks>
        private string getExpressionString(XmlReader reader, string operatorString)
        {
            string expressionString = "";

            int operandCount = 0;
            var operandStrings = new string[2];

            var operatorTable = new Hashtable();
            operatorTable.Add("add", " + ");
            operatorTable.Add("sub", " - ");
            operatorTable.Add("mul", " * ");
            operatorTable.Add("div", " / ");

            // while there's a node to read...
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // if its name is "string"...
                    if (reader.Name == "string")
                    {
                        // attempt to get value of "value" attribute
                        string tempString = getAttributeValue(reader, "value");

                        // if there was no "value" attribute...
                        if (tempString == "")
                        {
                            // get value of "field" attribute
                            tempString = "<<" + getAttributeValue(reader, "field") + ">>";
                        }

                        // append value of "value" or "field" attribute to expression string
                        expressionString += tempString;
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "add":
                            case "sub":
                            case "mul":
                            case "div":

                                // if at top-level element
                                if (operatorString == "")
                                {
                                    operatorString = (string)operatorTable[reader.Name];
                                }
                                else
                                {
                                    operandStrings[operandCount++] = "(" + getExpressionString(reader, (string)operatorTable[reader.Name]) +
                                                                     ")";
                                }

                                break;

                            case "operand":
                                string operandName = getAttributeValue(reader, "field");

                                if (operandName == "")
                                {
                                    operandName = getAttributeValue(reader, "value");
                                }
                                else
                                {
                                    operandName = "<<" + operandName + ">>";
                                }

                                operandStrings[operandCount++] = operandName;

                                break;
                        }

                        if (operandCount == 2)
                        {
                            expressionString = operandStrings[0] + operatorString + operandStrings[1];
                            break;
                        }
                    }
                }
            }

            if (operandCount == 1)
            {
                expressionString = operandStrings[0];
            }

            return expressionString;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the set element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at a set element</remarks>
        private ProcessLineList getSetList(XmlReader reader, string processName)
        {
            IXmlElement element = new XmlElement(reader);
            var setStatement = new SetStatement(element, processName);
            return new ProcessLineList(setStatement);

            // REVISIT: should we be using references to existing variables?

            //string varName = getAttributeValue(reader, "field");
            //IAssignable assignable = null;

            //if (Project.FieldMapByName.ContainsKey(varName))
            //{
            //    assignable = Project.FieldMapByName[varName] as IAssignable;
            //}

            //if (assignable != null)
            //{
            //    setStatement.Variable = assignable;
            //}
            //else
            //{
            //    setStatement.Variable = new Variable(varName);
            //}

            //setStatement.TreatArithmeticAsText = (getAttributeValue(reader, "arithmeticAsText") == "true") ? true : false;

            //// create new reader to read descendant nodes
            //XmlReader childReader = reader.ReadSubtree();

            //// consume the set node
            //childReader.Read();

            //// get expression beginning at first operator node
            //string expressionString = getExpressionString(childReader, "");

            //if (enclosingForEachStatement != null)
            //{
            //    setStatement.Expression = new Expression(expressionString, process.GetValidFields(enclosingForEachStatement));
            //}
            //else
            //{
            //    setStatement.Expression = new Expression(expressionString);
            //}

            //// create process line list from set statement
            //ProcessLineList setList = new ProcessLineList(setStatement);

            //// get rid of child reader
            //childReader.Close();

            //return setList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the get element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at a get element</remarks>
        private ProcessLineList getGetList(XmlReader reader, string processName)
        {
            IXmlElement element = new XmlElement(reader);

            var getStatement = new GetStatement(element, processName);

            //GetStatement getStatement = new GetStatement(element);

            //Process process = Project.Current.GetProcess(processName);

            //if (!process.RecordSets.Contains(getStatement.Records))
            //{
            //    process.RecordSets.Add(getStatement.Records);
            //}

            // create process line list from get statement
            var getList = new ProcessLineList(getStatement);

            return getList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the delete element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at a delete element</remarks>
        private ProcessLineList getDeleteList(XmlReader reader, string processName)
        {
            IXmlElement element = new XmlElement(reader);

            var deleteStatement = new DeleteStatement(element, processName);

            // create process line list from delete statement
            var deleteList = new ProcessLineList(deleteStatement);

            return deleteList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the foreach element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at a foreach element</remarks>
        private ProcessLineList getForEachRecordList(XmlReader reader, string processName)
        {
            string xmlString = getXmlStringWithSpaceCharacters(reader);

            IXmlElement element = new XmlElement(xmlString, true);

            var forEachStatement = new ForEachRecordStatement(element, processName);

            var forEachList = new ProcessLineList(forEachStatement);

            return forEachList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the arithmetic element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at an arithmetic element</remarks>
        private ProcessLineList getArithmeticList(XmlReader reader, Process process)
        {
            var arithmeticStatement = new ArithmeticStatement();

            switch (reader.Name)
            {
                case "addTo":
                    arithmeticStatement = new AddStatement();
                    break;

                case "subtractFrom":
                    arithmeticStatement = new SubtractStatement();
                    break;

                case "multiplyBy":
                    arithmeticStatement = new MultiplyStatement();
                    break;

                case "divideBy":
                    arithmeticStatement = new DivideStatement();
                    break;
            }

            // get variable string from add node "field" attribute
            arithmeticStatement.Variable = getAttributeValue(reader, "field");

            // create new reader to read descendant nodes
            XmlReader childReader = reader.ReadSubtree();

            // consume the set node
            childReader.Read();

            string initialText = "";
            FieldOrLiteral.StringType initialType = FieldOrLiteral.StringType.literal;

            while (childReader.Read())
            {
                if (childReader.Name == "operand")
                {
                    initialText = getAttributeValue(reader, "value");

                    if (initialText != "")
                    {
                        initialType = FieldOrLiteral.StringType.literal;
                    }
                    else
                    {
                        initialText = getAttributeValue(reader, "field");
                        initialType = FieldOrLiteral.StringType.field;
                    }
                }
            }

            if (enclosingForEachStatement != null)
            {
                arithmeticStatement.Value = new FieldOrLiteral(initialText, initialType, process.GetValidFields(enclosingForEachStatement));
            }
            else
            {
                arithmeticStatement.Value = new FieldOrLiteral(initialText, initialType);
            }

            // create process line list from add statement
            var arithmeticList = new ProcessLineList(arithmeticStatement);

            // get rid of child reader
            childReader.Close();

            return arithmeticList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the send element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at a send element</remarks>
        private ProcessLineList getSendList(XmlReader reader, string processName)
        {
            IXmlElement element = new XmlElement(reader, true);
            var sendStatement = new SendStatement(element, processName);

            // create process line list from send statement
            var sendList = new ProcessLineList(sendStatement);

            return sendList;
        }

        /// <summary>
        /// Creates and returns a ProcessLineList item from the comment element at the specified reader's current position
        /// </summary>
        /// <remarks>This method assumes that the reader's current position is at a comment element</remarks>
        private ProcessLineList getCommentList(XmlReader reader, string processName)
        {
            IXmlElement element = new XmlElement(reader);
            var commentStatement = new CommentStatement(element, processName);

            var commentList = new ProcessLineList(commentStatement);

            return commentList;
        }

        /// <summary>
        /// Converts process lines starting at the current position of the specified reader to a process line list.
        /// </summary>
        private ProcessLineList getProcessLines(XmlReader reader, string processName)
        {
            var processLines = new ProcessLineList(Project.Current.GetProcess(processName));

            // consume process node
            reader.Read();

            bool done = false;

            do
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // depending on the node name...
                    switch (reader.Name)
                    {
                        case "show":
                            // get process line list from show element
                            ProcessLineList showList = getShowList(reader, processName);
                            // add to existing process lines
                            processLines.Add(showList);
                            break;

                        case "edit":
                            // get process line list from show element
                            ProcessLineList showRecordList = getShowRecordList(reader, processName);
                            // add to existing process lines
                            processLines.Add(showRecordList);
                            break;

                        case "if":
                            // get process line list from if element
                            ProcessLineList ifList = getIfList(reader, processName);
                            // add to existing process lines
                            processLines.Add(ifList);
                            break;

                        case "set":
                            // get process line list from set element
                            ProcessLineList setList = getSetList(reader, processName);
                            // add to existing process lines
                            processLines.Add(setList);
                            break;

                        case "send":
                            // get process line list from send element
                            ProcessLineList sendList = getSendList(reader, processName);
                            // add to existing process lines
                            processLines.Add(sendList);
                            break;

                        case "addTo":
                        case "subtractFrom":
                        case "multiplyBy":
                        case "divideBy":
                            // get process line list from add element
                            ProcessLineList addList = getArithmeticList(reader, Project.Current.GetProcess(processName));

                            // add to existing process lines
                            processLines.Add(addList);
                            break;

                        case "append":
                            // get process line list from append element
                            ProcessLineList appendList = getAppendList(reader, processName);
                            // add to existing process lines
                            processLines.Add(appendList);
                            // consume append node
                            reader.Read();
                            break;

                        case "delete":
                            // delete process line list from delete element
                            ProcessLineList deleteList = getDeleteList(reader, processName);
                            // add to existing process lines
                            processLines.Add(deleteList);
                            break;

                        case "get":
                            // get process line list from get element
                            ProcessLineList getList = getGetList(reader, processName);
                            // add to existing process lines
                            processLines.Add(getList);
                            break;

                        case "foreach":
                            // get process line list from foreach record element
                            ProcessLineList forEachRecordList = getForEachRecordList(reader, processName);
                            // add to existing process lines
                            processLines.Add(forEachRecordList);
                            break;

                            #region ForEachQuestionStatement unused

#if false
						case "forEachMc":
							// get process line list from foreach question element
							ProcessLineList forEachQuestionList = getForEachQuestionList(reader, processName);
							processLines.Add(forEachQuestionList);
							break;
#endif

                            #endregion

                        case "comment":
                            ProcessLineList commentList = getCommentList(reader, processName);
                            processLines.Add(commentList);
                            break;
                    }
                }
                else
                {
                    // consume non-Element node
                    reader.Read();
                }

                done = (reader.NodeType == XmlNodeType.EndElement && reader.Name == "process");
            } while (!done);

            return (processLines);
        }

        //private void setProcessVariables(Process process)
        //{
        //    XmlTextReader reader = getXmlReader();

        //    // advance reader to the first "process" element after the first "processes" element
        //    reader.ReadToFollowing("processes");
        //    reader.ReadToFollowing("process");

        //    // process while there are more "process" elements to be read...
        //    do
        //    {
        //        // if there is an attribute...
        //        if (reader.MoveToFirstAttribute())
        //        {
        //                // if it's the specified process...
        //            if (reader.Name == "name" && reader.Value == process.Name)
        //            {
        //                // move reader back to process element
        //                reader.MoveToElement();

        //                // create new reader to read descendant nodes
        //                XmlReader processReader = reader.ReadSubtree();

        //                // consume the process node
        //                processReader.Read();

        //                // key = RecordSet name, value = Form name
        //                StringStringDictionary procRecordSets = new StringStringDictionary();

        //                // key = Record name, value = RecordSet name
        //                StringStringDictionary procRecords = new StringStringDictionary();

        //                // while any nodes exist...
        //                while (processReader.Read())
        //                {
        //                    if (processReader.NodeType == XmlNodeType.Element)
        //                    {
        //                        switch (processReader.Name)
        //                        {
        //                            case "get":
        //                                string recordList = getAttributeValue(processReader, "recordList");
        //                                if (processReader.ReadToDescendant("form"))
        //                                {
        //                                    procRecordSets.Add(recordList, getAttributeValue(processReader, "name"));
        //                                }
        //                                break;

        //                            case "foreach":
        //                                procRecords.AddUnique(getAttributeValue(processReader, "record"), getAttributeValue(processReader, "recordList"));
        //                                break;

        //                            case "set":
        //                            case "addTo":
        //                            case "subtractFrom":
        //                            case "multiplyBy":
        //                            case "divideBy":
        //                                string fieldString = getAttributeValue(processReader, "field");
        //                                if (fieldString != null && fieldString != "")
        //                                {
        //                                    if (isFormQualified(fieldString))
        //                                    {
        //                                        //addRecordVariableToForm(fieldString, procRecordSets, procRecords);
        //                                    }
        //                                    else
        //                                    {
        //                                        process.Variables.AddUnique(fieldString);

        //                                        RecordField recordField = makeRecordField(fieldString, procRecords);

        //                                        if (recordField != null)
        //                                        {
        //                                            addRecordFieldToMap(process, recordField);
        //                                        }
        //                                    }
        //                                }

        //                                processReader.Read();
        //                                processReader.Read();

        //                                break;

        //                            default:
        //                                break;
        //                        }
        //                    }
        //                }

        //                // get rid of the temporary reader
        //                processReader.Close();
        //            }
        //        }
        //    } while (reader.ReadToNextSibling("process"));
        //}

        private void setProcessVariables(Process process)
        {
            XmlTextReader reader = getXmlReader();

            // advance reader to the first "process" element after the first "processes" element
            reader.ReadToFollowing("processes");
            reader.ReadToFollowing("process");

            // process while there are more "process" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // if it's the specified process...
                    if (reader.Name == "name" && reader.Value == process.Name)
                    {
                        // move reader back to process element
                        reader.MoveToElement();

                        // create new reader to read descendant nodes
                        XmlReader processReader = reader.ReadSubtree();

                        // consume the process node
                        processReader.Read();

                        // key = RecordSet name, value = Form name
                        var procRecordSets = new StringStringDictionary();

                        // key = Record name, value = RecordSet name
                        var procRecords = new StringStringDictionary();

                        // while any nodes exist...
                        while (processReader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "get":
                                        string recordList = getAttributeValue(processReader, "recordList");
                                        if (processReader.ReadToDescendant("form"))
                                        {
                                            //											procRecordSets.Add(recordList, getAttributeValue(processReader, "name"));
                                            procRecordSets.AddUnique(recordList, getAttributeValue(processReader, "name"));
                                        }
                                        break;

                                    case "foreach":
                                        procRecords.AddUnique(getAttributeValue(processReader, "record"),
                                                              getAttributeValue(processReader, "recordList"));
                                        break;

                                    case "set":
                                    case "addTo":
                                    case "subtractFrom":
                                    case "multiplyBy":
                                    case "divideBy":
                                        string fieldString = getAttributeValue(reader, "field");
                                        if (fieldString != null && fieldString != "")
                                        {
                                            if (!isFormQualified(fieldString))
                                            {
                                                process.Variables.AddUnique(fieldString);
                                            }
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }

                        // get rid of the temporary reader
                        processReader.Close();
                    }
                }
            } while (reader.ReadToNextSibling("process"));
        }

        private bool isFormQualified(string fieldString)
        {
            return Regex.IsMatch(fieldString, @"[^:]+:.*$");
        }

        private bool isRecordAndFormQualified(string fieldString)
        {
            return Regex.IsMatch(fieldString, @"[^:]+:[^:]+:.*$");
        }

        /// <summary>
        /// Extracts the record name from a string of the format "Record Name:Form Name:Field Name".
        /// </summary>
        private string getRecordName(string fieldString)
        {
            return Regex.Match(fieldString, @"([^:]+):[^:]+:.*$").Groups[1].Value;
        }

        /// <summary>
        /// Extracts the form name from a string of the format "Record Name:Form Name:Field Name".
        /// </summary>
        private string getFormName(string fieldString)
        {
            return Regex.Match(fieldString, @"[^:]+:([^:]+):.*$").Groups[1].Value;
        }

        //private static void addRecordVariableToForm(string fieldString, Dictionary<string, string> procRecordSets, Dictionary<string, string> procRecords)
        //{
        //    int colonIndex = fieldString.IndexOf(':');
        //    string qualifier = fieldString.Substring(0, colonIndex++);
        //    fieldString = fieldString.Substring(colonIndex, fieldString.Length - colonIndex);

        //    try
        //    {
        //        Form form = Project.Current.GetForm(procRecordSets[procRecords[qualifier]]);
        //        if (form != null)
        //        {
        //            form.RecordVariables.AddUnique(fieldString);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        /// <summary>
        /// Converts process lines from the specified process in the XML file to a process line list.
        /// </summary>
        public ProcessLineList GetProcessLines(string processName)
        {
            var processLines = new ProcessLineList();

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "process" element after the first "processes" element
            reader.ReadToFollowing("processes");
            reader.ReadToFollowing("process");

            // process while there are more "process" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // if it's the specified process...
                        if (reader.Value == processName)
                        {
                            // move reader back to process element
                            reader.MoveToElement();

                            // create new reader to read descendant nodes
                            XmlReader processReader = reader.ReadSubtree();

                            // consume the process node
                            processReader.Read();

                            // read process lines
                            processLines = getProcessLines(processReader, processName);

                            // get rid of the temporary reader
                            processReader.Close();

                            break;
                        }
                    }
                }
            } while (reader.ReadToNextSibling("process"));

            return (processLines);
        }

        public ProcessList GetProcesses()
        {
            var processList = new ProcessList();
            XmlTextReader reader = getXmlReader();

            if (reader.ReadToFollowing("processes"))
            {
                string xmlString = reader.ReadOuterXml();
                IXmlElement element = new XmlElement(xmlString);
                processList = new ProcessList(element);
            }

            return (processList);
        }

        /// <summary>
        /// Converts process lines starting at the current position of the specified reader to a process line list.
        /// </summary>
        private ProcessLineList getSkipInstructionsLines(XmlReader reader, string formName, Process instructions)
        {
            var processLines = new ProcessLineList();

            // create new reader to read descendant nodes
            XmlReader childReader = reader.ReadSubtree();

            // consume the skipInstructions node
            childReader.Read();

            // while any nodes exist...
            while (childReader.Read())
            {
                // depending on the node name...
                switch (childReader.Name)
                {
                    case "if":
                        // get process line list from if element
                        ProcessLineList ifList = getSkipInstructionsIfList(childReader, formName, instructions);
                        // add to existing process lines
                        processLines.Add(ifList);
                        break;

                    case "set":
                        // get process line list from set element
                        ProcessLineList setList = getSetList(childReader, "");
                        // add to existing process lines
                        processLines.Add(setList);
                        break;

                    case "addTo":
                    case "subtractFrom":
                    case "multiplyBy":
                    case "divideBy":
                        // get process line list from add element
                        ProcessLineList addList = getArithmeticList(reader, instructions);
                        // add to existing process lines
                        processLines.Add(addList);
                        break;

                    case "skip":
                        // get process line list from skipTo element
                        ProcessLineList skipToList = getSkipToList(childReader, formName);
                        // add to existing process lines
                        processLines.Add(skipToList);
                        break;

                    case "comment":
                        ProcessLineList commentList = getCommentList(childReader, "");
                        processLines.Add(commentList);
                        break;
                }
            }

            // get rid of child reader
            childReader.Close();

            return (processLines);
        }

        /// <summary>
        /// Converts skip instructions lines from the specified form in the XML file to a process line list.
        /// </summary>
        public ProcessLineList GetSkipInstructionsLines(string formName, int formItemIndex, Process instructions)
        {
            var processLines = new ProcessLineList();

            XmlTextReader reader = getXmlReader();

            // advance reader to the first "forms" element after the first "form" element
            reader.ReadToFollowing("forms");
            reader.ReadToFollowing("form");

            // process while there are more "form" elements to be read...
            do
            {
                // if there is an attribute...
                if (reader.MoveToFirstAttribute())
                {
                    // and its name is "name"...
                    if (reader.Name == "name")
                    {
                        // if it's the specified form...
                        if (reader.Value == formName)
                        {
                            // move reader back to form element
                            reader.MoveToElement();

                            // create new reader to read descendant nodes
                            XmlReader formReader = reader.ReadSubtree();

                            // advance child reader to the first "items" element
                            formReader.ReadToFollowing("items");

                            // consume the "items" element
                            formReader.Read();

                            // advance reader to first "skipInstructions" element
                            formReader.ReadToNextSibling("skipInstructions");

                            // while not yet at element indicated by item index...
                            for (int i = 0; i < formItemIndex; i++)
                            {
                                // advance reader to next "skipInstructions" element
                                formReader.ReadToNextSibling("skipInstructions");
                            }

                            // read skip instructions lines
                            processLines = getSkipInstructionsLines(formReader, formName, instructions);

                            // get rid of the temporary reader
                            formReader.Close();

                            break;
                        }
                    }
                }
            } while (reader.ReadToNextSibling("form"));

            return (processLines);
        }

        private void setSkipInstructionsVariables(int formIndex, int skipItemIndex, SkipInstructions instructions)
        {
            XmlTextReader reader = getXmlReader();

            // advance reader to the specified  "forms" element after the first "form" element
            reader.ReadToFollowing("forms");

            int i = 0;

            do
            {
                reader.ReadToFollowing("form");
            } while (i++ < formIndex);

            // process each skipInstructions element in Form
            i = 0;

            do
            {
                reader.ReadToFollowing("skipInstructions");

                // create new reader to read descendant nodes
                XmlReader processReader = reader.ReadSubtree();

                // while any nodes exist...
                while (processReader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "set":
                            case "addTo":
                            case "subtractFrom":
                            case "multiplyBy":
                            case "divideBy":
                                string fieldString = getAttributeValue(reader, "field");
                                if (fieldString != null && fieldString != "")
                                {
                                    if (!isFormQualified(fieldString))
                                    {
                                        instructions.Variables.AddUnique(fieldString);
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                // get rid of the temporary reader
                processReader.Close();
            } while (i++ < skipItemIndex);
        }

        #region ForEachQuestionStatement unused

#if false
    /// <summary>
    /// Creates and returns a ProcessLineList item from the foreach element at the specified reader's current position
    /// </summary>
    /// <remarks>This method assumes that the reader's current position is at a forEachMc element</remarks>
		private ProcessLineList getForEachQuestionList(XmlReader reader, string processName)
		{
			ForEachQuestionStatement forEachStatement = new ForEachQuestionStatement();
			Process process = Project.Current.GetProcess(processName);

			// create new reader to read descendant nodes
			XmlReader childReader = reader.ReadSubtree();

			// consume the foreach node
			childReader.Read();

			// REVISIT: Need to get rid of these bracketing statements
			enclosingForEachStatement = forEachStatement;
			ProcessLineList enclosedLines = getProcessLines(childReader, process.Name);
			enclosingForEachStatement = null;

			// get rid of child reader
			childReader.Close();

			// create process line list from foreach statement
			ProcessLineList forEachList = new ProcessLineList(forEachStatement);
			forEachList.Insert(2, enclosedLines);

			return forEachList;
		}
#endif

        #endregion

        #region Nested type: StringStringDictionary

        private class StringStringDictionary : Dictionary<string, string>
        {
            public void AddUnique(string key, string value)
            {
                if (!ContainsKey(key))
                {
                    Add(key, value);
                }
            }
        }

        #endregion
    }
}