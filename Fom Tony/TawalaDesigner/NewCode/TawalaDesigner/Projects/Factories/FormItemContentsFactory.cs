// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Factories
{
    public static class FormItemContentsFactory
    {
        private static readonly Factory<IFormItemContents> formItemContentsFactory = new Factory<IFormItemContents>();

        static FormItemContentsFactory()
        {
            formItemContentsFactory.Register("#text", typeof(TextContents));
            formItemContentsFactory.Register("#whitespace", typeof(WhitespaceContents));
            formItemContentsFactory.Register("sp", typeof(WhitespaceContents));

            formItemContentsFactory.Register("p", typeof(ParagraphXhtmlContents));
            formItemContentsFactory.Register("paragraph", typeof(ParagraphXmlContents));

            formItemContentsFactory.Register("strong", typeof(BoldContents));
            formItemContentsFactory.Register("b", typeof(BoldContents));

            formItemContentsFactory.Register("em", typeof(ItalicContents));
            formItemContentsFactory.Register("i", typeof(ItalicContents));

            formItemContentsFactory.Register("u", typeof(UnderlineContents));

            formItemContentsFactory.Register("font", typeof(NewXmlFont));
            formItemContentsFactory.Register("span", typeof(NewXhtmlFont));
            formItemContentsFactory.Register("span", "class", typeof(NullContents));

            formItemContentsFactory.Register("field", typeof(FieldReference));

            formItemContentsFactory.Register("table", typeof(TableXhtmlContents));
            formItemContentsFactory.Register("table", typeof(TableXmlContents));
            formItemContentsFactory.Register("tbody", typeof(TableBodyXhtmlContents));
            formItemContentsFactory.Register("tr", typeof(TableRowXhtmlContents));
            formItemContentsFactory.Register("row", typeof(TableRowXmlContents));
            formItemContentsFactory.Register("td", typeof(TableCellXhtmlContents));
            formItemContentsFactory.Register("cell", typeof(TableCellXmlContents));
            formItemContentsFactory.Register("division", typeof(DivisionContents));

            formItemContentsFactory.Register("blank", typeof(XmlBlank));
            formItemContentsFactory.RegisterFactoryMethod(typeof(XhtmlBlank), "MakeHtmlBlank", "blank", "id");
            formItemContentsFactory.Register("blank", "new", typeof(BlankPlaceholderXhtmlContents));

            formItemContentsFactory.Register("question", typeof(Question));
            formItemContentsFactory.Register("choice", typeof(NewXhtmlChoice));
            formItemContentsFactory.Register("choice", "label", typeof(NewXmlChoice));

            formItemContentsFactory.Register("data-provider", typeof(DataXmlProvider));
            formItemContentsFactory.Register("choice", "functionId", typeof(DataXhtmlProvider));

            formItemContentsFactory.Register("function", typeof(FunctionXhtmlReference));

            formItemContentsFactory.Register("img", typeof(ImageXhtmlReference));
            formItemContentsFactory.Register("image", typeof(ImageXmlReference));

            formItemContentsFactory.Register("invitation", typeof(InvitationReference));
            formItemContentsFactory.Register("link", typeof(HyperlinkReference));
            formItemContentsFactory.RegisterFactoryMethod(typeof(LinkReference), "MakeHtmlLink", "link", "id");
        }

        public static IFormItemContents MakeObject(IXmlElement element)
        {
            IFormItemContents formItemContents = formItemContentsFactory.MakeObject(element);

            if (formItemContents == null)
            {
                formItemContents = makeFunction(element);
            }

            return (formItemContents ?? FormItemContents.NULL);
        }

        public static FormItemContentsCollection MakeChildren(IXmlElement element)
        {
            var contents = new FormItemContentsCollection();

            foreach (IXmlElement childElement in element.GetChildren())
            {
                contents.Add(MakeObject(childElement));
            }

            return contents;
        }

        public static FormItemContentsCollection MakeChildrenFromDescendants(IXmlElement element, params string[] descendantNames)
        {
            var contents = new FormItemContentsCollection();

            foreach (IXmlElement childElement in element.GetDescendants(descendantNames))
            {
                contents.Add(MakeObject(childElement));
            }

            return contents;
        }

        public static IFormItemContents MakeChildrenFromHtml(string html)
        {
            return MakeChildren(new XhtmlElement(html, true));
        }

        private static IFormItemContents makeFunction(IXmlElement element)
        {
            if (FunctionLoader.Current == null)
            {
                return null;
            }

            if (!FunctionLoader.Current.Functions.Contains(element.Name))
            {
                return null;
            }

            return new FunctionXmlReference(element);
        }
    }
}