// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using Tawala.Browser;
using Tawala.Dialogs;
using Tawala.FormDesigner.Dialogs;
using Tawala.Proj;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.ProjectUI;
using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.ComponentDesigner;

namespace Tawala.FormDesigner
{
	public partial class FormView : ProjectComponentView, IFormView
	{
		public FormView(IForm form)
			: this()
		{
			this.Text = form.Name;
			Presenter = new FormPresenter(this, form);
			browser.DocumentCompleted += webBrowser_DocumentCompleted;
            browser.FunctionDoubleClicked += browser_FunctionDoubleClicked;
            browser.LinkDoubleClicked += browser_LinkDoubleClicked;
			browser.LoadDocument(this);
		}

		protected FormView()
		{
			InitializeComponent();
		}

		public FormView(IForm form, System.Windows.Forms.Form mdiParent, IProjectExplorerPresenter projectExplorerPresenter)
			: this(form)
		{
			this.MdiParent = mdiParent;
			this.ProjectExplorerPresenter = projectExplorerPresenter;
		}

		protected override void OnLoad(EventArgs e)
		{
			IApplicationView mainWindow = ParentForm as IApplicationView;

			if (mainWindow != null)
			{
				toolStripComboBoxFontFace.ComboBox.DisplayMember = "Name";
				toolStripComboBoxFontFace.Items.AddRange(mainWindow.GetFontList());
			}

			ApplicationPresenter.EditDropDownOpened += new ApplicationPresenter.DropDownOpenedEventHandler(applicationPresenter_EditDropDownOpened);
			base.OnLoad(e);
		}

		private void applicationPresenter_EditDropDownOpened(object sender, DropDownOpenedEventArgs e)
		{
			populateConnectPreProcessDropDown();
			enableOrDisablePreProcessConnectAndDisconnectMenuItems();

			populateConnectPostProcessDropDown();
			enableOrDisablePostProcessConnectAndDisconnectMenuItems();

			showOrHideRenameItem();
		}

		private void showOrHideRenameItem()
		{
			toolStripMenuItemRename.Visible = ProjectExplorerPresenter.FormIsSelected;
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			Presenter.ViewClosed();

			browser.FormClosed();

			base.OnFormClosed(e);
		}

		#region IFormView Members

		public string GetSelection()
		{
			return browser.GetHtml();
		}

		public void SetSelection(string html)
		{
			browser.SetHtml(html);
		}

		public HtmlElement GetHtmlElementById(string id)
		{
			return browser.GetElementById(id);
		}

		public HtmlElementCollection GetElementsByTagName(string tagname)
		{
			return browser.GetElementsByTagName(tagname);
		}

		public void SetLabelText(int id, string labelText)
		{
			browser.SetLabelText(id, labelText);
		}


		public void SetFormName(string formName)
		{
			this.Text = formName;
		}

		public Collection<string> GetFormItemIds()
		{
			return browser.GetFormItemIds();
		}

		public IFormPresenter Presenter
		{
			get;
			set;
		}

		public IProjectExplorerPresenter ProjectExplorerPresenter
		{
			get;
			private set;
		}

		public HtmlElement ActiveFormItem
		{
			get { return browser.ActiveFormItem; }
		}

		public void InsertHeadingItem(string contentsXhtml, int index, int formItemId, string headingType)
		{
			browser.InsertHeadingItem(contentsXhtml, index, formItemId, headingType);
		}

		public void InsertTextItem(string contentsXhtml, int index, int formItemId)
		{
			browser.InsertTextItem(contentsXhtml, index, formItemId);
		}

		public void InsertFibItem(string contentsXhtml, int index, int formItemId)
		{
			browser.InsertFibItem(contentsXhtml, index, formItemId);
		}

		public void InsertMcqItem(string questionXhtml, string choicesXhtml, int index, int formItemId)
		{
			browser.InsertMcqItem(questionXhtml, choicesXhtml, index, formItemId);
		}

		public void InsertFieldItem(string name, int index, int formItemId)
		{
			browser.InsertFieldItem(name, index, formItemId);
		}

		public void InsertSkipItem(int index, int formItemId)
		{
			browser.InsertSkipItem(index, formItemId);
		}

		public void InsertBreakItem(int index, int formItemId)
		{
			browser.InsertBreakItem(index, formItemId);
		}

		public string CreateBlank()
		{
			return browser.CreateBlank();
		}

		public void InsertBlank(string html)
		{
			browser.PasteHtml(html);
		}

		public string GetAttribute(string htmlId, string name)
		{
			return GetHtmlElementById(htmlId).GetAttribute(name);
		}

		public void SetAttribute(string htmlId, string name, string value)
		{
			GetHtmlElementById(htmlId).SetAttribute(name, value);
		}

		public IApplicationView ParentView
		{
			get { return ParentForm as IApplicationView; }
		}

		public void InsertField(int id, string text)
		{
			browser.InsertField(id, text);
		}

		public void InsertFunction(int id, string text)
		{
			browser.InsertFunction(id, text);
		}

		public void InsertImage(string imageName)
		{
			if (browser.ActiveFormItem != null)
			{
				browser.InsertImage(imageName);
			}
		}

		public int GetInsertionIndex()
		{
			return browser.GetInsertionIndex();
		}

		public void CopySelectedItems()
		{
			Presenter.CopyFormItems(browser.GetSelectedFormItemIds());
		}

		public void CutSelectedItems()
		{
			Collection<string> formItemIds = browser.GetSelectedFormItemIds();
			Presenter.CutFormItems(formItemIds);
			browser.DeleteFormItemRowsById(formItemIds);
			browser.ClearInsertionPoint();
		}

		public void DeleteSelectedItems()
		{
			Collection<string> formItemIds = browser.GetSelectedFormItemIds();
			browser.DeleteFormItemRowsById(formItemIds);
			browser.ClearInsertionPoint();
			Presenter.DeleteFormItems(formItemIds);
		}

		public void PasteItems()
		{
			Presenter.PasteFormItems();
		}

		public bool AnyTextItemSelected()
		{
			return Presenter.ContainsTextItem(browser.GetSelectedFormItemIds());
		}

		public bool OnlyOneTextItemSelected()
		{
			return Presenter.ContainsOnlyOneTextItem(browser.GetSelectedFormItemIds());
		}

		public string GetStyleOfSelectedTextItem()
		{
			return Presenter.GetStyleOfSelectedTextItem(browser.GetSelectedFormItemIds());
		}

		public bool AnyFibItemSelected()
		{
			return Presenter.ContainsFibItem(browser.GetSelectedFormItemIds());
		}

		public bool OnlyOneFibItemSelected()
		{
			return Presenter.ContainsOnlyOneFibItem(browser.GetSelectedFormItemIds());
		}

		public bool SelectedFibItemsHaveSameStyle()
		{
			return Presenter.SelectedFibItemsHaveSameStyle(browser.GetSelectedFormItemIds());
		}

		public string GetStyleOfFirstSelectedFibItem()
		{
			return Presenter.GetStyleOfFirstSelectedFibItem(browser.GetSelectedFormItemIds());
		}

		public bool AnyMcqItemSelected()
		{
			return Presenter.ContainsMcqItem(browser.GetSelectedFormItemIds());
		}

		public string GetStyleOfSelectedMcqItem()
		{
			return Presenter.GetStyleOfSelectedMcqItem(browser.GetSelectedFormItemIds());
		}

		public int GetColumnCountOfSelectedMcqItem()
		{
			return Presenter.GetColumnCountOfSelectedMcqItem(browser.GetSelectedFormItemIds());
		}

		public bool OnlyOneMcqItemSelected()
		{
			return Presenter.ContainsOnlyOneMcqItem(browser.GetSelectedFormItemIds());
		}

		public void NotifyHtmlDragDropEnded()
		{
			browser.NotifyHtmlDragDropEnded();
		}

		#endregion

		private void setStyleOfSelectedTextItems(string style)
		{
			Presenter.SetStyleOfTextItems(browser.GetSelectedFormItemIds(), style);
		}

		private void setStyleOfSelectedFibItems(string style)
		{
			Presenter.SetStyleOfFibItems(browser.GetSelectedFormItemIds(), style);
		}

		private void setStyleOfSelectedMcqItems(string style)
		{
			Presenter.SetStyleOfMcqItems(browser.GetSelectedFormItemIds(), style);
		}

		private void setStyleOfSelectedMcqItems(string style, int columnCount)
		{
			Presenter.SetStyleOfMcqItems(browser.GetSelectedFormItemIds(), style, columnCount);
		}

		private void hookFieldsPalette()
		{
			if (FieldsPalette.Palette != null)
			{
				FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
			}
		}

		private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			Presenter.InsertField(e.Node.Tag as IPaletteField);
		}

		private void document_MouseDown(object sender, HtmlElementEventArgs e)
		{
			hookFieldsPalette();
		}

		private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			hookFieldsPalette();
			browser.Document.MouseDown += new HtmlElementEventHandler(document_MouseDown);

            formPreviewControl.SetPreviewForm(Presenter.Form);

			Presenter.ViewInitializationCompleted();
		}

		private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.ViewSource(this);
		}

		private void toolStripComboBoxFontFace_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (toolStripComboBoxFontFace.SelectedItem is FontFamily)
			{
				FontFamily family = toolStripComboBoxFontFace.SelectedItem as FontFamily;
				HtmlSelection selection = new HtmlSelection(GetSelection());
				selection.SetFontFace(family.Name);
				SetSelection(selection.ToXhtml());
			}
		}

		private void toolStripComboBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (toolStripComboBoxFontSize.SelectedIndex >= 0)
			{
				HtmlSelection selection = new HtmlSelection(GetSelection());

				try
				{
					selection.SetFontSize(Convert.ToInt32(toolStripComboBoxFontSize.SelectedItem));
				}
				catch
				{
					selection.SetFontSize(NewFont.DefaultFontSize);
				}

				SetSelection(selection.ToXhtml());
			}
		}

		private void chooseColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			cd.AllowFullOpen = true;
			cd.FullOpen = true;
			cd.AnyColor = true;

			if (cd.ShowDialog(ParentForm) == DialogResult.OK)
			{
				Color color = cd.Color;
				string colorString = String.Format("{0:X6}", (color.R << 16) + (color.G << 8) + color.B);

				HtmlSelection selection = new HtmlSelection(GetSelection());
				selection.SetFontColor(colorString);
				SetSelection(selection.ToXhtml());
			}
		}

		private void toolStripButtonBold_Click(object sender, EventArgs e)
		{
			browser.ToggleBold();
		}

		private void toolStripButtonItalic_Click(object sender, EventArgs e)
		{
			browser.ToggleItalic();
		}

		private void toolStripButtonUnderline_Click(object sender, EventArgs e)
		{
			browser.ToggleUnderline();
		}

		private void toolStripButtonOutdent_Click(object sender, EventArgs e)
		{
			browser.Outdent();
		}

		private void toolStripButtonIndent_Click(object sender, EventArgs e)
		{
			browser.Indent();
		}

		private void formView_Activated(object sender, EventArgs e)
		{
			if (ActiveFormItem != null)
			{
				ActiveFormItem.RemoveFocus();				
			}

			if (ProjectExplorerPresenter != null)
			{
				ProjectExplorerPresenter.FormSelected(this);
			}

			ToolStripManager.Merge(formToolStrip, "mainToolStrip");
		}

		private void formView_Deactivate(object sender, EventArgs e)
		{
			if (ProjectExplorerPresenter != null)
			{
				ProjectExplorerPresenter.FormDeselected();
			}

			ToolStripManager.RevertMerge("mainToolStrip");
		}

		private void toolStripButtonCut_Click(object sender, EventArgs e)
		{
			CutSelectedItems();
		}

		private void toolStripButtonCopy_Click(object sender, EventArgs e)
		{
			CopySelectedItems();
		}

		private void toolStripButtonPaste_Click(object sender, EventArgs e)
		{
			PasteItems();
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			DeleteSelectedItems();
		}

		private void functionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HtmlElement activeFormItem = getActiveFunctionFormItem();
			if (activeFormItem == null) return;

			string activeElementId = browser.ActiveElementId;

			if (activeElementId != null && activeElementId.StartsWith("func_"))
			{
				int functionId = Convert.ToInt32(activeElementId.Replace("func_", ""));
				Tawala.Functions.Controls.ConfigureFunctionDialog.Presenter.EditFunction(Project.FunctionMapById[functionId], functionEdited);
			}
			else
			{
				browser.SetBookmark();
				Tawala.Functions.Controls.InsertFunctionDialog insertFunctionDialog = new Tawala.Functions.Controls.InsertFunctionDialog(functionInserted);
				insertFunctionDialog.ShowDialog(ParentForm);
			}
		}

		private void browser_FunctionDoubleClicked(object sender, ObjectDoubleClickedEventArgs e)
		{
			Tawala.Functions.Controls.ConfigureFunctionDialog.Presenter.EditFunction(Project.FunctionMapById[e.Id], functionEdited);
		}

		private HtmlElement getActiveFunctionFormItem()
		{
			HtmlElement activeFormItem = browser.ActiveFormItem;
			if (activeFormItem == null) return null;
			if (string.Compare(activeFormItem.GetAttribute("FunctionsAllowed"), "true", true) != 0) return null;
			return activeFormItem;
		}

		private void functionInserted(object sender, Tawala.Functions.ViewPresenter.FunctionConfiguredEventArgs args)
		{
			if (!args.Canceled)
			{
				Presenter.InsertFunction(args.EditedInstance);
			}
			browser.ClearBookmark();
		}

		private void functionEdited(object sender, Tawala.Functions.ViewPresenter.FunctionConfiguredEventArgs args)
		{
			if (!args.Canceled)
			{
				Project.FunctionMapById.Remove(args.OriginalInstance.InstanceId);
				Project.FunctionMapById.AddUnique(args.EditedInstance);
				browser.UpdateFunction(args.OriginalInstance.InstanceId, args.EditedInstance.InstanceId, args.EditedInstance.ToDisplayString());
			}
		}

		private void toolStripMenuItemFibStyle_Click(object sender, EventArgs e)
		{
			IFibItemStylesView stylesView = new FibItemStylesView(this);

			if (stylesView.ShowDialog() == DialogResult.OK)
			{
				if (stylesView.FibApplyAllSpecified)
				{
					setAllFibItemsToChosenStyle(stylesView);
				}
				else
				{
					setFibItemsToChosenStyle(stylesView);
				}
			}
		}

		private void setAllFibItemsToChosenStyle(IFibItemStylesView stylesView)
		{
			if (stylesView.FibFreeformSpecified)
			{
				Presenter.SetStyleOfAllFibItems("freeform");
			}
			else if (stylesView.FibLeftLabelsSpecified)
			{
				Presenter.SetStyleOfAllFibItems("leftAlignLabels");
			}
			else if (stylesView.FibRightLabelsSpecified)
			{
				Presenter.SetStyleOfAllFibItems("rightAlignLabels");
			}
			else if (stylesView.FibLeftLabelsJustifiedSpecified)
			{
				Presenter.SetStyleOfAllFibItems("leftAlignLabelsJustified");
			}
			else if (stylesView.FibRightLabelsJustifiedSpecified)
			{
				Presenter.SetStyleOfAllFibItems("rightAlignLabelsJustified");
			}
			else if (stylesView.FibTopLabelsSpecified)
			{
				Presenter.SetStyleOfAllFibItems("topLabels");
			}
		}

		private void setFibItemsToChosenStyle(IFibItemStylesView stylesView)
		{
			if (stylesView.FibFreeformSpecified)
			{
				setStyleOfSelectedFibItems("freeform");
			}
			else if (stylesView.FibLeftLabelsSpecified)
			{
				setStyleOfSelectedFibItems("leftAlignLabels");
			}
			else if (stylesView.FibRightLabelsSpecified)
			{
				setStyleOfSelectedFibItems("rightAlignLabels");
			}
			else if (stylesView.FibLeftLabelsJustifiedSpecified)
			{
				setStyleOfSelectedFibItems("leftAlignLabelsJustified");
			}
			else if (stylesView.FibRightLabelsJustifiedSpecified)
			{
				setStyleOfSelectedFibItems("rightAlignLabelsJustified");
			}
			else if (stylesView.FibTopLabelsSpecified)
			{
				setStyleOfSelectedFibItems("topLabels");
			}

			refreshFormPreview();
		}

		private void toolStripMenuItemMcqStyle_Click(object sender, EventArgs e)
		{
			IMcqItemStylesView stylesView = new McqItemStylesView(this);

			if (stylesView.ShowDialog() == DialogResult.OK)
			{
				if (stylesView.McqApplyAllSpecified)
				{
					setAllMcqItemsToChosenStyle(stylesView);
				}
				else
				{
					setSelectedMCQStyles(stylesView);
				}
			}
		}
		private void setAllMcqItemsToChosenStyle(IMcqItemStylesView stylesView)
		{
			if (stylesView.MCQVerticalSpecified)
			{
				Presenter.SetStyleOfAllMcqItems("vertical");
			}
			else if (stylesView.MCQHorizontalSpecified)
			{
				Presenter.SetStyleOfAllMcqItems("horizontal");
			}
			else if (stylesView.MCQMultiColumnSpecified)
			{
				Presenter.SetStyleOfAllMcqItems("multicolumn", stylesView.MCQMultiColumnCount);
			}
		}

		private void setSelectedMCQStyles(IMcqItemStylesView stylesView)
		{
			if (stylesView.MCQVerticalSpecified)
			{
				setStyleOfSelectedMcqItems("vertical");
			}
			else if (stylesView.MCQHorizontalSpecified)
			{
				setStyleOfSelectedMcqItems("horizontal");
			}
			else if (stylesView.MCQMultiColumnSpecified)
			{
				setStyleOfSelectedMcqItems("multicolumn", stylesView.MCQMultiColumnCount);
			}

			refreshFormPreview();
		}

		private void toolStripMenuItemTextStyle_Click(object sender, EventArgs e)
		{
			ITextItemStylesView stylesView = new TextItemStylesView(this);

			if (stylesView.ShowDialog() == DialogResult.OK)
			{
				if (stylesView.TextApplyAllSpecified)
				{
					setAllTextItemsToChosenStyle(stylesView);
				}
				else
				{
					setTextItemsToChosenStyle(stylesView);
				}
			}
		}

		private void setAllTextItemsToChosenStyle(ITextItemStylesView stylesView)
		{
			if (stylesView.TextNormalSpecified)
			{
				Presenter.SetStyleOfAllTextItems("normal");
			}
			else if (stylesView.TextInstructionalSpecified)
			{
				Presenter.SetStyleOfAllTextItems("instructional");
			}
			else if (stylesView.TextErrorSpecified)
			{
				Presenter.SetStyleOfAllTextItems("error");
			}
		}

		private void setTextItemsToChosenStyle(ITextItemStylesView stylesView)
		{
			if (stylesView.TextNormalSpecified)
			{
				setStyleOfSelectedTextItems("normal");
			}
			else if (stylesView.TextInstructionalSpecified)
			{
				setStyleOfSelectedTextItems("instructional");
			}
			else if (stylesView.TextErrorSpecified)
			{
				setStyleOfSelectedTextItems("error");
			}

			refreshFormPreview();
		}

		private static void refreshFormPreview()
		{
			Project.Events.RaiseThemeChangedEvent();
		}

		private void insertTableToolStripMenuItem_Click(object sender, EventArgs e)
		{
            InsertTableView view = new InsertTableView();
            if (view.ShowDialog(this) == DialogResult.OK)
            {
                browser.InsertTable(view.TableWidthInPoints, view.Rows, view.Columns);
            }
		}


		private void columnBeforeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.InsertTableColumnBeforeCurrentColumn();
		}

		private void columnAfterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.InsertTableColumnAfterCurrentColumn();
		}

		private void rowBeforeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.InsertTableRowBeforeCurrentRow();
		}

		private void rowAfterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.InsertTableRowAfterCurrentRow();
		}

		private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.DeleteCurrentTableRow();
		}

		private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.DeleteCurrentTableColumn();
		}


        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browser.DeleteCurrentTable();
        }

        private void tableFormatToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void headingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.InsertHeadingItem(-1);
		}

		private void textItemToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.InsertTextItem(-1);
		}

		private void fillInTheBlankToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.InsertFibItem(-1);
		}

		private void multipleChoiceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.InsertMcqItem(-1);
		}

		private void hiddenFieldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.InsertFieldItem(-1);
		}

		private void pageBreakToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.InsertBreakItem(-1);
		}

		private void skipInstructionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Presenter.InsertSkipItem(-1);
		}

		private void imageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string fileName = ImageFileDialog.Browse(this);

			if (fileName != null)
			{
				Presenter.InsertImage(fileName);
			}
		}

		private void alignLeftToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.AlignLeft();
		}

		private void alignCenterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.AlignCenter();
		}

		private void alignRightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.AlignRight();
		}

		private void justifyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			browser.AlignJustify();
		}

		private void startingPointToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProjectExplorerPresenter.StartingPointToggleRequested();
		}

		private void startingPointToolStripMenuItem_Paint(object sender, PaintEventArgs e)
		{
			startingPointToolStripMenuItem.Checked =  ProjectExplorerPresenter.SelectedFormIsStartingPoint;
		}

		private void populateConnectPreProcessDropDown()
		{
			connectPreProcessToolStripMenuItem.DropDownItems.Clear();

			foreach (string processName in ProjectExplorerPresenter.GetProcessNames())
			{
				ToolStripItem processMenuItem = connectPreProcessToolStripMenuItem.DropDownItems.Add(processName);
				processMenuItem.Click += new EventHandler(connectPreProcessToolStripMenuItem_Click);
			}
		}

		private void enableOrDisablePreProcessConnectAndDisconnectMenuItems()
		{
			connectPreProcessToolStripMenuItem.Enabled = ProjectExplorerPresenter.CanConnectPreProcess(this);
			disconnectPreProcessToolStripMenuItem.Enabled = ProjectExplorerPresenter.CanDisconnectPreProcess(this);
		}

		private void connectPreProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

			if (menuItem != null)
			{
				ProjectExplorerPresenter.PreProcessConnectionRequested(this, menuItem.Text);
			}
		}

		private void disconnectPreProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProjectExplorerPresenter.PreProcessDisconnectionRequested(this);
		}

		private void populateConnectPostProcessDropDown()
		{
			connectPostProcessToolStripMenuItem.DropDownItems.Clear();

			foreach (string processName in ProjectExplorerPresenter.GetProcessNames())
			{
				ToolStripItem processMenuItem = connectPostProcessToolStripMenuItem.DropDownItems.Add(processName);
				processMenuItem.Click += new EventHandler(connectPostProcessToolStripMenuItem_Click);
			}
		}

		private void enableOrDisablePostProcessConnectAndDisconnectMenuItems()
		{
			connectPostProcessToolStripMenuItem.Enabled = ProjectExplorerPresenter.CanConnectPostProcess(this);
			disconnectPostProcessToolStripMenuItem.Enabled = ProjectExplorerPresenter.CanDisconnectPostProcess(this);
		}

		private void connectPostProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

			if (menuItem != null)
			{
				ProjectExplorerPresenter.PostProcessConnectionRequested(this, menuItem.Text);
			}
		}

		private void disconnectPostProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProjectExplorerPresenter.PostProcessDisconnectionRequested(this);
		}

		private void formView_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (userClickedCloseBox(e))
			{
				hideInsteadOfClosing(e);
			}
		}

        private void invitationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = getActiveLinkId();

            if (id != -1)
            {
                LinkEditor.EditLink(id, linkEditor_EditLinkComplete);
            }
            else
            {
                browser.SetBookmark();
                LinkEditor.NewInvitation(linkEditor_NewLink);
            }
        }

        private void hyperlinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = getActiveLinkId();

            if (id != -1)
            {
                LinkEditor.EditLink(id, linkEditor_EditLinkComplete);
            }
            else
            {
                browser.SetBookmark();
                LinkEditor.NewHyperlink(linkEditor_NewLink);
            }
        }

        private int getActiveLinkId()
        {
            string linkIdPrefix = "link_";

            if (browser.ActiveElementId != null && browser.ActiveElementId.StartsWith(linkIdPrefix))
            {
                return Convert.ToInt32(browser.ActiveElementId.Replace(linkIdPrefix, ""));
            }
            return -1;
        }

        private void linkEditor_NewLink(object sender, LinkEditCompleteEventArgs e)
        {
            if (!e.Canceled)
            {
                browser.InsertLink(e.Link.Id, e.Link.DisplayText);
            }
            browser.ClearBookmark();
        }

        private void browser_LinkDoubleClicked(object sender, ObjectDoubleClickedEventArgs e)
        {
            LinkEditor.EditLink(e.Id, linkEditor_EditLinkComplete);
        }

        private void linkEditor_EditLinkComplete(object sender, LinkEditCompleteEventArgs e)
        {
            if (!e.Canceled)
            {
                browser.UpdateLink(e.Link.Id, e.Link.DisplayText);
            }
        }
        private void formInsertMergeToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool enable = ActiveFormItem != null &&
                ActiveFormItem.GetAttribute("nodeName").CompareTo("TextItem") == 0 && 
                !Tawala.Dialogs.LinkEditor.IsDialogActive;

            hyperlinkToolStripMenuItem.Enabled = enable;
            invitationToolStripMenuItem.Enabled = enable;
        }

		private void toolStripMenuItemRename_Click(object sender, EventArgs e)
		{
			ProjectExplorerPresenter.EditComponentName();
		}

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
			if (e.TabPage == tabPagePreview)
			{
                string id = browser.GetFirstVisibleFormItemId();
                string anchor = Presenter.CreateFormPreviewAnchor(id);
                formPreviewControl.Activate(anchor);
            }
			else
			{
				formPreviewControl.Deactivate();
			}
		}
	}
}
