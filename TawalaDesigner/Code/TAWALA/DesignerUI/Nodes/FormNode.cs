// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;
using Tawala.Projects.Processes;

namespace Tawala.DesignerUI
{
    internal class FormNode : ComponentNode
    {
        private const int imageForm = 0;

        internal FormNode(IForm form)
            : base(form, GetFormIconImageIndex(form))
        {
        }

        internal bool HasConnectedProcess { get { return form.HasConnectedProcess; } }

        private Form form { get { return Tag as Form; } }

        internal string DataSourceName { get { return form.DataSourceName; } set { form.DataSourceName = value; } }

        internal static int GetFormIconImageIndex(IForm form)
        {
            int index = 0;

            if (form.StartingPoint)
            {
                index += 1;
            }

            if (form.DataEntryOnly)
            {
                index += 2;
            }

            if (form.BlockBackButton)
            {
                index += 4;
            }

            return imageForm + index;
        }

        internal void ConnectProcess(ProcessNode processNode)
        {
            Project.Current.ConnectProcessToForm(Project.Current.GetProcess(processNode.Text), form.Name);
        }

        internal void DisconnectProcess()
        {
            Project.Current.DisconnectProcessFromForm(Text);
        }

        internal void ConnectPreProcess(ProcessNode processNode)
        {
            Process process = Project.Current.GetProcess(processNode.Text);

            Project.Current.ConnectPreProcessToForm(process, form.Name);

            Project.Events.RaisePreProcessConnectedToFormEvent(new ProcessConnectionArgs(process, form));
        }

        internal void DisconnectPreProcess()
        {
            IProcess process = form.ConnectedPreProcess;

            Project.Current.DisconnectPreProcessFromForm(form.Name);

            Project.Events.RaisePreProcessDisconnectedFromFormEvent(new ProcessConnectionArgs(process, form));
        }

        internal void RemoveConnectedProcessNode()
        {
            foreach (ProcessNode processNode in Nodes)
            {
                if (processNode is ConnectedProcessNode)
                {
                    processNode.Remove();
                }
            }
        }

        internal void RemoveConnectedPreProcessNode()
        {
            foreach (ProcessNode processNode in Nodes)
            {
                if (processNode is ConnectedPreProcessNode)
                {
                    processNode.Remove();
                }
            }
        }

        public override bool Rename(string newName)
        {
            return !Project.Current.RenameForm(Text, newName);
        }
    }
}