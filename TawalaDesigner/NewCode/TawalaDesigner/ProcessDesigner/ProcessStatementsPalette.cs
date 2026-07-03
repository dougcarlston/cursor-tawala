// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using System.Reflection;
using Tawala.Processes;

namespace Tawala.ProcessDesigner
{
	public partial class ProcessStatementsPalette : UserControl, IStatementSelector, IProcessStatementsPalette
	{
		private IProcessEditor processEditor;

		public ProcessStatementsPalette(Type[] statementViewTypes)
		{
			InitializeComponent();

			populateButtonPanel(statementViewTypes);
		}

		#region IStatementSelector Members

		private void populateButtonPanel(Type[] statementViewTypes)
		{
			tableLayoutPanelButtons.SuspendLayout();
			tableLayoutPanelButtons.RowCount = statementViewTypes.Length;
			tableLayoutPanelButtons.RowStyles.Clear();

			int row = 0;

			foreach (Type statementViewType in statementViewTypes)
			{
				StatementButton statementButton = null;

				RowStyle rowStyle = new RowStyle(System.Windows.Forms.SizeType.Absolute, 20F);

				if (isSeparator(statementViewType))
				{
					rowStyle.Height = 10F;
				}
				else
				{
					Type statementType = getStatementType(statementViewType);

					statementButton = new StatementButton();
					statementButton.Text = statementViewType.Name.Replace("Details", "");
					statementButton.Text = statementButton.Text.Replace("StatementView", "");
					statementButton.Tag = statementType;
					statementButton.Enabled = true;
					statementButton.Click += new EventHandler(button_Click);
				}

				tableLayoutPanelButtons.RowStyles.Add(rowStyle);

				if (statementButton != null)
				{
					tableLayoutPanelButtons.Controls.Add(statementButton, 0, row);
				}

				row++;
			}

			tableLayoutPanelButtons.ResumeLayout(true);
		}

		private static bool isSeparator(Type statementViewType)
		{
			return statementViewType == null;
		}

		private static Type getStatementType(Type statementViewType)
		{
			FieldInfo fieldInfo = statementViewType.GetField("statementType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			Type statementType = fieldInfo.GetValue(null) as Type;
			return statementType;
		}

		private void button_Click(object sender, EventArgs e)
		{
			if (processEditor != null)
			{
				processEditor.StatementButtonClicked(sender as StatementButton);
			}
		}

		public void SyncButtonToCurrentStatementType(System.Type t)
		{
		}

		public IProcessEditor ProcessEditor
		{
			get { return processEditor; }
			set { processEditor = value; }
		}

		public void Init(Type[] statementViewTypes)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
