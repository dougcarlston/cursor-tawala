// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Windows.Forms;
using Tawala.ProjectUI;

namespace Tawala.Processes
{
    public partial class StatementPalette : ComponentPalette, IStatementSelector
    {
        public StatementPalette()
        {
            InitializeComponent();
        }

        #region IStatementSelector Members

        public void Init(Type[] statementViewTypes)
        {
            commandTable.SuspendLayout();
            commandTable.RowCount = statementViewTypes.Length;
            commandTable.RowStyles.Clear();

            for (int i = 0; i < statementViewTypes.Length; ++i)
            {
                StatementButton statementButton = null;
                Type statementViewType = statementViewTypes[i];

                var rowStyle = new RowStyle(SizeType.Absolute, 20F);

                if (isSeparator(statementViewType))
                {
                    if (separatorIsNeeded(statementViewTypes, i))
                    {
                        rowStyle.Height = 10F;
                    }
                }
                else
                {
                    Type statementType = getStatementType(statementViewType);

                    statementButton = new StatementButton();
                    statementButton.Text = statementViewType.Name.Replace("Details", "");
                    statementButton.Text = statementButton.Text.Replace("StatementView", "");
                    statementButton.Tag = statementType;
                    statementButton.Enabled = true;
                    statementButton.Click += button_Click;
                }

                commandTable.RowStyles.Add(rowStyle);

                if (statementButton != null)
                {
                    commandTable.Controls.Add(statementButton, 0, i);
                }
            }

            commandTable.ResumeLayout(true);
        }

        public void SyncButtonToCurrentStatementType(Type t)
        {
            foreach (Control control in commandTable.Controls)
            {
                var button = control as StatementButton;

                if (button != null)
                {
                    button.Active = (button.Tag == t);
                }
            }
        }

        public IProcessEditor ProcessEditor { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        #endregion

        private static Type getStatementType(Type statementViewType)
        {
            FieldInfo fieldInfo = statementViewType.GetField("statementType",
                                                             BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var statementType = fieldInfo.GetValue(null) as Type;
            return statementType;
        }

        private static bool separatorIsNeeded(Type[] statementViewTypes, int i)
        {
            return (i != statementViewTypes.Length - 1);
        }

        private static bool isSeparator(Type statementViewType)
        {
            return statementViewType == null;
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
                    editor.StatementButtonClicked(sender as StatementButton);
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