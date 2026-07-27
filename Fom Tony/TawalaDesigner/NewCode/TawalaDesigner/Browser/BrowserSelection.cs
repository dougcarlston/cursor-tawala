// Copyright © 2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tawala.Browser
{
    public class BrowserSelection
    {
        private readonly BrowserControl browser;
        private readonly HtmlDocument document;
        private readonly BrowserObjectWrapper range;
        private readonly BrowserObjectWrapper selection;

        public BrowserSelection(BrowserControl browser)
        {
            this.browser = browser;
            document = browser.Document;
            selection = BrowserObjectWrapper.GetWrapper(document.DomDocument, "selection");
            range = new BrowserObjectWrapper(selection.InvokeMethod("createRange"));
        }

        public string Text
        {
            get { return range.GetProperty<string>("text"); }
        }

        public void PasteHtml(string xhtml)
        {
            range.InvokeMethod("pasteHTML", xhtml);
        }

        public string GetHtml()
        {
            string text = range.GetProperty<string>("htmlText") ?? string.Empty;
            return text.Replace("\r\n", "");
        }

        public void Clear()
        {
            selection.InvokeMethod("clear");
        }

        public void SetBookmark()
        {
            ClearBookmark();
            PasteHtml("<t:bookmark/>");
        }

        public void ClearBookmark()
        {
            foreach (HtmlElement bookmark in document.GetElementsByTagName("bookmark"))
            {
                bookmark.Parent.InvokeMember("removeChild", bookmark.DomElement);
            }
        }

        public void SetHtml(string html)
        {
            clearRange();
            PasteHtml(html);
        }

        public HtmlElement ParentElement()
        {
            return browser.GetHtmlElement(range.InvokeMethod("parentElement"));
        }

        public void ToggleBold()
        {
            range.InvokeMethod("execCommand", "Bold");
        }

        public void ToggleItalic()
        {
            range.InvokeMethod("execCommand", "Italic");
        }

        public void ToggleUnderline()
        {
            range.InvokeMethod("execCommand", "Underline");
        }

        public void Indent()
        {
            offsetParagraphMargin(36);
        }

        public void Outdent()
        {
            offsetParagraphMargin(-36);
        }

        public void AlignLeft()
        {
            setParagraphAlign("left");
        }

        public void AlignCenter()
        {
            setParagraphAlign("center");
        }

        public void AlignRight()
        {
            setParagraphAlign("right");
        }

        public void AlignJustify()
        {
            setParagraphAlign("justify");
        }

        private void setParagraphAlign(string value)
        {
            foreach (HtmlElement paragraph in getSelectedParagraphs())
            {
                paragraph.SetAttribute("align", value);
            }
        }

        private Collection<HtmlElement> getSelectedParagraphs()
        {
            HtmlElement parentElement = ParentElement();
            var paragraphs = new Collection<HtmlElement>();

            while (parentElement != null)
            {
                if (parentElement.TagName.ToLower() == "p")
                {
                    paragraphs.Add(parentElement);
                    break;
                }

                parentElement = parentElement.Parent;
            }

            return paragraphs;
        }

        private void clearRange()
        {
            HtmlElement parentElement = ParentElement();
            if (parentElement != null && parentElement.TagName.ToLower() == "span")
            {
                if (Text == parentElement.InnerText)
                {
                    parentElement.InvokeMember("removeNode", true);
                    return;
                }
            }

            Clear();
        }

        private void offsetParagraphMargin(int offset)
        {
            foreach (HtmlElement paragraph in getSelectedParagraphs())
            {
                BrowserObjectWrapper style = BrowserObjectWrapper.GetWrapper(paragraph, "style");

                int margin = 0;

                var marginLeft = style.GetProperty<string>("marginLeft");

                if (!string.IsNullOrEmpty(marginLeft))
                {
                    Match match = Regex.Match(marginLeft, "[0-9]+");

                    if (match != null)
                    {
                        margin = Convert.ToInt32(match.Value);
                    }
                }

                margin += offset;
                margin = margin < 0 ? 0 : margin;

                style.SetProperty("marginLeft", margin + "pt");
            }
        }
    }
}