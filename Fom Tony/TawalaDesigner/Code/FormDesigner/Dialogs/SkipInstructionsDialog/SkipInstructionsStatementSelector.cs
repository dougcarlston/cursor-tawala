using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Tawala.Processes;

namespace Tawala.FormDesigner.Dialogs.SkipInstructionsDialog
{
	public partial class SkipInstructionsStatementSelector : UserControl, IStatementSelector
	{
		private IProcessEditor processEditor;

		public SkipInstructionsStatementSelector()
		{
			InitializeComponent();
		}

		#region IStatementSelector Members

		public void Init(Type[] statementViewTypes)
		{
			populateButtonPanel(statementViewTypes);
		}

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

		public void SyncButtonToCurrentStatementType(Type t)
		{
		}

		public IProcessEditor ProcessEditor
		{
			get { return processEditor; }
			set { processEditor = value; }
		}

		#endregion
	}
}
