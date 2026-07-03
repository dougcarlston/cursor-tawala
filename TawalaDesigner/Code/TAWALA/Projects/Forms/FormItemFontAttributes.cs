// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;
using Tawala.Projects.Documents;
using Tawala.Projects.Factories;
using Tawala.Projects.Function;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    [Serializable]
    public class FormItemFontAttributes : FontAttributes
    {
        private static readonly Factory<IParagraphComponent> componentFactory = new Factory<IParagraphComponent>();

        static FormItemFontAttributes()
        {
            componentFactory.Register("#text", typeof(FormItemText));
            componentFactory.Register("#whitespace", typeof(FormItemText));
            componentFactory.Register("sp", typeof(FormItemSpace));
            componentFactory.Register("field", "name", typeof(FormItemNamedField));
            componentFactory.Register("image", "id", typeof(GraphicImageReference));
            //componentFactory.Register("image", "format", typeof(GraphicImage));
            componentFactory.Register("image", typeof(GraphicImage));
            componentFactory.Register("b", typeof(BoldText));
            componentFactory.Register("i", typeof(ItalicText));
            componentFactory.Register("u", typeof(UnderlineText));
            componentFactory.Register("blank", typeof(Blank));
            componentFactory.Register("functionField", typeof(DocumentIdedFunctionField), "instanceId");

            IFunctionRepository functionRepository = FunctionLoader.Repository;
            if (functionRepository != null)
            {
                foreach (IFunctionInfo info in functionRepository.Functions)
                {
                    componentFactory.Register(info.Id, typeof(DocumentPersistedFunctionField));
                }
            }
        }

        public FormItemFontAttributes()
        {
        }

        public FormItemFontAttributes(IXmlElement element) : base(element)
        {
            IXmlElement childElement = element.GetChild(0);
            IParagraphComponent component = componentFactory.MakeObject(childElement);

            contents = (component == null ? NULL : component);
        }

        public FormItemFontAttributes(IXmlElement element, FibItem owner)
        {
            Face = element.GetAttribute("face");
            Size = Convert.ToInt32(element.GetAttribute("size"));
            Color = Convert.ToInt32(element.GetAttribute("color"), 16);

            IXmlElement childElement = element.GetChild(0);
            IParagraphComponent component;

            if (childElement.Name == "blank")
            {
                component = owner.BlankList.MakeBlank(childElement, owner);
                owner.BlankList.BlankIndex++;
            }
            else
            {
                component = componentFactory.MakeObject(childElement);
            }

            contents = (component == null ? NULL : component);
        }

        public override string ToRtf(RtfDocument document)
        {
            string rtfString = @"{{\f{0}\fs{1}\cf{2} {3}}}";

            int fontIndex = document.FontTable.IndexMatching(Face);
            int colorIndex = document.ColorTable.IndexMatching(Color);

            return string.Format(rtfString, fontIndex, TwipsToHalfPoints(Size), ZeroBasedToOneBased(colorIndex), contents.ToRtf());
        }
    }
}