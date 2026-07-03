using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.Processes;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Interfaces;

namespace Tawala.FormDesigner.Dialogs.SkipInstructionsDialog
{
	public partial class SkipInstructionsView : System.Windows.Forms.Form
	{
		private IFormView view;
		private ISkipInstructionsItem skipInstructionsItem;

		private static readonly Type[] statementViewTypes =
		{
			typeof(Processes.IfStatementView), 
            null,
            typeof(Processes.SkipToStatementView),
            null,
            typeof(Processes.SetStatementView),
			null,
            typeof(Processes.CommentStatementView),
			null
		};

		public SkipInstructionsView(IFormView view, ISkipInstructionsItem skipInstructionsItem)
		{
			InitializeComponent();

			this.view = view;
			this.skipInstructionsItem = skipInstructionsItem;

			skipInstructionsStatementSelector.Init(statementViewTypes);
			skipInstructionsStatementSelector.ProcessEditor = processEditor;

			processEditor.Init(skipInstructionsStatementSelector, statementViewTypes);
			processEditor.Process = skipInstructionsItem.Instructions;
			processEditor.ConnectProjectEvents(true);
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void SkipInstructionsView_FormClosed(object sender, FormClosedEventArgs e)
		{
			view.SetAttribute(skipInstructionsItem.Id.ToString(), "Summary", skipInstructionsItem.GetSummary());
			processEditor.ConnectProjectEvents(false);
		}
	}
}

