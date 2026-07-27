using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tawala.FormDesigner;
using Tawala.Browser;
using Tawala.Interfaces;

namespace Tawala.FormDesigner
{
	public class FormBrowserControl : BrowserControl
	{
		public FormBrowserControl()
		{
		}

		public void LoadDocument(IFormView formView)
		{
            ObjectForScripting = new FormScriptingObject(formView);

			string url = null;

			if (Assembly.GetEntryAssembly() == null)
			{
				url = Path.Combine(Path.GetTempPath(), @"FormDesignerTest\html\form.htm");
			}
			else
			{
				url = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"html\form.htm");
			}

			LoadDocument(url);
		}

		public HtmlElement ActiveFormItem
		{
			get { return ((FormScriptingObject)ObjectForScripting).ActiveFormItem; }
		}

		public void InsertHeadingItem(string contents, int index, int formItemId, string headingType)
		{
			var element = insertFormItem("t:HeadingItem", index, formItemId);
			element.SetAttribute("Contents", contents);
		}

		public void InsertTextItem(string contents, int index, int formItemId)
		{
			var element = insertFormItem("t:TextItem", index, formItemId);
			element.SetAttribute("Contents", contents);

//          Causes bug #805; (introduced for S-02890 which is on indefinite hold)
//			element.InvokeMember("SelectText");
		}

		public void InsertFibItem(string contents, int index, int formItemId)
		{
			var element = insertFormItem("t:FibItem", index, formItemId);
			element.SetAttribute("Contents", contents);
		}

		public void InsertMcqItem(string question, string choices, int index, int formItemId)
		{
			var element = insertFormItem("t:McqItem", index, formItemId);
			element.SetAttribute("Question", question);
			element.SetAttribute("Choices", choices);
		}

		public void InsertFieldItem(string name, int index, int formItemId)
		{
			var element = insertFormItem("t:FieldItem", index, formItemId);
			element.SetAttribute("FieldName", name);
		}

		public void InsertSkipItem(int index, int formItemId)
		{
			insertFormItem("t:SkipItem", index, formItemId);
		}

		public void InsertBreakItem(int index, int formItemId)
		{
			insertFormItem("t:BreakItem", index, formItemId);
		}

		public string CreateBlank()
		{
			HtmlElement element = Document.CreateElement("t:Blank");
			return element.OuterHtml;
		}

		public int GetInsertionIndex()
		{
			string result = dropArea.GetAttribute("InsertionPoint");
			return Convert.ToInt32(result);
		}

		public Collection<string> GetFormItemIds()
		{
			var formItemIds = new Collection<string>();

			foreach (HtmlElement row in formTBody.Children)
			{
				formItemIds.Add(row.Id.Replace("row_", ""));
			}

			return formItemIds;
		}

		public Collection<string> GetSelectedFormItemIds()
		{
			var formItemIds = new Collection<string>();

			foreach (HtmlElement row in formTBody.Children)
			{
				if (row.GetAttribute("Selected").ToLower() == "true")
				{
					formItemIds.Add(row.Id.Replace("row_", ""));
				}
			}

			return formItemIds;
		}

		public void ClearInsertionPoint()
		{
			dropArea.InvokeMember("setAttribute", "InsertionPoint", -1);
			formContainer.InvokeMember("setAttribute", "SelectionAnchor", null);
		}

		public void DeleteFormItemRowsById(Collection<string> formItemIds)
		{
			foreach (string id in formItemIds)
			{
				HtmlElement row = GetElementById("row_" + id);
				formTBody.InvokeMember("deleteRow", row.GetAttribute("sectionRowIndex"));
			}
		}

		public void SetLabelText(int id, string labelText)
		{
			var label = GetElementById("label_" + id);

			if (label != null && !string.IsNullOrEmpty(labelText))
			{
				label.SetAttribute("LabelText", labelText);
			}
		}

        public string GetFirstVisibleFormItemId()
        {
            if (body != null)
            {
                foreach (HtmlElement row in formTBody.Children)
                {
                    int y = row.OffsetRectangle.Top - body.ScrollTop;

                    if (y < -10)
                        continue;
                    if (y > body.ScrollRectangle.Height)
                        break;

                    return row.GetAttribute("FormItemId");
                }
            }

            return string.Empty;
        }

		public void NotifyHtmlDragDropEnded()
		{
			dropArea.InvokeMember("NotifyDragDropEnded");
		}

		public void ViewSource(IWin32Window owner)
		{
			new SourceView(formTBody.OuterHtml).ShowDialog(owner);
		}

		private HtmlElement insertFormItem(string elementName, int index, int formItemId)
		{
			var element = Document.CreateElement(elementName);
			element.Id = formItemId.ToString();
			formContainer.InvokeMember("InsertFormItem", element.DomElement, index);
			return element;
		}

		protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
		{
			formTBody = GetElementById("formTBody");
			formContainer = GetElementById("formContainer");
			dropArea = GetElementById("dropArea");
            body = Document.Body;

			base.OnDocumentCompleted(e);
		}

		private HtmlElement formTBody;
		private HtmlElement formContainer;
		private HtmlElement dropArea;
        private HtmlElement body;
	}
}
