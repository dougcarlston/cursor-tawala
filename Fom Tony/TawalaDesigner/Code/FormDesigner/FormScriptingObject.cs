// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Tawala.FormDesigner.Dialogs.SkipInstructionsDialog;
using Tawala.FormDesigner.FormItemOptions;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.Functions.Runtime;
using Tawala.XmlSupport;
using Tawala.Interfaces;
using Tawala.MainApplication;

namespace Tawala.FormDesigner
{
	[System.Runtime.InteropServices.ComVisible(true)]
	public class FormScriptingObject : Browser.BrowserScriptingObject
	{
		private string activeElementId;

		private FormItemOptionsDialog formItemOptionsDialog
		{
			get { return TawalaFormDesigner.FormItemOptionsDialog; }
		}

		public FormScriptingObject(IFormView view)
		{
			this.view = view;
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
				activeElementId = htmlId;

				if (view.ParentView == null)
				{
					return;
				}
			}
		}

		public void FormItemActivated(string id)
		{
			activeElementId = id;
		}

		public void HeadingItemActivated(string headingItemId)
		{
			activeElementId = headingItemId;

			IHeadingItem headingItem = view.Presenter.GetFormItem(headingItemId) as IHeadingItem;

			showHeadingItemOptions(headingItem);
		}

		private void showHeadingItemOptions(IHeadingItem headingItem)
		{
			formItemOptionsDialog.Controls.Clear();
			formItemOptionsDialog.Controls.Add(new HeadingOptionsView(headingItem));
			formItemOptionsDialog.Visible = false;
			formItemOptionsDialog.Show(ApplicationPresenter.MainApplicationForm);
		}

		public void TextItemActivated(string textItemId)
		{
			activeElementId = textItemId;

			ITextItem textItem = view.Presenter.GetFormItem(textItemId) as ITextItem;

			showTextItemOptions(textItem);
		}

		private void showTextItemOptions(ITextItem textItem)
		{
			formItemOptionsDialog.Controls.Clear();
			formItemOptionsDialog.Controls.Add(new TextOptionsView(textItem));
			formItemOptionsDialog.Visible = false;
			formItemOptionsDialog.Show(ApplicationPresenter.MainApplicationForm);
		}

		public void McqItemActivated(string mcqId)
		{
			activeElementId = mcqId;

			IMcqItem mcqItem = view.Presenter.GetFormItem(mcqId) as IMcqItem;

			showMcqItemOptions(mcqItem);
		}

		private void showMcqItemOptions(IMcqItem mcqItem)
		{
			formItemOptionsDialog.Controls.Clear();
			formItemOptionsDialog.Controls.Add(new McqOptionsView(mcqItem));
			formItemOptionsDialog.Visible = false;
			formItemOptionsDialog.Show(ApplicationPresenter.MainApplicationForm);
		}

		public void FibItemActivated(string fibId, string blankId)
		{
			activeElementId = fibId;

			NewFibItem fibItem = view.Presenter.GetFormItem(fibId) as NewFibItem;
			NewBlank blank = null;

			if (!string.IsNullOrEmpty(blankId))
			{
				blank = Project.FieldMapById[Convert.ToInt32(blankId)] as NewBlank;
			}

			showFibItemOptions(fibItem, blank);
		}

		private void showFibItemOptions(NewFibItem fibItem, NewBlank blank)
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
			}

			formItemOptionsDialog.Hide();
		}

		public void HiddenFieldChanged()
		{
			Project.Events.RaiseSynchronizeProjectEvent();
		}

		public HtmlElement ActiveFormItem
		{
			get { return (activeElementId == null ? null : view.GetHtmlElementById(activeElementId)); }
		}

		public void EditSkipInstructions(string skipElementId)
		{
			ISkipInstructionsItem skipInstructionsItem = view.Presenter.GetFormItem(skipElementId) as ISkipInstructionsItem;

			view.Presenter.MakeFormActiveComponent();
			SkipInstructionsView skipInstructionsView = new SkipInstructionsView(view, skipInstructionsItem);
			skipInstructionsView.Location = centeredLocation(skipInstructionsView);
			skipInstructionsView.Show(view as System.Windows.Forms.Form);
		}

		private McqChoicesView editChoicesDialog;
		private string mcqElementId;

		public void EditMcqChoices(string mcqElementId)
		{
			this.mcqElementId = mcqElementId;
			IMcqItem mcqItem = view.Presenter.GetFormItem(mcqElementId) as IMcqItem;

			editChoicesDialog = new McqChoicesView(mcqItem);

			editChoicesDialog.FormClosed += new FormClosedEventHandler(editChoicesDialog_FormClosed);

			editChoicesDialog.Location = centeredLocation(editChoicesDialog);
			editChoicesDialog.Show(view as System.Windows.Forms.Form);
		}

		private Point centeredLocation(System.Windows.Forms.Form form)
		{
			System.Windows.Forms.Form mainWindow = Application.OpenForms[0];
			int x = mainWindow.Left + ((mainWindow.Width / 2) - (form.Width / 2));
			int y = mainWindow.Top + ((mainWindow.Height / 2) - (form.Height / 2));
			return (new Point(x, y));
		}

		void editChoicesDialog_FormClosed(object sender, FormClosedEventArgs e)
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

		private IFormView view;
	}

}
