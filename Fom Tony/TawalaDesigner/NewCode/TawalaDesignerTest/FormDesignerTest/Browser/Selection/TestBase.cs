// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Browser;
using Tawala.FormDesigner;
using Tawala.Interfaces;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.FormDesignerTest.Browser.Selection
{
    public abstract class TestBase
    {
        private BrowserControl browser;
        protected IFormView View { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        [TearDown]
        public void TearDown()
        {
            browser = null;
            ((FormView)View).Close();
            View = null;
        }

        protected void CreateViewWithTextItem(params string[] paragraphXmlContents)
        {
            XmlElement textItemXml = createTextItemXmlElement(paragraphXmlContents);
            createViewWithTextItem(textItemXml);
        }

        protected string GetParagraphHtmlFromBody()
        {
            return Regex.Match(browser.Document.Body.InnerHtml, "<P .*</P>", RegexOptions.Singleline).Value;
        }

        protected void SelectViewContents(string text)
        {
            string scriptFormat = @"var range = document.body.createTextRange(); range.findText('{0}'); range.select();";
            scriptEval(string.Format(scriptFormat, text));
        }

        protected void SelectViewContentsAcrossParagraphs(string p1Text, string p2Text)
        {
            string scriptFormat =
                @"var range1 = document.body.createTextRange(); range1.findText('{0}'); " +
                @"var range2 = document.body.createTextRange(); range2.findText('{1}'); " +
                @"range1.setEndPoint('EndToEnd', range2); " +
                @"range1.select();";

            scriptEval(string.Format(scriptFormat, p1Text, p2Text));
        }

        protected void SelectViewContentsAcrossParagraphs(string p1Text, string p2Text, string p3Text)
        {
            string scriptFormat =
                @"var range1 = document.body.createTextRange(); range1.findText('{0}'); " +
                @"var range2 = document.body.createTextRange(); range2.findText('{1}'); " +
                @"var range3 = document.body.createTextRange(); range3.findText('{2}'); " +
                @"range1.setEndPoint('EndToEnd', range3); " +
                @"range1.select();";

            scriptEval(string.Format(scriptFormat, p1Text, p2Text, p3Text));
        }

        protected void SelectViewContentsAcrossParagraphs(params string[] selectionStrings)
        {
            const string rangeAssignmentFormat = @"var range{0} = document.body.createTextRange(); range{0}.findText('{1}'); ";
            const string rangeSetEndpointFormat = @"range1.setEndPoint('EndToEnd', range{0}); ";

            var script = new StringBuilder();

            int rangeId;

            for (int i = 0; i < selectionStrings.Length; i++)
            {
                rangeId = i + 1;
                script.AppendFormat(rangeAssignmentFormat, rangeId, selectionStrings[i]);
            }

            rangeId = selectionStrings.Length;
            script.AppendFormat(rangeSetEndpointFormat, rangeId);

            script.Append("range1.select();");

            scriptEval(script.ToString());
        }

        protected void SelectionRemoveColor()
        {
            var selection = new HtmlSelection(View.GetSelection());
            if (selection.RemoveFontColor())
            {
                View.SetSelection(selection.ToXhtml());
            }
        }

        protected void SelectionRemoveFormatting()
        {
            browser.RemoveFormatting();
            var selection = new HtmlSelection(View.GetSelection());
            if (selection.RemoveFontFormatting())
            {
                View.SetSelection(selection.ToXhtml());
            }
        }

        protected void setFaceOnViewSelection(string face, string selectionString)
        {
            SelectViewContents(selectionString);
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontFace(face);
            View.SetSelection(selection.ToXhtml());
        }

        protected void setFaceOnMultiParagraphViewSelection(string face, params string[] selectionStrings)
        {
            SelectViewContentsAcrossParagraphs(selectionStrings);
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontFace(face);
            View.SetSelection(selection.ToXhtml());
        }

        protected void setSizeOnViewSelection(int size, string selectionString)
        {
            SelectViewContents(selectionString);
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(size);
            View.SetSelection(selection.ToXhtml());
        }

        protected void setSizeOnMultiParagraphViewSelection(int size, params string[] selectionStrings)
        {
            SelectViewContentsAcrossParagraphs(selectionStrings);
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontSize(size);
            View.SetSelection(selection.ToXhtml());
        }

        protected void setColorOnViewSelection(string color, string selectionString)
        {
            SelectViewContents(selectionString);
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontColor(color);
            View.SetSelection(selection.ToXhtml());
        }

        protected void setColorOnMultiParagraphViewSelection(string color, params string[] selectionStrings)
        {
            SelectViewContentsAcrossParagraphs(selectionStrings);
            var selection = new HtmlSelection(View.GetSelection());
            selection.SetFontColor(color);
            View.SetSelection(selection.ToXhtml());
        }

        private XmlElement createTextItemXmlElement(params string[] paragraphXmlContents)
        {
            var sb = new StringBuilder();
            sb.Append("<text>");

            foreach (string contents in paragraphXmlContents)
            {
                sb.Append("<paragraph align=\"left\" indent=\"0\">");
                sb.Append(contents);
                sb.Append("</paragraph>");
            }

            sb.Append("</text>");

            return new XmlElement(sb.ToString(), true);
        }

        private void createViewWithTextItem(XmlElement element)
        {
            IForm form = Project.Current.AddForm();
            ITextItem textItem = new NewTextItem(element);
            form.ItemList.Add(textItem);

            View = new FormView(form);

            browser = (BrowserControl)TestUtil.PumpMessagesUntilUIReady(View);
        }

        private void scriptEval(string script)
        {
            browser.Document.InvokeScript("fnEval", new object[] {script});
        }
    }
}