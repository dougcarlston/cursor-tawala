// Copyright © 2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.Projects;

namespace Tawala.Browser
{
    public delegate void FunctionDoubleClickedEventHandler(int id);

    public partial class BrowserControl : WebBrowser
    {
        private const string functionIdPrefix = "func_";
        private const string linkIdPrefix = "link_";
        private BrowserObjectWrapper windowWrapper;

        protected BrowserControl()
        {
            InitializeComponent();
        }

        public HtmlElement ActiveElement
        {
            get { return Document.ActiveElement; }
        }

        public string ActiveElementId
        {
            get { return ActiveElement != null ? ActiveElement.Id : null; }
        }

        protected BrowserSelection CurrentSelection
        {
            get { return new BrowserSelection(this); }
        }

        protected BrowserObjectWrapper EventObject
        {
            get { return windowWrapper.GetWrapper("event"); }
        }

        public event EventHandler<ObjectDoubleClickedEventArgs> FunctionDoubleClicked;
        public event EventHandler<ObjectDoubleClickedEventArgs> LinkDoubleClicked;

        public HtmlElement GetElementById(string id)
        {
            return Document.GetElementById(id);
        }

        public HtmlElementCollection GetElementsByTagName(string tagname)
        {
            return Document.GetElementsByTagName(tagname);
        }

        public void SetBookmark()
        {
            CurrentSelection.SetBookmark();
        }

        public void ClearBookmark()
        {
            CurrentSelection.ClearBookmark();
        }

        public string GetHtml()
        {
            return CurrentSelection.GetHtml();
        }

        public void SetHtml(string html)
        {
            CurrentSelection.SetHtml(html);
        }

		public bool CanCut()
		{
			return queryCommandEnabled("Cut");
		}

        public void Cut()
        {
            Document.ExecCommand("Cut", false, null);
        }

		public bool CanCopy()
		{
			return queryCommandEnabled("Copy");
		}

        public void Copy()
        {
            Document.ExecCommand("Copy", false, null);
        }

		public bool CanPaste()
		{
			return queryCommandEnabled("Paste");
		}

    	private bool queryCommandEnabled(string command)
    	{
    		var domDocumentType = Document.DomDocument.GetType();
    		var flags = BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public;
    		return Convert.ToBoolean(domDocumentType.InvokeMember("queryCommandEnabled", flags, null, Document.DomDocument, new object[] { command }));
    	}

    	public void Paste()
        {
            Document.ExecCommand("Paste", false, null);
        }

		public bool CanDelete()
		{
			return queryCommandEnabled("Delete");
		}

        public void Delete()
        {
            Document.ExecCommand("Delete", false, null);
        }

		public bool CanUndo()
		{
			return queryCommandEnabled("Undo");
		}

        public void Undo()
        {
            Document.ExecCommand("Undo", false, null);
        }

		public bool CanRedo()
		{
			return queryCommandEnabled("Redo");
		}

        public void Redo()
        {
            Document.ExecCommand("Redo", false, null);
        }

        public void RemoveFormatting()
        {
            Document.ExecCommand("RemoveFormat", false, null);
        }

        public void ToggleBold()
        {
            CurrentSelection.ToggleBold();
        }

        public void ToggleItalic()
        {
            CurrentSelection.ToggleItalic();
        }

        public void ToggleUnderline()
        {
            CurrentSelection.ToggleUnderline();
        }

        public void Indent()
        {
            CurrentSelection.Indent();
        }

        public void Outdent()
        {
            CurrentSelection.Outdent();
        }

        public void AlignLeft()
        {
            CurrentSelection.AlignLeft();
        }

        public void AlignCenter()
        {
            CurrentSelection.AlignCenter();
        }

        public void AlignRight()
        {
            CurrentSelection.AlignRight();
        }

        public void AlignJustify()
        {
            CurrentSelection.AlignJustify();
        }

        public void InsertTable(int tableWidthPoints, int rows, int columns)
        {
            int columnWidthPoints = tableWidthPoints/columns;

            var tableXhtml = new StringBuilder();

            tableXhtml.AppendFormat(@"<table class=""user"" style=""width: {0}pt;""><tbody>", tableWidthPoints);
            
			for (int row = 0; row < rows; ++row)
            {
                tableXhtml.Append("<tr style='height: 12pt;'>");

                for (int column = 0; column < columns; ++column)
                {
					tableXhtml.AppendFormat("<td style='width: {0}pt;'></td>", columnWidthPoints);
				}
                
				tableXhtml.Append("</tr>");
            }

            tableXhtml.Append("</tbody></table>");

			//PasteHtml(tableXhtml.ToString());

			InvokeScript("fnInsertTable", tableXhtml.ToString());
        }

        public void InsertTableRowBeforeCurrentRow()
        {
            InvokeScript("fnInsertTableRowBefore");
        }

        public void InsertTableRowAfterCurrentRow()
        {
            InvokeScript("fnInsertTableRowAfter");
        }

        public void DeleteCurrentTableRow()
        {
            InvokeScript("fnDeleteTableRow");
        }

		public bool ColumnIsSelected()
		{
			return (bool)InvokeScript("fnTableCellIsSelected");
		}

		public double GetColumnWidthInInches()
		{
			string widthString = (string)InvokeScript("fnGetCellWidth");

			string widthInPoints = Regex.Replace(widthString, @"(\d+)pt", "$1");

			double widthInInches = Convert.ToDouble(widthInPoints)/72;
			return widthInInches;
		}

		public void SetColumnWidthInInches(double widthInInches)
		{
			string widthString = (widthInInches * 72).ToString();

			InvokeScript("fnSetColumnWidth", widthString);
		}

    	public void InsertTableColumnBeforeCurrentColumn()
        {
            InvokeScript("fnInsertTableColumnBefore");
        }

        public void InsertTableColumnAfterCurrentColumn()
        {
            InvokeScript("fnInsertTableColumnAfter");
        }

        public void DeleteCurrentTableColumn()
        {
            InvokeScript("fnDeleteTableColumn");
        }

        public void DeleteCurrentTable()
        {
            InvokeScript("fnDeleteTable");
        }

        public void InsertField(IField field)
        {
            InsertField(field.Id, field.ToString());
        }

        public void InsertField(int id, string text)
        {
            string elementHtml = string.Format("<t:field id=\"field_{0}\" fieldID=\"{1}\">{2}</t:field>",
                                               DateTime.Now.Ticks.ToString("x"), id, text);
            PasteHtml(elementHtml);
        }

        public void InsertFunction(int id, string text)
        {
            HtmlElement bookmark = GetElementsByTagName("bookmark")[0];
            string funcId = functionIdPrefix + id;
            bookmark.OuterHtml = string.Format("<t:function id=\"{0}\">{1}</t:function>",
                                               funcId,
                                               XMLStringFormatter.EscapeAttributeText(
                                                   text.Replace("<<", "").Replace(">>", "")));
        }

        public void UpdateFunction(int oldId, int newId, string text)
        {
            HtmlElement functionElement = GetElementById(functionIdPrefix + oldId);
            functionElement.Id = functionIdPrefix + newId;
            functionElement.InnerText = text.Replace("<<", "").Replace(">>", "");
        }

        public void InsertImage(string imageName)
        {
            string elementHtml = string.Format("<img src='{0}' onload='fnImageLoad()' />", imageName);
            PasteHtml(elementHtml);
        }

        public void InsertLink(int id, string text)
        {
            HtmlElement bookmark = GetElementsByTagName("bookmark")[0];
            string linkId = linkIdPrefix + id;
            string elementHtml = string.Format("<t:link id=\"{0}\">{1}</t:link>", linkId, text);
            bookmark.OuterHtml = elementHtml;
        }

        public void UpdateLink(int id, string text)
        {
            string linkId = linkIdPrefix + id;
            HtmlElement element = GetElementById(linkId);
            element.SetAttribute("DisplayText", text);
        }

        public void FormClosed()
        {
            ObjectForScripting = null;
        }

        public void PasteHtml(string xhtml)
        {
            var selection = new BrowserSelection(this);
            selection.PasteHtml(xhtml);
        }

        internal HtmlElement GetHtmlElement(object rawElement)
        {
            var elementWrapper = new BrowserObjectWrapper(rawElement);
            var id = elementWrapper.GetProperty<string>("uniqueID");
            Debug.Assert(!string.IsNullOrEmpty(id));
            return Document.GetElementById(id);
        }

        protected object InvokeScript(string functionName)
        {
            return Document.InvokeScript(functionName);
        }

        protected object InvokeScript(string functionName, params object[] arguments)
        {
            return Document.InvokeScript(functionName, arguments);
        }

        protected void LoadDocument(string url)
        {
            Navigate(url);
        }

        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            windowWrapper = new BrowserObjectWrapper(Document.Window.DomWindow);

            Document.Body.DoubleClick += body_DoubleClick;

            base.OnDocumentCompleted(e);
        }

        private void body_DoubleClick(object sender, HtmlElementEventArgs e)
        {
            object srcElement = EventObject.GetProperty("srcElement");

            if (srcElement == null)
            {
                return;
            }

            var sourceElement = new BrowserObjectWrapper(srcElement);
            var sourceElementId = sourceElement.GetProperty<string>("id");

            if (string.IsNullOrEmpty(sourceElementId))
            {
                return;
            }

            HtmlElement element = Document.GetElementById(sourceElementId);

            if (element == null || string.IsNullOrEmpty(element.Id))
            {
                return;
            }

            if (element.Id.StartsWith(functionIdPrefix))
            {
                raiseObjectDoubleClicked(element, FunctionDoubleClicked);
            }
            else if (element.Id.StartsWith(linkIdPrefix))
            {
                raiseObjectDoubleClicked(element, LinkDoubleClicked);
            }
        }

        private void raiseObjectDoubleClicked(HtmlElement element, EventHandler<ObjectDoubleClickedEventArgs> handler)
        {
            if (handler != null)
            {
                int id = Convert.ToInt32(Regex.Replace(element.Id, "[^0-9]*", ""));
                var eventArgs = new ObjectDoubleClickedEventArgs(id);
                handler(this, eventArgs);
            }
        }
    }
}