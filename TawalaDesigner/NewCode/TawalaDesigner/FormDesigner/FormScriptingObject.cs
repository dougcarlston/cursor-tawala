// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tawala.Browser;
using Tawala.FormDesigner.Dialogs.SkipInstructionsDialog;
using Tawala.FormDesigner.FormItemOptions;
using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Form=System.Windows.Forms.Form;

namespace Tawala.FormDesigner
{
	[ComVisible(true)]
	public class FormScriptingObject : BrowserScriptingObject
	{
		private readonly IFormView view;
		private string activeElementId;
		private McqChoicesView editChoicesDialog;
		private string mcqElementId;

		public FormScriptingObject(IFormView view)
		{
			this.view = view;
		}

		private static FormItemOptionsDialog formItemOptionsDialog
		{
			get { return TawalaFormDesigner.FormItemOptionsDialog; }
		}

		public HtmlElement ActiveFormItem
		{
			get { return (activeElementId == null ? null : view.GetHtmlElementById(activeElementId)); }
		}

		public void DeleteSelectedItems()
		{
			view.DeleteSelectedItems();
		}

		public void CopySelectedItems()
		{
			view.CopySelectedItems();
		}

		public void InsertBlank(string fibHtmlId)
		{
			view.Presenter.InsertBlank(fibHtmlId);
		}

		public void FormItemsMoved()
		{
			view.Presenter.FormItemsMoved();
		}

		public void SingleFormItemSelected(string htmlId)
		{
			IFormItem formItem = view.Presenter.GetFormItem(htmlId);

			if (formItem != null)
			{
				formItemDeactivated();
			}
		}

		private void formItemActivated(string id)
		{
			activeElementId = id;
			view.FormItemIsActive = true;
		}

		private void formItemDeactivated()
		{
			activeElementId = null;
			view.FormItemIsActive = false;
		}

		public void HeadingItemActivated(string headingItemId)
		{
			formItemActivated(headingItemId);
			//activeElementId = headingItemId;

			var headingItem = view.Presenter.GetFormItem(headingItemId) as IHeadingItem;

			showHeadingItemOptions(headingItem);
		}

		private static void showHeadingItemOptions(IHeadingItem headingItem)
		{
			formItemOptionsDialog.Controls.Clear();
			formItemOptionsDialog.Controls.Add(new HeadingOptionsView(headingItem));
			formItemOptionsDialog.Visible = false;
			formItemOptionsDialog.Show(ApplicationPresenter.MainApplicationForm);
		}

		public void TextItemActivated(string textItemId)
		{
			formItemActivated(textItemId);
			//activeElementId = textItemId;

			var textItem = view.Presenter.GetFormItem(textItemId) as ITextItem;

			showTextItemOptions(textItem);
		}

		private static void showTextItemOptions(ITextItem textItem)
		{
			formItemOptionsDialog.Controls.Clear();
			formItemOptionsDialog.Controls.Add(new TextOptionsView(textItem));
			formItemOptionsDialog.Visible = false;
			formItemOptionsDialog.Show(ApplicationPresenter.MainApplicationForm);
		}

		public void McqItemActivated(string mcqItemId)
		{
			formItemActivated(mcqItemId);
			//activeElementId = mcqItemId;

			var mcqItem = view.Presenter.GetFormItem(mcqItemId) as IMcqItem;

			showMcqItemOptions(mcqItem);
		}

		private static void showMcqItemOptions(IMcqItem mcqItem)
		{
			formItemOptionsDialog.Controls.Clear();
			formItemOptionsDialog.Controls.Add(new McqOptionsView(mcqItem));
			formItemOptionsDialog.Visible = false;
			formItemOptionsDialog.Show(ApplicationPresenter.MainApplicationForm);
		}

		public void FibItemActivated(string fibItemId, string blankId)
		{
			formItemActivated(fibItemId);
			//activeElementId = fibItemId;

			var fibItem = view.Presenter.GetFormItem(fibItemId) as NewFibItem;
			NewBlank blank = null;

			if (!string.IsNullOrEmpty(blankId))
			{
				blank = Project.FieldMapById[Convert.ToInt32(blankId)] as NewBlank;
			}

			showFibItemOptions(fibItem, blank);
		}

		private static void showFibItemOptions(IFibItem fibItem, IBlank blank)
		{
			formItemOptionsDialog.Controls.Clear();
			formItemOptionsDialog.Controls.Add(new FibOptionsView(fibItem, blank));
			formItemOptionsDialog.Visible = false;
			formItemOptionsDialog.Show(ApplicationPresenter.MainApplicationForm);
		}

		public void FormItemDeactivated(string id)
		{
			if (activeElementId == id)
			{
				activeElementId = null;
				view.FormItemIsActive = false;
			}

			formItemOptionsDialog.Hide();
		}

		public void HiddenFieldChanged()
		{
			Project.Events.RaiseSynchronizeProjectEvent();
		}

		public void EditSkipInstructions(string skipElementId)
		{
			var skipInstructionsItem = view.Presenter.GetFormItem(skipElementId) as ISkipInstructionsItem;

			view.Presenter.MakeFormActiveComponent();
			var skipInstructionsView = new SkipInstructionsView(view, skipInstructionsItem);
			skipInstructionsView.Location = centeredInClientLocation(skipInstructionsView);
			skipInstructionsView.Show();
		}

		public void EditMcqChoices(string mcqElementId)
		{
			this.mcqElementId = mcqElementId;
			var mcqItem = view.Presenter.GetFormItem(mcqElementId) as IMcqItem;

			editChoicesDialog = new McqChoicesView(mcqItem);

			editChoicesDialog.FormClosed += editChoicesDialog_FormClosed;

			editChoicesDialog.Location = centeredInClientLocation(editChoicesDialog);
			editChoicesDialog.Show();
		}

		private static Point centeredInClientLocation(Form form)
		{
			Form mainWindow = Application.OpenForms[0];
			int x = ((mainWindow.ClientRectangle.Width - 400)/2) - (form.Width/2);
			int y = (mainWindow.ClientRectangle.Height/2) - (form.Height/2);
			return (new Point(x, y));
		}

		private void editChoicesDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (editChoicesDialog.DialogResult == DialogResult.OK)
			{
				view.Presenter.UpdateMcqChoicesInView(mcqElementId);
			}
		}

		public string GetBlankId(string blankHtml)
		{
			return view.Presenter.GetBlankId(blankHtml);
		}
	}
}