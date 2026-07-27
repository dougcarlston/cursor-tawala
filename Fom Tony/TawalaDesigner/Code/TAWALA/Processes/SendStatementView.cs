// $Workfile: SendStatementView.cs $
// $Revision: 13 $	$Date: 12/12/07 4:57p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;

namespace Tawala.Processes
{
	/// <summary>
	/// Summary description for SendDetails.
	/// </summary>
    public partial class SendStatementView : SendStatementViewBase
	{
		// original control sizes saved for layout
		private int originalTextBoxEmailRecipientWidth;
		private int originalTabPageEmailWidth;
		private int lastTabPageEmailWidth;

		private BindingSource documentBinder = new BindingSource();

		private bool textBoxEmailRecipientSelected;
		private bool textBoxEmailCCSelected;
		private bool textBoxEmailSubjectSelected;
		private bool textBoxEmailFromAddressSelected;
		private bool textBoxEmailFromAliasSelected;

		public SendStatementView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			originalTabPageEmailWidth = lastTabPageEmailWidth = tabPageEmail.Width;
			originalTextBoxEmailRecipientWidth = textBoxEmailRecipient.Width;

			comboBoxEmailDoc.DataSource = documentBinder;

			Project.Events.DocumentChanged += events_ComponentChanged;
			Project.Events.ComponentRenamed += events_ComponentChanged;
			Project.Events.ComponentAdded += events_ComponentChanged;
			Project.Events.ComponentRemoved += events_ComponentChanged;

		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			FieldsPalette.Palette.FieldNodeDoubleClick += new TreeNodeMouseClickEventHandler(palette_FieldNodeDoubleClick);
			Debug.WriteLine("SendStatementView - OnEnter");
		}

		private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node != null)
			{
				IPaletteField field = e.Node.Tag as IPaletteField;

				if (field != null)
				{
					if (textBoxEmailRecipientSelected)
					{
						textBoxEmailRecipient.Tag = field;
						textBoxEmailRecipient.Text = ((IPaletteField)field).QualifiedFieldName;

						textBoxEmailCC.Focus();
					}
					else if (textBoxEmailCCSelected)
					{
						textBoxEmailCC.Tag = field;
						textBoxEmailCC.Text = ((IPaletteField)field).QualifiedFieldName;

						textBoxEmailFromAddress.Focus();
					}
					else if (textBoxEmailFromAddressSelected)
					{
						textBoxEmailFromAddress.Tag = field;
						textBoxEmailFromAddress.Text = ((IPaletteField)field).QualifiedFieldName;
						textBoxEmailFromAlias.Focus();
					}
					else if (textBoxEmailFromAliasSelected)
					{
						textBoxEmailFromAlias.Tag = field;
						textBoxEmailFromAlias.Text = ((IPaletteField)field).QualifiedFieldName;
						textBoxEmailSubject.Focus();
					}
					else if (textBoxEmailSubjectSelected)
					{
						textBoxEmailSubject.Insert((IPaletteField)e.Node.Tag);
					}
				}
			}
		}

		private void events_ComponentChanged(object sender, ComponentEventArgs e)
		{
			if (e.Component is Document)
			{
				UpdateDataSource(documentBinder, Project.Current.RealOrVirtualDocumentList);
			}
		}

		protected override void NewStatement()
		{
			statement = new SendStatement();
			statement.SendBody = new SendDocumentBody(Document.NULL);
		}

		protected override void OnEdit()
		{
			ParentView.SplitContainer.SplitterDistance = 240;

			UpdateDataSource(documentBinder, Project.Current.RealOrVirtualDocumentList);

			textBoxEmailRecipient.Text = statement.AddressTo.Text;
			textBoxEmailRecipient.Tag = null;
			if (statement.AddressTo.Type == FieldOrLiteral.StringType.field)
			{
				textBoxEmailRecipient.Tag = ParentView.Process.MappedFields.AllFields[statement.AddressTo.Text];
			}

			setFromAddress();
			setFromAlias();

			textBoxEmailCC.Text = statement.AddressCc.Text;
			textBoxEmailSubject.Text = statement.Subject;

			setDocument();

			textBoxEmailRecipient.Focus();
		}

		private void setDocument()
		{
			SendDocumentBody body = statement.SendBody as SendDocumentBody;
			if (body != null)
			{
				checkBoxReset.Checked = body.ResetDocumentAfterSend;

				if (Project.Current.PageHeader.HasContent())
				{
					checkBoxShowPageHeader.Enabled = true;
					checkBoxShowPageHeader.Checked = body.ShowPageHeader;
				}
				else
				{
					checkBoxShowPageHeader.Checked = false;
					checkBoxShowPageHeader.Enabled = false;
				}

				int index = documentBinder.IndexOf(body.Document);
				if (index >= 0)
				{
					documentBinder.Position = index;
				}
			}
		}

		private void setFromAddress()
		{
			textBoxEmailFromAddress.Text = "";
			textBoxEmailFromAddress.Tag = null;

			if (statement.AddressFrom.HasSingleFieldElement)
			{
				IPaletteField addressField = ((FieldElement)statement.AddressFrom.Elements[0]).Field;
				textBoxEmailFromAddress.Tag = addressField;
				textBoxEmailFromAddress.Text = addressField.QualifiedFieldName;
			}
			else if (statement.AddressFrom.HasSingleStringElement)
			{
				String addressLiteral = ((StringElement)statement.AddressFrom.Elements[0]).Text;
				textBoxEmailFromAddress.Tag = null;
				textBoxEmailFromAddress.Text = addressLiteral;
			}
		}

		private void setFromAlias()
		{
			textBoxEmailFromAlias.Text = "";
			textBoxEmailFromAlias.Tag = null;

			if (statement.AliasFrom.HasSingleFieldElement)
			{
				IPaletteField addressField = ((FieldElement)statement.AliasFrom.Elements[0]).Field;
				textBoxEmailFromAlias.Tag = addressField;
				textBoxEmailFromAlias.Text = addressField.QualifiedFieldName;
			}
			else if (statement.AliasFrom.HasSingleStringElement)
			{
				String addressLiteral = ((StringElement)statement.AliasFrom.Elements[0]).Text;
				textBoxEmailFromAlias.Tag = null;
				textBoxEmailFromAlias.Text = addressLiteral;
			}
		}

		protected override void OnIdle()
		{
			bool bAlwaysCheck = AtInsertPosition() || ModifyMode;

			buttonAddModifyDoc.Enabled = bAlwaysCheck && textBoxEmailRecipient.Text.Length > 0 && textBoxEmailSubject.Text.Length > 0 && comboBoxEmailDoc.Text.Length > 0;
		}

		#region Send Document Controls Events

		private void tabPageDocument_Layout(object sender, LayoutEventArgs e)
		{
			int width = tabPageEmail.Width;

			if (originalTextBoxEmailRecipientWidth != 0 && lastTabPageEmailWidth != width && width > 240)
			{
				textBoxEmailRecipient.Width = originalTextBoxEmailRecipientWidth + (width - originalTabPageEmailWidth) / 2; ;
				labelEmailCC.Left = textBoxEmailRecipient.Right + 8;
				textBoxEmailCC.Left = labelEmailCC.Right + 8;
				textBoxEmailCC.Width = width - 8 - textBoxEmailCC.Left;
				textBoxEmailFromAddress.Width = textBoxEmailRecipient.Width;
				labelEmailFromAlias.Left = labelEmailCC.Left;
				textBoxEmailFromAlias.Left = textBoxEmailCC.Left;
				textBoxEmailFromAlias.Width = textBoxEmailCC.Width;
				textBoxEmailSubject.Width = width - 8 - textBoxEmailSubject.Left;

				comboBoxEmailDoc.Left = labelEmailDoc.Right + 8;
				comboBoxEmailDoc.Width = Math.Max(100, width - 8 - comboBoxEmailDoc.Left);
			}

			lastTabPageEmailWidth = width;
		}

		private void buttonAddModifyDoc_Click(object sender, System.EventArgs e)
		{
			RememberAction();

			addOrModifyRecipientAddress();
			addOrModifyCCAddress();
			addOrModifyFromAddress();
			addOrModifyFromAlias();

			statement.Subject = textBoxEmailSubject.Text;

			SendDocumentBody body = new SendDocumentBody(Project.Current.GetRealOrVirtualDocument(comboBoxEmailDoc.Text, false));
			body.ResetDocumentAfterSend = checkBoxReset.Checked;

			if (Project.Current.PageHeader.HasContent())
			{
				body.ShowPageHeader = checkBoxShowPageHeader.Checked;
			}

			statement.SendBody = body;

			SaveStatement();
		}

		private void addOrModifyRecipientAddress()
		{
			IPaletteField recipientField = textBoxEmailRecipient.Tag as IPaletteField;
			if (recipientField is RecordField)
			{
				statement.AddressTo = new FieldOrLiteral(recipientField as RecordField);
			}
			else
			{
				statement.AddressTo = new FieldOrLiteral();
				statement.AddressTo.Text = textBoxEmailRecipient.Text;

				statement.AddressTo.Type = FieldOrLiteral.StringType.literal;

				if (recipientField != null)
				{
					if (recipientField.QualifiedFieldName.Equals(textBoxEmailRecipient.Text))
					{
						statement.AddressTo.Type = FieldOrLiteral.StringType.field;
					}
				}
			}
		}

		private void addOrModifyCCAddress()
		{
			IPaletteField ccField = textBoxEmailCC.Tag as IPaletteField;
			if (ccField is RecordField)
			{
				statement.AddressCc = new FieldOrLiteral(ccField as RecordField);
			}
			else
			{
				statement.AddressCc = new FieldOrLiteral();
				statement.AddressCc.Text = textBoxEmailCC.Text;

				statement.AddressCc.Type = FieldOrLiteral.StringType.literal;

				if (ccField != null)
				{
					if (ccField.QualifiedFieldName.Equals(textBoxEmailCC.Text))
					{
						statement.AddressCc.Type = FieldOrLiteral.StringType.field;
					}
				}
			}
		}

		private void addOrModifyFromAddress()
		{
			IPaletteField fromField = textBoxEmailFromAddress.Tag as IPaletteField;

			if (fromField is RecordField)
			{
				statement.AddressFrom = new Expression(fromField as RecordField);
			}
			else
			{
				if (fromField != null)
				{
					statement.AddressFrom = new Expression(fromField);
				}
				else
				{
					statement.AddressFrom = new Expression(textBoxEmailFromAddress.Text);
				}
			}
		}

		private void addOrModifyFromAlias()
		{
			IPaletteField fromField = textBoxEmailFromAlias.Tag as IPaletteField;

			if (fromField is RecordField)
			{
				statement.AliasFrom = new Expression(fromField as RecordField);
			}
			else
			{
				if (fromField != null)
				{
					statement.AliasFrom = new Expression(fromField);
				}
				else
				{
					statement.AliasFrom = new Expression(textBoxEmailFromAlias.Text);
				}
			}
		}

		#endregion

		private void textBoxEmailRecipient_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.Copy)
			{
				IField field = textBoxEmailRecipient.DraggedField(e.Data);

				if (field != null)
				{
					textBoxEmailRecipient.Text = ((IPaletteField)field).QualifiedFieldName;
				}

				textBoxEmailRecipient.Tag = field;
				textBoxEmailCC.Focus();
			}
		}

		private void textBoxEmailRecipient_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (textBoxEmailRecipient.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		private void textBoxEmailRecipient_KeyPress(object sender, KeyPressEventArgs e)
		{
			textBoxEmailRecipient.Tag = null;
		}

		private void textBoxEmailRecipient_Enter(object sender, EventArgs e)
		{
			selectTextBoxEmailRecipient();
			FieldsPalette.Palette.FieldNodeDoubleClick += new TreeNodeMouseClickEventHandler(palette_FieldNodeDoubleClick);
		}

		private void selectTextBoxEmailRecipient()
		{
			unselectAllTextBoxes();

			textBoxEmailRecipientSelected = true;
			textBoxEmailRecipient.BackColor = SelectedColor;
		}

		private void textBoxEmailCC_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.Copy)
			{
				IField field = textBoxEmailCC.DraggedField(e.Data);

				if (field != null)
				{
					textBoxEmailCC.Text = ((IPaletteField)field).QualifiedFieldName;
				}

				textBoxEmailCC.Tag = field;
				textBoxEmailFromAddress.Focus();
			}
		}

		private void textBoxEmailCC_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (textBoxEmailCC.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		private void textBoxEmailCC_KeyPress(object sender, KeyPressEventArgs e)
		{
			textBoxEmailCC.Tag = null;
		}

		private void textBoxEmailCC_Enter(object sender, EventArgs e)
		{
			selectTextBoxEmailCC();
			FieldsPalette.Palette.FieldNodeDoubleClick += new TreeNodeMouseClickEventHandler(palette_FieldNodeDoubleClick);
		}

		private void selectTextBoxEmailCC()
		{
			unselectAllTextBoxes();

			textBoxEmailCCSelected = true;
			textBoxEmailCC.BackColor = SelectedColor;
		}

		private void unselectAllTextBoxes()
		{
			textBoxEmailSubjectSelected = false;
			textBoxEmailSubject.BackColor = UnselectedColor;

			textBoxEmailRecipientSelected = false;
			textBoxEmailRecipient.BackColor = UnselectedColor;

			textBoxEmailCCSelected = false;
			textBoxEmailCC.BackColor = UnselectedColor;

			textBoxEmailFromAddressSelected = false;
			textBoxEmailFromAddress.BackColor = UnselectedColor;

			textBoxEmailFromAliasSelected = false;
			textBoxEmailFromAlias.BackColor = UnselectedColor;
		}

		private void textBoxEmailFromAddress_Enter(object sender, EventArgs e)
		{
			selectTextBoxEmailFromAddress();
			FieldsPalette.Palette.FieldNodeDoubleClick += new TreeNodeMouseClickEventHandler(palette_FieldNodeDoubleClick);
		}

		private void textBoxEmailFromAddress_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.Copy)
			{
				IField field = textBoxEmailFromAddress.DraggedField(e.Data);

				if (field != null)
				{
					textBoxEmailFromAddress.Text = ((IPaletteField)field).QualifiedFieldName;
				}

				textBoxEmailFromAddress.Tag = field;
				textBoxEmailFromAlias.Focus();
			}
		}

		private void textBoxEmailFromAddress_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (textBoxEmailFromAddress.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		private void textBoxEmailFromAddress_KeyPress(object sender, KeyPressEventArgs e)
		{
			textBoxEmailFromAddress.Tag = null;
		}

		private void textBoxEmailFromAddress_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBoxEmailFromAddress.Text))
			{
				textBoxEmailFromAddress.Tag = null;
			}
		}

		private void selectTextBoxEmailFromAddress()
		{
			unselectAllTextBoxes();

			textBoxEmailFromAddressSelected = true;
			textBoxEmailFromAddress.BackColor = SelectedColor;
		}

		private void textBoxEmailFromAlias_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.Copy)
			{
				IField field = textBoxEmailFromAlias.DraggedField(e.Data);

				if (field != null)
				{
					textBoxEmailFromAlias.Text = ((IPaletteField)field).QualifiedFieldName;
				}

				textBoxEmailFromAlias.Tag = field;
				textBoxEmailSubject.Focus();
			}
		}

		private void textBoxEmailFromAlias_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (textBoxEmailFromAlias.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);

		}

		private void textBoxEmailFromAlias_Enter(object sender, EventArgs e)
		{
			selectTextBoxEmailFromAlias();
			FieldsPalette.Palette.FieldNodeDoubleClick += new TreeNodeMouseClickEventHandler(palette_FieldNodeDoubleClick);
		}

		private void textBoxEmailFromAlias_KeyPress(object sender, KeyPressEventArgs e)
		{
			textBoxEmailFromAlias.Tag = null;
		}

		private void textBoxEmailFromAlias_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(textBoxEmailFromAlias.Text))
			{
				textBoxEmailFromAlias.Tag = null;
			}
		}

		private void selectTextBoxEmailFromAlias()
		{
			unselectAllTextBoxes();

			textBoxEmailFromAliasSelected = true;
			textBoxEmailFromAlias.BackColor = SelectedColor;
		}

		private void textBoxEmailSubject_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = (textBoxEmailSubject.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
		}

		private void textBoxEmailSubject_DragDrop(object sender, DragEventArgs e)
		{
			IField field = textBoxEmailSubject.DraggedField(e.Data);

			textBoxEmailSubject.Insert(field);
		}

		private void textBoxEmailSubject_Enter(object sender, EventArgs e)
		{
			selectTextBoxEmailSubject();
		}

		private void selectTextBoxEmailSubject()
		{
			unselectAllTextBoxes();

			textBoxEmailSubjectSelected = true;
			textBoxEmailSubject.BackColor = SelectedColor;
		}

		protected override void OnMdiWindowActivated()
		{
			base.OnMdiWindowActivated();

			textBoxEmailRecipient.BackColor = SelectedColor;
		}

		protected override void OnMdiWindowDeactivated()
		{
			base.OnMdiWindowDeactivated();

			textBoxEmailRecipient.BackColor = UnselectedColor;
		}
	}

	// Work around for VSN Form Designer issue with Generics

    public class SendStatementViewBase : StatementView<SendStatement>
	{
	}
}
