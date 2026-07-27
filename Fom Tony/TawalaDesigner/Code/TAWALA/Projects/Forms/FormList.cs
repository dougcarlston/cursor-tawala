// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Components;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    /// <summary>
    /// List of forms.
    /// </summary>
    [Serializable]
    public class FormList : ComponentList<IForm>
    {
        static FormList()
        {
            xmlStartTag = "<forms>\r\n";
            xmlEndTag = "</forms>\r\n";
        }

        public FormList()
        {
        }

        /// <summary>
        /// Construct FormList from XML &lt;forms&gt; element.
        /// </summary>
        public FormList(IXmlElement element) : this()
        {
            foreach (IXmlElement childElement in element.GetChildren())
            {
                Add(ComponentMaker.MakeFormObject(childElement));
            }
        }

        /// <summary>
        /// Associates the specified Process with the specified Form
        /// </summary>
        public void ConnectProcessToForm(IProcess process, string formName)
        {
            int index = IndexOf(formName);
            if (index >= 0)
            {
                IForm form = this[index];
                form.ConnectedPostProcess = process;

                if (EnableEvents)
                {
                    Project.Events.RaiseProcessConnectedToFormEvent(new ProcessConnectionArgs(process, form));
                }
            }
        }

        /// <summary>
        /// Disconnects the connected Process from the specified Form
        /// </summary>
        public void DisconnectProcessFromForm(string formName)
        {
            int index = IndexOf(formName);

            if (index >= 0)
            {
                IForm form = this[index];
                IProcess oldProcess = form.ConnectedPostProcess;

                form.ConnectedPostProcess = null;

                if (EnableEvents)
                {
                    Project.Events.RaiseProcessDisconnectedFromFormEvent(new ProcessConnectionArgs(oldProcess, form));
                }
            }
        }

        /// <summary>
        /// Disconnects the specified Process from all Forms it is associated with
        /// </summary>
        public void DisconnectProcessFromAllForms(Process process)
        {
            for (int index = 0; index < Count; index++)
            {
                IForm form = this[index];
                if (form.ConnectedPostProcess == process)
                {
                    IProcess oldProcess = form.ConnectedPostProcess;

                    form.ConnectedPostProcess = null;

                    if (EnableEvents)
                    {
                        Project.Events.RaiseProcessDisconnectedFromFormEvent(new ProcessConnectionArgs(oldProcess, form));
                    }
                }
            }
        }
    }
}