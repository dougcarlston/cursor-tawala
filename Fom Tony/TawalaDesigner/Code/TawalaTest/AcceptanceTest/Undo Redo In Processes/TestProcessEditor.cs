// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.UndoSupport;

namespace TawalaTest.AcceptanceTest.UndoRedoInProcesses
{
    public class TestProcessEditor
    {
        private static ProcessLineList clipboardLines;
        private readonly Process process;

        private readonly UndoManager undoManager = new UndoManager();
        private int insertionIndex;
        private int selectedLineIndex;

        public TestProcessEditor(Process process)
        {
            this.process = process;
        }

        public int SelectedLineIndex { set { selectedLineIndex = value; } }
        public int InsertionIndex { set { insertionIndex = value; } }
        public string UndoActionText { get { return undoManager.UndoActionText; } }

        public string RedoActionText { get { return undoManager.RedoActionText; } }

        public void Delete()
        {
            undoManager.Remember(getCurrentState("Delete"));

            ProcessLine line = process.Lines[selectedLineIndex];

            if (line.SelectsGroup)
            {
                process.Lines.Remove(selectedLineIndex, line.Group);
            }
            else
            {
                process.Lines.RemoveAt(selectedLineIndex);
            }
        }

        public void Cut()
        {
            undoManager.Remember(getCurrentState("Cut"));

            ProcessLine line = process.Lines[selectedLineIndex];

            if (line.SelectsGroup)
            {
                process.Lines.Remove(selectedLineIndex, line.Group);
            }
            else
            {
                process.Lines.RemoveAt(selectedLineIndex);
            }
        }

        protected void setDataObject(ProcessLineList lines)
        {
            clipboardLines = lines.Copy();
        }

        protected ProcessLineList getData()
        {
            return clipboardLines.Copy();
        }

        public void Copy()
        {
            var lines = new ProcessLineList(selectedLineIndex, process.Lines);
            setDataObject(lines);
        }

        public void Paste()
        {
            ProcessLineList lines = getData();

            if (selectedLineIndex != -1)
            {
                undoManager.Remember(getCurrentState("Paste"));

                process.Lines.Insert(selectedLineIndex, lines);
            }
            else if (insertionIndex >= 0 && insertionIndex <= process.Lines.Count)
            {
                undoManager.Remember(getCurrentState("Paste"));

                process.Lines.Insert(insertionIndex, lines);
            }
        }

        public void Move(int sourceIndex, int targetIndex)
        {
            undoManager.Remember(getCurrentState("Move"));

            process.Lines.DragDrop(sourceIndex, targetIndex);
        }

        public void AddStatement(ProcessStatement processStatement)
        {
            var newLines = new ProcessLineList(processStatement);

            if (insertionIndex == -1 || insertionIndex >= process.Lines.Count)
            {
                undoManager.Remember(getCurrentState("Add"));
                process.Lines.Add(newLines);
            }
            else
            {
                if (process.Lines[insertionIndex].CanInsertBefore)
                {
                    undoManager.Remember(getCurrentState("Add"));
                    process.Lines.Insert(insertionIndex, newLines);
                }
            }
        }

        public void ModifyStatement(ProcessStatement processStatement)
        {
            undoManager.Remember(getCurrentState("Modify"));
            process.Lines.Replace(selectedLineIndex, processStatement);
        }

        public void Undo()
        {
            restoreCurrentState(undoManager.Undo(getCurrentState(undoManager.UndoActionText)));
        }

        public void Redo()
        {
            restoreCurrentState(undoManager.Redo(getCurrentState(undoManager.RedoActionText)));
        }

        public IMemento getCurrentState(string actionText)
        {
            return new TestProcessMemento(process, insertionIndex, actionText);
        }

        public void restoreCurrentState(IMemento memento)
        {
            var processMemento = memento as TestProcessMemento;

            if (processMemento != null)
            {
                process.Lines = processMemento.Lines;
                process.Variables = processMemento.Variables;
                insertionIndex = processMemento.SelectedLineIndex;
            }
        }
    }
}