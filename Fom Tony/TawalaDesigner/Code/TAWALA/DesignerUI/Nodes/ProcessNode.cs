// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;
using Tawala.Projects.Processes;

namespace Tawala.DesignerUI
{
    internal class ProcessNode : ComponentNode
    {
        private const int imageProcess = 11;

        public ProcessNode(IProcess process)
            : this(process, imageProcess)
        {
        }

        public ProcessNode(IProcess process, int imageIndex)
            : base(process, imageIndex)
        {
        }

        public void DisconnectProcess()
        {
            ((FormNode)Parent).DisconnectProcess();
        }

        public void DisconnectPreProcess()
        {
            ((FormNode)Parent).DisconnectPreProcess();
        }

        public override bool Rename(string newName)
        {
            return !Project.Current.RenameProcess(Text, newName);
        }
    }
}