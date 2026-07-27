using System;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public class NullContents : FormItemContents
    {
        public NullContents()
        {
        }

        public NullContents(IFormItemContents contents)
        {
            Contents = contents;
        }

        public NullContents(IXmlElement element)
        {
        }

        public override string Text
        {
            get { return string.Empty; }
        }
    }
}