// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Form=System.Windows.Forms.Form;

namespace Tawala.Processes
{
    public partial class ProcessesToolPaletteView : UserControl
    {
        public ProcessesToolPaletteView()
        {
            InitializeComponent();

            buttonIf.Tag = typeof(IfStatement);
            buttonShow.Tag = typeof(ShowStatement);
            buttonAppend.Tag = typeof(AppendStatement);
            buttonComment.Tag = typeof(CommentStatement);
            buttonDelete.Tag = typeof(DeleteStatement);
            buttonForEach.Tag = typeof(ForEachStatement);
            buttonGet.Tag = typeof(GetStatement);
            buttonSend.Tag = typeof(SendStatement);
            buttonSet.Tag = typeof(SetStatement);
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (ParentForm != null && ParentForm.ActiveMdiChild is MDIComponentView)
            {
                Form activeMDI = ParentForm.ActiveMdiChild;
                ProcessEditor editor = findProcessEditor(activeMDI.Controls);
                if (editor != null)
                {
                    activeMDI.Activate(); // keep it active
                    editor.StatementButtonClicked(sender as Button);
                }
            }
        }

        private ProcessEditor findProcessEditor(ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c is ProcessEditor)
                {
                    return c as ProcessEditor;
                }
            }

            foreach (Control c in controls)
            {
                ProcessEditor pe = findProcessEditor(c.Controls);

                if (pe != null)
                {
                    return pe;
                }
            }

            return null;
        }
    }
}