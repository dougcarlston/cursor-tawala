// $Workfile: ForEachStatementView.cs $
// $Revision: 9 $	$Date: 12/12/07 4:57p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Processes
{
	/// <summary>
	/// Details panel for ForEach Statement
	/// </summary>
    public partial class ForEachStatementView : ForEachStatementViewBase
	{
		ForEachRecordStatement forEachRecordStatement = new ForEachRecordStatement();

		// control size ratio for layout as tabpage changes width

		double ratioComboBoxRecord;
		int lastTabPageGetWidth;

		private const int detailsPanelHeight = 135;

		public ForEachStatementView()
		{
			InitializeComponent();

			ratioComboBoxRecord = comboBoxRecords.Width / (double)tabPageRecord.Width;
			tabPageRecord.Layout += new LayoutEventHandler(tabPageForEachRecord_Layout);
			tabPageRecord.PerformLayout();
		}
		
		protected override void OnProcessFormConnectionChange(ProcessConnectionArgs e)
		{
			refreshDataSources();
		}

		protected override void NewStatement()
		{
			forEachRecordStatement = new ForEachRecordStatement();

			selectStatement();
		}

		private void selectStatement()
		{
			statement = forEachRecordStatement;
		}

		/// <summary>
		/// This function handles editing a new or existing statement.
		/// </summary>
		protected override void OnEdit()
		{
			setDetailsPanelHeight(detailsPanelHeight);

			refreshDataSources();

			forEachRecordStatement = (ForEachRecordStatement)statement;
			tabControl.SelectedTab = tabPageRecord;

			// update the controls from the statement
			if (forEachRecordStatement.Record != null)
			{
				comboBoxRecords.Text = forEachRecordStatement.Record.FieldName;	// editable combo, don't sync
				comboBoxRecordSets.SelectedItem = forEachRecordStatement.RecordList;
			}

			comboBoxRecords.Focus();
		}

		/// <summary>
		/// On application idle update state of UI
		/// </summary>
		protected override void OnIdle()
		{
			bool bAlwaysCheck = AtInsertPosition() || ModifyMode;
			bool enabled =
				bAlwaysCheck &&
				comboBoxRecords.Text.Length > 0 &&
				comboBoxRecordSets.Text.Length > 0;
            if (enabled && Process != null)
			{
				enabled = Process.ValidRecordVariableName(comboBoxRecords.Text);
			}
			buttonAddModifyRecord.Enabled = enabled;
		}

		/// <summary>
		/// Refresh the data source and try to resync previous combobox selection
		/// </summary>
		private void refreshDataSources()
		{
			if (Process != null)
			{
				SetDataSource(comboBoxRecords, Process.Records);
				SetDataSource(comboBoxRecordSets, Process.RecordSets);
			}
		}

		#region Project Events

		protected override void OnCurrentComponentSet(Tawala.Projects.Component c)
		{
			if (c is Projects.Process)
			{
				refreshDataSources();
			}
		}

		#endregion

		#region Control Events

		/// <summary>
		/// Relayout the controls based on space available
		/// </summary>
		private void tabPageForEachRecord_Layout(object sender, LayoutEventArgs e)
		{
			if (ratioComboBoxRecord != 0.0 && lastTabPageGetWidth != tabPageRecord.Width && tabPageRecord.Width >= 240)
			{
				int width = tabPageRecord.Width;
				int oldRight = comboBoxRecords.Right;
				comboBoxRecords.Width = (int)(width * ratioComboBoxRecord); ;
				int offset = comboBoxRecords.Right - oldRight;
				labelIn.Left += offset;
				comboBoxRecordSets.Left += offset;
				comboBoxRecordSets.Width = width - 8 - comboBoxRecordSets.Left;
			}
			lastTabPageGetWidth = tabPageRecord.Width;
		}

		/// <summary>
		/// User clicked the Add / Modify button
		/// </summary>
		private void buttonAddModifyRecord_Click(object sender, EventArgs e)
		{
			RememberAction();

			// we trim leading and trailing whitespace from variable names
			string text = comboBoxRecords.Text.Trim();
			comboBoxRecords.Text = text;	// (put the trimmed name back into the text box)

			forEachRecordStatement.Record = updateRecordsDataSource(comboBoxRecords.Text);
			forEachRecordStatement.RecordList = (RecordSet)comboBoxRecordSets.SelectedItem;

			selectStatement();
			SaveStatement();

			Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(Process, ParentView.InsertionPoint));
		}

		private Record updateRecordsDataSource(string text)
		{
			Record record = null;

			// if designer typed a new name into the Records combo box
			if (Process.Records.IndexOf(text) == -1)
			{
				// create a new Record and add it to the processes list
				record = new Record(text);
				Process.Records.Add(record);

				refreshDataSources();
			}
			else
			{
				record = (Record)comboBoxRecords.SelectedItem;
			}

			return record;
		}

		#endregion
	}


	// Work around for VSN Form Designer issue with Generics

    public class ForEachStatementViewBase : StatementView<ForEachStatement>
	{
	}
}
