// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using Tawala.UndoSupport;

namespace TawalaTest.AcceptanceTest.UndoRedoInProcesses
{
    [Serializable]
    public class TestProcessMemento : Memento
    {
        private readonly ProcessLineList lines;
        private readonly int selectedLineIndex;

        private readonly VariableList variables;

        public TestProcessMemento(Process process, int selectedLineIndex, string actionText)
            : base(actionText)
        {
            lines = process.Lines;
            variables = process.Variables;
            this.selectedLineIndex = selectedLineIndex;
        }

        public ProcessLineList Lines { get { return lines; } }

        public VariableList Variables { get { return variables; } }

        public int SelectedLineIndex { get { return selectedLineIndex; } }
    }
}