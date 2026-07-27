// $Workfile: ShowStatementView.cs $
// $Revision: 14 $	$Date: 2/28/08 2:01p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Controls;
using Tawala.ProjectUI;

namespace Tawala.Processes
{
    public partial class ShowStatementView : ShowStatementViewBase
	{
		private ShowDocumentStatement showDocumentStatement = new ShowDocumentStatement();
		private ShowFormStatement showFormStatement = new ShowFormStatement();
		private ShowRecordStatement showRecordStatement = null;
		private ShowUrlStatement showUrlStatement = new ShowUrlStatement("");

		private const int documentDetailsPanelHeight = 170;
		private const int formDetailsPanelHeight = 135;
		private const int recordDetailsPanelHeight = 200;
		private const int urlDetailsPanelHeight = 170;

		// control size for layout as tabpage changes width
		int lastDocTabPageWidth = 0;
		int lastFormTabPageWidth = 0;
		int lastUrlTabPageWidth = 0;

		private BindingSource documentBinder = new BindingSource();
		private BindingSource formBinder = new BindingSource();
		private BindingSource recordBinder = new BindingSource();

		private ConditionGroupCollection groups;
		private int conditionGroupHeight = 0;
		
		public ShowStatementView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			comboBoxDoc.DataSource = documentBinder;
			comboBoxForm.DataSource = formBinder;
			comboBoxRecordForm.DataSource = recordBinder;

			Project.Events.DocumentChanged += Events_DocumentOrFormChanged;
			Project.Events.ComponentRenamed += Events_DocumentOrFormChanged;
            Project.Events.ComponentAdded += Events_DocumentOrFormChanged;
			Project.Events.ComponentRemoved += Events_DocumentOrFormChanged;

			groupBoxWhere.ControlAdded += groupBoxWhere_ControlAdded;
			groupBoxWhere.ControlRemoved += groupBoxWhere_ControlRemoved;

			groupBoxWhere.Width = tabPageStoredRecord.Width - groupBoxWhere.Left - 8;
			groupBoxWhere.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
		}

		void groupBoxWhere_ControlAdded(object sender, ControlEventArgs e)
		{
			ConditionGroup group = e.Control as ConditionGroup;

			if (conditionGroupHeight == 0)
			{
				conditionGroupHeight = group.Height;
			}

			groupBoxWhere.Height = conditionGroupHeight * groupBoxWhere.Controls.Count + conditionGroupHeight;

			group.TextBoxField.Enter += new EventHandler(textBoxField_Enter);
			group.TextBoxExpression.Enter += new EventHandler(textBoxExpression_Enter);

			group.Top = conditionGroupHeight * (groupBoxWhere.Controls.Count - 1) + conditionGroupHeight / 2;

			buttonAddModifyRecord.Top = groupBoxWhere.Bottom + 8;

			group.Visible = true;

			autoSizeHeight();

			adjustTabs();
		}

		void textBoxField_Enter(object sender, EventArgs e)
		{
			groups.SelectTextBox(sender);
		}

		void textBoxExpression_Enter(object sender, EventArgs e)
		{
			groups.SelectTextBox(sender);
		}

		private void autoSizeHeight()
		{
			if (groupBoxWhere.Controls.Count >= 1)
			{
				Point pt = tabPageStoredRecord.PointToScreen(new Point(buttonAddModifyRecord.Left, buttonAddModifyRecord.Bottom));
				pt = splitContainer.Panel1.PointToClient(pt);
				if (pt.Y > 0)
				{
					splitContainer.SplitterDistance = pt.Y + 8;
					Height = splitContainer.SplitterDistance;
				}
			}
		}

		private void adjustTabs()
		{
			for (int i = 0; i < groupBoxWhere.Controls.Count; ++i)
			{
				groupBoxWhere.Controls[i].TabIndex = i;
			}
		}

		private SplitContainer splitContainer;

		void groupBoxWhere_ControlRemoved(object sender, ControlEventArgs e)
		{
			if (groupBoxWhere.Controls.Count >= 1)
			{
				groupBoxWhere.Height -= conditionGroupHeight;
				buttonAddModifyRecord.Top = groupBoxWhere.Bottom + 8;
				autoSizeHeight();
			}
			adjustTabs();
		}

		void Events_DocumentOrFormChanged(object sender, ComponentEventArgs e)
		{
			UpdateDataSource(documentBinder, Project.Current.RealOrVirtualDocumentList);
			UpdateDataSource(formBinder, Project.Current.FormList);
			UpdateDataSource(recordBinder, Project.Current.FormList);
		}

		protected override void NewStatement()
		{
			if (tabControl.SelectedTab == tabPageDocument)
			{
				showDocumentStatement = new ShowDocumentStatement();
			}

			if (tabControl.SelectedTab == tabPageForm)
			{
				showFormStatement = new ShowFormStatement();
			}

			if (tabControl.SelectedTab == tabPageStoredRecord)
			{
				createNewShowRecordStatement();
			}

			if (tabControl.SelectedTab == tabPageUrl)
			{
				showUrlStatement = new ShowUrlStatement("");
			}

			selectStatement();
		}

		private void createNewShowRecordStatement()
		{
			showRecordStatement = new ShowRecordStatement();
			scratchStatement = ShowRecordStatement.ShallowCopy(showRecordStatement);

			radioButtonCreate.Checked = !scratchStatement.ModifyOnSubmit;
			radioButtonModify.Checked = scratchStatement.ModifyOnSubmit;

			groupBoxWhere.Hide();
			groups.Conditions = new Conditions();
			groupBoxWhere.Show();

			deselectAllConditionsControls();

			updateConditionForms();
		}

		private void selectStatement()
		{
			if (tabControl.SelectedTab == tabPageDocument)
			{
				setDetailsPanelHeight(documentDetailsPanelHeight);
				statement = showDocumentStatement;
			}

			if (tabControl.SelectedTab == tabPageForm)
			{
				setDetailsPanelHeight(formDetailsPanelHeight);
				statement = showFormStatement;
			}

			if (tabControl.SelectedTab == tabPageStoredRecord)
			{
//				setDetailsPanelHeight(recordDetailsPanelHeight);
				autoSizeHeight();
				statement = showRecordStatement;
				updateConditionForms();
			}

			if (tabControl.SelectedTab == tabPageUrl)
			{
				statement = showUrlStatement;
			}

		}

		ShowRecordStatement scratchStatement;

		/// <summary>
		/// This function handles editing a new or existing statement.
		/// </summary>
		protected override void OnEdit()
		{
			UpdateDataSource(documentBinder, Project.Current.RealOrVirtualDocumentList);
			UpdateDataSource(formBinder, Project.Current.FormList);
			UpdateDataSource(recordBinder, Project.Current.FormList);

			if (statement is ShowDocumentStatement)
			{
				setDetailsPanelHeight(documentDetailsPanelHeight);

				showDocumentStatement = (ShowDocumentStatement)statement;
				tabControl.SelectedTab = tabPageDocument;

				if (statement.Document != null)
				{
					documentBinder.Position = documentBinder.IndexOf(Project.Current.GetRealOrVirtualDocument(statement.Document.Name, false));
				}
				checkBoxReset.Checked = showDocumentStatement.ResetAfterShow;
			}
			else if (statement is ShowFormStatement)
			{
				setDetailsPanelHeight(formDetailsPanelHeight);

				showFormStatement = (ShowFormStatement)statement;
				tabControl.SelectedTab = tabPageForm;

				if (statement.Form != null)
				{
					formBinder.Position = formBinder.IndexOf(Project.Current.GetForm(statement.Form.Name));
				}
			}
			else if (statement is ShowRecordStatement)
			{
				setDetailsPanelHeight(recordDetailsPanelHeight);

				showRecordStatement = (ShowRecordStatement)statement;
				tabControl.SelectedTab = tabPageStoredRecord;

				groupBoxWhere.Hide();

				if (ModifyMode)
				{
					scratchStatement = ShowRecordStatement.ShallowCopy(statement as ShowRecordStatement);
				}

				if (scratchStatement.Form != null)
				{
					recordBinder.Position = recordBinder.IndexOf(Project.Current.GetForm(scratchStatement.Form.Name));
					comboBoxRecordForm.SelectedItem = scratchStatement.Form;
					radioButtonCreate.Checked = !scratchStatement.ModifyOnSubmit;
					radioButtonModify.Checked = scratchStatement.ModifyOnSubmit;
					groups.Conditions = scratchStatement.Conditions;
				}

				broadcastStatementChange();

				autoSizeHeight();

				deselectAllConditionsControls();

				groupBoxWhere.Show();

				comboBoxRecordForm.Focus();

				updateConditionForms();
			}
			else if (statement is ShowUrlStatement)
			{
				setDetailsPanelHeight(urlDetailsPanelHeight);

				showUrlStatement = (ShowUrlStatement)statement;
				textBoxUrl.Text = showUrlStatement.Url;
				tabControl.SelectedTab = tabPageUrl;
			}
		}

		private void editRecordStatement()
		{
		}

		private void deselectAllConditionsControls()
		{
			groups.SelectTextBox(null);
		}

		private void broadcastStatementChange()
		{
			if (scratchStatement != null)
			{
				if (ParentView != null)
				{
					Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(ParentView.Process, scratchStatement));
				}
			}
		}

		protected override void OnMdiWindowDeactivated()
		{
			Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs());
			FieldsPalette.Palette.ConditionsForms = Tawala.Projects.FunctionFormCollection.NULL;
			base.OnMdiWindowDeactivated();
		}

		protected override void OnMdiWindowActivated()
		{
			base.OnMdiWindowActivated();
			groups.UpdateFieldsPaletteChoices();
			updateConditionForms();
		}

		/// <summary>
		/// Update state of UI
		/// </summary>
		protected override void OnIdle()
		{
			if (tabControl.SelectedTab == tabPageDocument)
			{
				bool bAlwaysCheck = AtInsertPosition() || ModifyMode;
				bool docAvailable = comboBoxDoc.Text.Length > 0;
				buttonAddModifyDoc.Enabled = docAvailable && bAlwaysCheck;

				FieldsPalette.Palette.ClearConditionsForms();
			}
			else if (tabControl.SelectedTab == tabPageForm)
			{
				bool bAlwaysCheck = AtInsertPosition() || ModifyMode;
				bool formAvailable = comboBoxForm.Text.Length > 0;
				buttonAddModifyForm.Enabled = formAvailable && bAlwaysCheck;

				FieldsPalette.Palette.ClearConditionsForms();
			}
			else if (tabControl.SelectedTab == tabPageStoredRecord)
			{
				bool bAlwaysCheck = AtInsertPosition() || ModifyMode;
				bool formAvailable = comboBoxRecordForm.Text.Length > 0;
				buttonAddModifyRecord.Enabled = formAvailable && bAlwaysCheck && groups.WhereConditionsAreValid();
			}
			else if (tabControl.SelectedTab == tabPageUrl)
			{
				buttonAddModifyUrl.Enabled = textBoxUrl.Text != "";
			}
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			if (Parent != null && tabControl.SelectedTab == tabPageStoredRecord && comboBoxRecordForm.Items.Count > 0)
			{
				FieldsPalette.Palette.ConditionsForms = new Tawala.Projects.FunctionFormCollection(comboBoxRecordForm.SelectedItem as IForm);
			}
			else
			{
				FieldsPalette.Palette.ClearConditionsForms();
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			splitContainer = GetSplitContainer();

			groups = new ConditionGroupCollection(groupBoxWhere, comboBoxAllAny, true);
		}

		#region Project Events

		protected override void OnCurrentComponentSet(Tawala.Projects.Component c)
		{
			if (c is Projects.Process)
			{
				UpdateDataSource(documentBinder, Project.Current.RealOrVirtualDocumentList);
				UpdateDataSource(formBinder, Project.Current.FormList);
				UpdateDataSource(recordBinder, Project.Current.FormList);
			}
		}

		#endregion

		private void buttonAddModifyDoc_Click(object sender, System.EventArgs e)
		{
			RememberAction();

			showDocumentStatement.Document = Project.Current.GetRealOrVirtualDocument(comboBoxDoc.Text, false);
			showDocumentStatement.ResetAfterShow = checkBoxReset.Checked;

			selectStatement();
			SaveStatement();
		}

		private void buttonAddModifyForm_Click(object sender, System.EventArgs e)
		{
			RememberAction();

			showFormStatement.Form = Project.Current.GetForm(comboBoxForm.Text);

			selectStatement();
			SaveStatement();
		}

		private void buttonAddModifyRecord_Click(object sender, EventArgs e)
		{
			RememberAction();

			showRecordStatement.Form = Project.Current.GetForm(comboBoxRecordForm.Text);
			showRecordStatement.ModifyOnSubmit = radioButtonModify.Checked;
			showRecordStatement.Conditions = groups.Conditions;

			selectStatement();
			SaveStatement();
		}

		private void buttonAddModifyUrl_Click(object sender, EventArgs e)
		{
			RememberAction();

			showUrlStatement.Url = textBoxUrl.Text;

			selectStatement();
			SaveStatement();
		}


		private void tabPageDocument_Layout(object sender, LayoutEventArgs e)
		{
			if (lastDocTabPageWidth != tabPageDocument.Width && tabPageDocument.Width >= 240)
			{
				comboBoxDoc.Width = tabPageDocument.Width - 8 - comboBoxDoc.Left;
			}
			lastDocTabPageWidth = tabControl.ClientSize.Width;
		}

		private void tabPageForm_Layout(object sender, LayoutEventArgs e)
		{
			if (lastFormTabPageWidth != tabPageForm.Width && tabPageForm.Width >= 240)
			{
				comboBoxForm.Width = tabPageForm.Width - 8 - comboBoxForm.Left;
			}
			lastFormTabPageWidth = tabControl.ClientSize.Width;
		}

		private void tabPageUrl_Layout(object sender, LayoutEventArgs e)
		{
			if (lastUrlTabPageWidth != tabPageUrl.Width && tabPageUrl.Width >= 240)
			{
				textBoxUrl.Width = tabPageUrl.Width - 8 - textBoxUrl.Left;
			}

			lastUrlTabPageWidth = tabControl.ClientSize.Width;
		}

		private void radioButtonCreate_CheckedChanged(object sender, EventArgs e)
		{
			showRecordStatement.ModifyOnSubmit = !radioButtonCreate.Checked;
		}

		private void radioButtonModify_CheckedChanged(object sender, EventArgs e)
		{
			showRecordStatement.ModifyOnSubmit = radioButtonModify.Checked;
		}

		private void comboBoxRecordForm_SelectionChangeCommitted(object sender, EventArgs e)
		{
			updateConditionForms();
		}

		private void updateConditionForms()
		{
			IForm form = comboBoxRecordForm.SelectedItem as IForm;
			FieldsPalette.Palette.ConditionsForms = form != null ? new Tawala.Projects.FunctionFormCollection(form) : Tawala.Projects.FunctionFormCollection.NULL;
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPage == tabPageStoredRecord)
			{
				if (showRecordStatement == null)
				{
					createNewShowRecordStatement();
				}
				else if (comboBoxRecordForm.Items.Count > 0)
				{
					FieldsPalette.Palette.ConditionsForms = new Tawala.Projects.FunctionFormCollection(comboBoxRecordForm.SelectedItem as IForm);
				}
				else
				{
					FieldsPalette.Palette.ClearConditionsForms();
				}
			}
		}

		private void comboBoxAllAny_VisibleChanged(object sender, EventArgs e)
		{
			if (comboBoxAllAny.Visible)
			{
				labelWhere2.Visible = true;
				if (comboBoxAllAny.SelectedIndex < 0)
				{
					comboBoxAllAny.SelectedIndex = 0;
				}
			}
			else
			{
				labelWhere2.Visible = false;
			}
		}

		private void groupBoxWhere_Layout(object sender, LayoutEventArgs e)
		{
			foreach (Control c in groupBoxWhere.Controls)
			{
				c.Left = 2;
				c.Width = groupBoxWhere.Width - 4;
				c.PerformLayout();
			}
		}
	}

	// Work around for VSN Form Designer issue with Generics

	public class ShowStatementViewBase : StatementView<ShowStatement>
	{
	}
}
