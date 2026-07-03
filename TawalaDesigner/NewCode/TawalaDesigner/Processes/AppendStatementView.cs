// $Workfile: AppendStatementView.cs $
// $Revision: 4 $	$Date: 12/12/07 4:57p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Tawala.Projects;

namespace Tawala.Processes
{
	public partial class AppendStatementView : AppendStatementViewBase
	{
		// control size ratio for layout as tabpage changes width
		double ratioComboBoxAppend = 0.0;
		int lastTabPageWidth = 0;

		private const int detailsPanelHeight = 135;

		// These serve has data source for the combo boxes.
		// Since they use the same list if you don't wrap it in a separate
		// binding source then changing one combo box affects the other.
		private BindingSource appendageBinder = new BindingSource();
		private BindingSource documentBinder = new BindingSource();

        public AppendStatementView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			ratioComboBoxAppend = comboBoxAppend.Width / (double)tabPageDocument.Width;

			comboBoxAppend.DataSource = appendageBinder;
			comboBoxDoc.DataSource = documentBinder;

			Project.Events.DocumentChanged += Events_DocumentChanged;

			Project.Events.ComponentRenamed += Events_DocumentChanged;
		}

		void Events_DocumentChanged(object sender, ComponentEventArgs e)
		{
			refreshDataSources();
		}

		/// <summary>
		/// This function handles editing a new or existing statement.
		/// </summary>
		protected override void OnEdit()
		{
			setDetailsPanelHeight(detailsPanelHeight);

			// update the combo box lists
			refreshDataSources();

			// resync the lists with the current entries
			if (statement.Appendage != null)
			{
				appendageBinder.Position = appendageBinder.IndexOf(Project.Current.GetRealOrVirtualDocument(statement.Appendage.Name, false));
			}
			else
			{
				appendageBinder.Position = 0;
			}

			if (statement.Document != null)
			{
				documentBinder.Position = documentBinder.IndexOf(Project.Current.GetRealOrVirtualDocument(statement.Document.Name, false));
			}
			else
			{
				documentBinder.Position = 0;
			}
		}

		/// <summary>
		/// Update state of UI
		/// </summary>
		protected override void OnIdle()
		{
			bool bAlwaysCheck = AtInsertPosition() || ModifyMode;
			bool docAvailable = comboBoxDoc.Text.Length > 0;
			bool docNameValid = Project.Current.ValidDocumentNameForAppendStatement(comboBoxDoc.Text);
			bool appendageAvailable = comboBoxAppend.Text.Length > 0;

			buttonAddModify.Enabled = docAvailable && docNameValid && appendageAvailable && bAlwaysCheck;
		}

		/// <summary>
		/// Refresh the data sources for the combo boxes
		/// </summary>
		private void refreshDataSources()
		{
			// both combo boxes use the composite list of real and virtual documents
			UpdateDataSource(appendageBinder, Project.Current.RealOrVirtualDocumentList);
			UpdateDataSource(documentBinder, Project.Current.RealOrVirtualDocumentList);
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

		private void buttonAddModify_Click(object sender, System.EventArgs e)
		{
			RememberAction();

			// get statement's documents from names in combo boxes
			statement.Appendage = Project.Current.GetRealOrVirtualDocument(comboBoxAppend.Text, false);

			// a manually entered name will create a new virtual document, by virtue of the "true" arg
			string docName = comboBoxDoc.Text.Trim();
			comboBoxDoc.Text = docName;			// (put the trimmed name back into the text box)
			statement.Document = Project.Current.GetRealOrVirtualDocument(docName, true);

			// save the statement
			SaveStatement();

			// update the combo box lists because there may be a new virtual document
			refreshDataSources();

			//make sure the document box is current if a virtual document was added
			documentBinder.Position = documentBinder.IndexOf(Project.Current.GetRealOrVirtualDocument(docName, false));
		}

		private void tabPageDocument_Layout(object sender, LayoutEventArgs e)
		{
			if (ratioComboBoxAppend != 0.0 && lastTabPageWidth != tabPageDocument.Width && tabPageDocument.Width >= 240)
			{
				int width = tabPageDocument.Width;
				int oldRight = comboBoxAppend.Right;
				comboBoxAppend.Width = (int)(width * ratioComboBoxAppend);
				int offset = comboBoxAppend.Right - oldRight;
				labelTo.Left += offset;
				comboBoxDoc.Left += offset;
				comboBoxDoc.Width = width - 8 - comboBoxDoc.Left;
			}
			lastTabPageWidth = tabPageDocument.Width;
		}
	}

	// Work around for VSN Form Designer issue with Generics

	public class AppendStatementViewBase : StatementView<AppendStatement>
	{
	}
}
