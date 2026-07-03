// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects
{
    public class HtmlSelection
    {
        private FormItemContentsCollection paragraphs;
        private Collection<NewFont> topLevelFonts;

        public HtmlSelection(string htmlText)
        {
            processHtml(htmlText);
        }

        private void processHtml(string htmlText)
        {
            if (containsParagraphs(htmlText))
            {
                paragraphs = makeParagraphs(htmlText);
                topLevelFonts = getTopLevelFonts(paragraphs);
            }
            else
            {
                paragraphs = null;
                topLevelFonts = makeFontFromHtml(htmlText);
            }
        }

        public bool RemoveFontColor()
        {
            bool removed = false;

            foreach (NewFont font in topLevelFonts)
            {
                if (removeFontAttributes(font.GetDescendants(typeof(NewFont)), RemoveFontAttribute.Color))
                {
                    removed = true;
                }
            }

            return removed;
        }

        public bool RemoveFontFormatting()
        {
            bool removed = false;

            foreach (NewFont font in topLevelFonts)
            {
                if (removeFontAttributes(font.GetDescendants(typeof(NewFont)), RemoveFontAttribute.All))
                {
                    removed = true;
                }
            }

            return removed;
        }

        [Flags]
        private enum RemoveFontAttribute
        {
            None = 0x0,
            Color=0x01,
            Face = 0x02,
            Size = 0x04,
            All = 0x07
        } ;

        private static bool removeFontAttributes(FormItemContentsCollection fonts, RemoveFontAttribute removeFlags)
        {
            bool removed = false;

            foreach (NewFont font in fonts)
            {
                if ((removeFlags & RemoveFontAttribute.Color) == RemoveFontAttribute.Color)
                {
                    if (!string.IsNullOrEmpty(font.FontColor))
                    {
                        font.FontColor = string.Empty;
                        removed = true;
                    }
                }

                if ((removeFlags & RemoveFontAttribute.Face) == RemoveFontAttribute.Face)
                {
                    if (!string.IsNullOrEmpty(font.FontFace))
                    {
                        font.FontFace = string.Empty;
                        removed = true;
                    }
                }

                if ((removeFlags & RemoveFontAttribute.Size) == RemoveFontAttribute.Size)
                {
                    if (font.FontSizeInPoints != NewFont.DefaultFontSize)
                    {
                        font.FontSizeInPoints = NewFont.DefaultFontSize;
                        removed = true;
                    }
                }
            }

            return removed;
        }

        public void SetFontFace(string fontFace)
        {
            foreach (NewFont font in topLevelFonts)
            {
                font.FontFace = fontFace;
            }
        }

        public void SetFontSize(int fontSize)
        {
            foreach (NewFont font in topLevelFonts)
            {
                font.FontSizeInPoints = fontSize;
            }
        }

        public void SetFontColor(string fontColor)
        {
            foreach (NewFont font in topLevelFonts)
            {
                font.FontColor = fontColor;
            }
        }

        public string ToXhtml()
        {
            if (paragraphs != null)
            {
                string xhtmlString = paragraphs.ToXhtml(null);

                xhtmlString = trimParagraphs(xhtmlString);

                return xhtmlString;
            }
            else
            {
                var xhtmlString = new StringBuilder();

                foreach (NewFont font in topLevelFonts)
                {
                    xhtmlString.Append(font.ToXhtml(null));
                }

                return xhtmlString.ToString();
            }
        }

        private static Collection<NewFont> makeFontFromHtml(string htmlString)
        {
            var fonts = new Collection<NewFont>();

            if (isEnclosedBySpan(htmlString))
            {
                fonts.Add(new NewXhtmlFont(new XhtmlElement(htmlString, true)));
            }
            else
            {
                fonts.Add(new NewXhtmlFont(new XhtmlElement("<SPAN>" + htmlString + "</SPAN>", true)));
            }

            return fonts;
        }

        private static Collection<NewFont> getTopLevelFonts(FormItemContentsCollection contents)
        {
            var fonts = new Collection<NewFont>();

            foreach (IFormItemContents paragraph in contents)
            {
                if (paragraph.Contents is NewFont)
                {
                    fonts.Add(paragraph.Contents as NewFont);
                }
                else if (paragraph.Contents is FormItemContentsCollection)
                {
                    fonts.Add(((FormItemContentsCollection)paragraph.Contents)[0] as NewFont);
                }
            }

            return fonts;
        }

        private static FormItemContentsCollection makeParagraphs(string htmlText)
        {
            var paragraphs =
                FormItemContentsFactory.MakeChildrenFromHtml("<container>" + htmlText.Trim() + "</container>") as FormItemContentsCollection;

            foreach (IFormItemContents paragraph in paragraphs)
            {
                if (paragraph.Contents != null)
                {
                    if (!isEnclosedBySpan(paragraph.Contents.ToXhtml(null)))
                    {
                        NewFont font = new NewXhtmlFont(new XhtmlElement("<SPAN>" + paragraph.Contents.ToXhtml(null) + "</SPAN>", true));
                        paragraph.Contents = font;
                    }
                }
            }

            return paragraphs;
        }

        private static bool isEnclosedBySpan(string htmlString)
        {
            return Regex.IsMatch(htmlString, "^<SPAN.+</SPAN>$") || Regex.IsMatch(htmlString, "^<span.+</span>$");
        }

        private static bool containsParagraphs(string htmlString)
        {
            return Regex.IsMatch(htmlString, "<[Pp]");
        }

        private static string trimParagraphs(string htmlString)
        {
            return Regex.Replace(htmlString, @"^<p[^>]+>(<span.+</span>)+</p>$", "$1");
        }
    }
}