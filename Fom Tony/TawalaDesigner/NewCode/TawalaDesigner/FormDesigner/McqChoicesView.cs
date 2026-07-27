// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.ProjectUI;
using Tawala.MainApplication;
using Tawala.Common;

namespace Tawala.FormDesigner
{
	public partial class McqChoicesView : Form, IMcqChoicesView
	{
		private IMcqChoicesPresenter presenter = McqChoicesPresenter.NULL;
		private int choiceSourceType = NewMcqItem.StaticChoices;
		private IMcqItem mcqItem;

		public McqChoicesView(IMcqItem mcqItem)
		{
			InitializeComponent();

			this.mcqItem = mcqItem;
			MdiParent = ApplicationPresenter.MainApplicationForm;
		}

		#region IMcqChoicesView Members

		public string ChoicesText
		{
			get { return webBrowserChoices.Document.Body.InnerText; }
		}

		public string ChoicesHtml
		{
			get { return webBrowserChoices.Document.Body.OuterHtml; }
			
			set
			{
				if (webBrowserChoices.Document != null)
				{
					webBrowserChoices.Document.Body.InnerHtml = value;
				}
			}
		}

		public void InsertField(string fieldName)
		{
			//string fieldXhtml = string.Format(@"<t:field name=""{0}""/>", fieldName);
			string fieldXhtml = string.Format(@"<t:field contentEditable=""false"" fieldID=""1"">{0}</t:field>", fieldName);

			replaceSelection(fieldXhtml);
		}

		private void replaceSelection(string text)
		{
			string scriptFormat = @"var range = document.selection.createRange(); range.pasteHTML('{0}'); range.select();";
			string script = string.Format(scriptFormat, text);

			webBrowserChoices.Document.InvokeScript("fnEval", new object[] { script });
		}

		public void SetChoiceSource(int choiceSourceType)
		{
			this.choiceSourceType = choiceSourceType;
		}

		public void EnableChoiceConfiguration(bool enable)
		{
			linkLabelConfigure.Enabled = enable;
			webBrowserChoices.Enabled = !enable;
		}

		#endregion

		private string initialHtml =
			@"<html>" +
			@"<head>" +
			@"<?import namespace=""t"" implementation=""field.htc"" />" +
			@"<script  type=""text/jscript"">" +
			@"function fnEval(script){eval(script);}" +
			@"</script>" +
			@"<style>" +
			@"body" +
			@"{" +
			@"font: normal normal normal 10pt Microsoft Sans Serif;" +
			@"margin: 0px;" +
			@"padding: 0px;" +
			@"}" +
			@"p" +
			@"{" +
			@"margin-top: 0pt;" +
			@"margin-bottom: 0pt;" +
			@"}" +
			@"</style>" +
			@"</head>" +
			@"<body contentEditable=""true""></body>" +
			@"</html>";

		protected override void OnLoad(EventArgs e)
		{
			if (FieldsPalette.Palette != null)
			{
				FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
			}

			base.OnLoad(e);

			webBrowserChoices.DocumentText = initialHtml;
		}

		private void toolStripButtonOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			presenter.ChoicesAccepted();

			Close();
		}

		private void toolStripButtonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			presenter.FieldsPaletteDoubleClicked(e.Node.Tag as IField);
		}

		private void textBoxChoices_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof (IPaletteField)))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void textBoxChoices_DragDrop(object sender, DragEventArgs e)
		{
			var paletteField = e.Data.GetData(typeof (IPaletteField)) as IPaletteField;

			if (paletteField != null)
			{
				presenter.FieldDropped(paletteField);
			}
		}

		private void McqChoicesView_Load(object sender, EventArgs e)
		{
			comboBoxSource.SelectedIndex = choiceSourceType;
		}

		private void linkLabelConfigure_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			presenter.ConfigurationRequested();
		}

		private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
		{
			presenter.ChoiceSourceChanged(comboBoxSource.SelectedIndex);
		}

		private void McqChoicesView_Activated(object sender, EventArgs e)
		{
			ToolStripManager.Merge(toolStripEditChoices, "mainToolStrip");
		}

		private void McqChoicesView_Deactivate(object sender, EventArgs e)
		{
			ToolStripManager.RevertMerge("mainToolStrip");
		}

		private void toolStripButtonBold_Click(object sender, EventArgs e)
		{
			webBrowserChoices.Document.ExecCommand("Bold", false, null);
		}

		private void toolStripButtonItalic_Click(object sender, EventArgs e)
		{
			webBrowserChoices.Document.ExecCommand("Italic", false, null);
		}

		private void toolStripButtonUnderline_Click(object sender, EventArgs e)
		{
			webBrowserChoices.Document.ExecCommand("Underline", false, null);
		}

		private void webBrowserChoices_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			presenter = new McqChoicesPresenter(this, mcqItem);
			webBrowserChoices.Focus();
		}
	}
}